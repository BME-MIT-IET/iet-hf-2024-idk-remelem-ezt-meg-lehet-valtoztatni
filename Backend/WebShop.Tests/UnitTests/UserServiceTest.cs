using AutoMapper;
using Bogus;
using Moq;
using WebShop.Dal.Entities;
using WebShop.Dal;
using Microsoft.EntityFrameworkCore;
using WebShop.Bll.Services;
using FluentAssertions;
using WebShop.Bll.Dtos;
using MockQueryable.Moq;
using WebShop.Bll.Exceptions;
using WebShop.Bll.Interfaces;

namespace WebShop.Tests.UnitTests;

public class UserServiceTest
{
    private List<User> users;
    private Faker<User> userFaker;
    private Mock<AppDbContext> mockDbContext;
    private Mock<DbSet<User>> mockDbSet;
    private TestHelper<User> testHelper;
    private UserService userService;
    private IMapper mapper;

    public UserServiceTest()
    {
        users = new List<User>();
        mockDbSet = users.AsQueryable().BuildMockDbSet();
        mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
        userFaker = new Faker<User>()
            .CustomInstantiator(f => new User("", "", ""))
            .RuleFor(p => p.Name, f => f.Commerce.Product())
            .RuleFor(p => p.Email, f => f.Internet.Email())
            .RuleFor(p => p.Password, f => f.Internet.Password())
            .RuleFor(p => p.IsAdmin, false);

        testHelper = new TestHelper<User>(mockDbContext, mockDbSet, userFaker);
        testHelper.SetupFunc = ctx => ctx.Users;
        testHelper.RemoveFunc = (p1, p2) => p1.Id == p2.Id;

        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new WebApiProfile());
        }
        );

        mapper = new Mapper(mapperConfiguration);
        userService = new UserService(mockDbContext.Object, mapper);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public async void GetUserCount(int userCount)
    {
        // Arrange
        GenerateAndAddUsers(userCount);
        testHelper.ConfigureDbContextToReturnItems();
    
        // Act
        var resultCount = await userService.GetUserCountAsync();
    
        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        resultCount.Should().Be(userCount);
    }

    [Fact]
    public async void GetExistingUser()
    {
        // Arrange
        var userId = 1;
        GenerateAndAddUsersWithId(1, 2, 3);
        var userToFind = users.Find(u => u.Id == userId);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outUser = await userService.GetUserAsync(userId);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        testHelper.VerifyIsItemTheSame(userToFind, outUser);
    }

    [Fact]
    public async void GetNonExistingUser()
    {
        // Arrange
        var userId = 10;
        GenerateAndAddUsersWithId(1, 2, 3);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outUser = async () => await userService.GetUserAsync(userId);

        // Assert
        await outUser.Should().ThrowAsync<EntityNotFoundException>();
        testHelper.VerifySaveChangesIsNotCalled();
    }

    [Fact]
    public async void FindExistingUser()
    {
        // Arrange
        var userName = "testName";
        var userEmail = "test@test.com";
        var userPassword = "password123";
        var userId = 10;
        var userIn = new UserIn { Name = userName, Email = userEmail, Password = userPassword };
        var userToFind = new User(userName, userEmail, userPassword) { Id = userId };
        GenerateAndAddUsersWithId(1, 2, 3);
        users.Add(userToFind);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outUser = await userService.FindUserAsync(userIn);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        outUser.Id.Should().Be(userId);
        outUser.Name.Should().Be(userName);
    }

    [Fact]
    public async void FindUserWithIncorrectPassword()
    {
        // Arrange
        var userName = "testName";
        var userEmail = "test@test.com";
        var userPassword = "password123";
        var userIn = new UserIn { Name = userName, Email = userEmail, Password = "incorrectPassword" };
        var userToFind = new User(userName, userEmail, userPassword);
        GenerateAndAddUsersWithId(1, 2, 3);
        users.Add(userToFind);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outUser = async () => await userService.FindUserAsync(userIn);

        // Assert
        await outUser.Should().ThrowAsync<EntityNotFoundException>();
        testHelper.VerifySaveChangesIsNotCalled();
    }

    [Fact]
    public async void FindUserWithIncorrectEmail()
    {
        // Arrange
        var userName = "testName";
        var userEmail = "test@test.com";
        var userPassword = "password123";
        var userIn = new UserIn { Name = userName, Email = "incorrect@email.com", Password = userPassword };
        var userToFind = new User(userName, userEmail, userPassword);
        GenerateAndAddUsersWithId(1, 2, 3);
        users.Add(userToFind);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outUser = async () => await userService.FindUserAsync(userIn);

        // Assert
        await outUser.Should().ThrowAsync<EntityNotFoundException>();
        testHelper.VerifySaveChangesIsNotCalled();
    }

    [Fact]
    public async void InsertUser()
    {
        // Arrange
        var newUser = GenerateUserIn();
        testHelper.ConfigureDbContextToInsertItem(users);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outUser = await userService.InsertUserAsync(newUser);

        // Assert
        testHelper.VerifyInsertIsCalledOnce();
        testHelper.VerifySaveChangesCalledOnce();
        outUser.Id.Should().NotBe(null);
        outUser.Name.Should().Be(newUser.Name);
    }

    private UserIn GenerateUserIn()
    {
        var user = userFaker.Generate();
        return new UserIn() { Name = user.Name, Email = user.Email, Password = user.Password};
    }

    private void GenerateAndAddUsers(int count)
    {
        users.AddRange(GenerateUsers(count));
    }

    private void GenerateAndAddUsersWithId(params int[] ids)
    {
        users.AddRange(GenerateUsersWithId(ids));
    }

    private List<User> GenerateUsers(int count)
    {
        var ids = Enumerable.Range(1, count).ToArray();
        return GenerateUsersWithId(ids);
    }

    private List<User> GenerateUsersWithId(params int[] ids)
    {
        return testHelper.GenerateItemsWithParameters((p, id) => p.Id = id, ids);
    }
}
