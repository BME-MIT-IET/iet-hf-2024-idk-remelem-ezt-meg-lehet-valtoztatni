
using AutoMapper;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using WebShop.Bll.Dtos;
using WebShop.Bll.Exceptions;
using WebShop.Bll.Services;
using WebShop.Dal;
using WebShop.Dal.Entities;

namespace WebShop.Tests.UnitTests;

public class OrderServiceTest
{
    private List<Order> orders;
    private Faker<Order> orderFaker;
    private Mock<AppDbContext> mockDbContext;
    private Mock<DbSet<Order>> mockDbSet;
    private TestHelper<Order> testHelper;
    private OrderService orderService;
    private IMapper mapper;

    public OrderServiceTest()
    {
        orders = new List<Order>();
        mockDbSet = orders.AsQueryable().BuildMockDbSet();
        mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());

        orderFaker = new Faker<Order>()
            .RuleFor(o => o.OrderDate, f => f.Date.Recent())
            .RuleFor(o => o.UserId, f => f.Random.Int(10, 20))
            .RuleFor(o => o.OrderStatus, OrderStatus.Rejected);

        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new WebApiProfile());
        });
        
        testHelper = new TestHelper<Order>(mockDbContext, mockDbSet, orderFaker);
        testHelper.SetupFunc = ctx => ctx.Orders;
        testHelper.RemoveFunc = (p1, p2) => p1.Id == p2.Id;

        mapper = new Mapper(mapperConfiguration);
        orderService = new OrderService(mockDbContext.Object, mapper);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public async void GetOrderCount(int count)
    {
        // Arrange
        GenerateAndAddOrders(count);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var resultCount = await orderService.GetOrderCountAsync();

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        resultCount.Should().Be(count);
    }

    [Fact]
    public async void GetOrders()
    {
        // Arrange
        GenerateAndAddOrdersWithId(1, 2, 3, 4, 5);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outOrders = await orderService.GetOrdersAsync();

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        testHelper.VerifyAllItemsAreTheSame(orders, outOrders);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async void GetExistingOrder(int orderId)
    {
        // Arrange
        GenerateAndAddOrdersWithId(1, 2, 3, 4, 5);
        var searchedOrder = orders.Find(o => o.Id == orderId);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outOrder = await orderService.GetOrderAsync(orderId);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        testHelper.VerifyIsItemTheSame(searchedOrder, outOrder);
    }

    [Fact]
    public async void GetNonExistingOrder()
    {
        // Arrange
        GenerateAndAddOrdersWithId(1, 2, 3, 4, 5);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outOrder = async () => await orderService.GetOrderAsync(10);

        // Assert
        await outOrder.Should().ThrowAsync<EntityNotFoundException>();
        testHelper.VerifySaveChangesIsNotCalled();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async void GetUserOrders(int userId)
    {
        // Arrange
        GenerateAndAddOrdersUserId(1, 1, 2);
        var ordersMadeByUser = orders.FindAll(o => o.UserId == userId);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outOrders = await orderService.GetUserOrdersAsync(userId);

        // Assert
        outOrders.Count().Should().Be(ordersMadeByUser.Count);
        testHelper.VerifySaveChangesIsNotCalled();
        testHelper.VerifyAllItemsAreTheSame(ordersMadeByUser, outOrders);
    }

    [Fact]
    public async void InsertOrder()
    {
        // Arrange
        var newOrder = new OrderIn() { OrderStatus = OrderStatus.Delivered };
        var newOrderUserId = 1;
        testHelper.ConfigureDbContextToInsertItem(orders);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outOrder = await orderService.InsertOrderAsync(newOrderUserId, newOrder);

        // Assert
        outOrder.OrderDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        outOrder.OrderStatus.Should().Be(OrderStatus.Unread);
        orders.Count.Should().Be(1);
        testHelper.VerifyInsertIsCalledOnce();
        testHelper.VerifySaveChangesCalledOnce();
    }

    private void GenerateAndAddOrders(int count)
    {
        orders.AddRange(GenerateOrders(count));
    }

    private List<Order> GenerateOrders(int count)
    {
        var ids = Enumerable.Range(1, count).ToArray();
        return GenerateOrdersWithId(ids);
    }

    private void GenerateAndAddOrdersWithId(params int[] ids)
    {
        orders.AddRange(GenerateOrdersWithId(ids));
    }

    private void GenerateAndAddOrdersUserId(params int[] userIds)
    {
        orders.AddRange(GenerateOrdersWithUserId(userIds));
    }

    private List<Order> GenerateOrdersWithId(params int[] ids)
    {
        return testHelper.GenerateItemsWithParameters((o, id) => o.Id = id, ids);
    }

    private List<Order> GenerateOrdersWithUserId(params int[] userIds)
    {
        return testHelper.GenerateItemsWithParameters((o, uId) => o.UserId = uId, userIds);
    }
}
