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

namespace WebShop.Tests;

public class ProductServiceUnitTests
{
    private Faker<Product> _productFaker;
    private Mock<AppDbContext> _mockDbContext;
    private IMapper _mapper;

    public ProductServiceUnitTests()
    {
        _productFaker = new Faker<Product>()
            .CustomInstantiator(f => new Product("asd", 10))
            .RuleFor(p => p.Name, f => f.Commerce.Product())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => f.Random.Int(200, 2000));

        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new WebApiProfile());
        });
        _mapper = new Mapper(mapperConfiguration);
        _mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());

    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async void GetProductCount(int count)
    {
        // Arrange
        var data = _productFaker.Generate(count);
        var mock = data.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(c => c.Products).Returns(mock.Object);
        var service = new ProductService(_mockDbContext.Object, _mapper);

        // Act
        var resultCount = await service.GetProductCountAsync();

        // Assert
        _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        _mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        resultCount.Should().Be(count);
    }

    [Fact]
    public async void GetProductAsync()
    {
        // Arrange
        var data = new List<Product> { new Product("asd", 10) { Id = 1 } };
        var mock = data.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(c => c.Products).Returns(mock.Object);
        var service = new ProductService(_mockDbContext.Object, _mapper);

        // Act
        var productOut = await service.GetProductAsync(1);

        // Assert
        _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        _mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        productOut.Should().BeEquivalentTo(
            data[0],
            opt => opt.ExcludingMissingMembers()
        );
    }

    [Fact]
    public async void GetProductsAsync()
    {
        // Arrange
        var data = new List<Product> {
            new Product("asd", 10) { Id = 1 },
            new Product("asd", 10) { Id = 2 },
            new Product("asd", 10) { Id = 3 }
        };
        var mock = data.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(ctx => ctx.Products).Returns(mock.Object);
        var service = new ProductService(_mockDbContext.Object, _mapper);

        // Act
        var productsOut = await service.GetProductsAsync(null, null, null);

        // Assert
        _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        _mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        productsOut.Should().BeEquivalentTo(
            data,
            opt => opt.ExcludingMissingMembers()
        );
    }

    [Fact]
    public async void InsertProduct()
    {
        // Arrange
        var data = new List<Product>();
        var newProduct = new ProductIn() { Name = "asd", CategoryId = 1, Description = "asd", Price = 1000};
        var mock = data.AsQueryable().BuildMockDbSet();
        mock.Setup(set => set.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback((Product entity, CancellationToken _) => data.Add(entity));
        _mockDbContext.Setup(ctx => ctx.Products).Returns(mock.Object);
        var service = new ProductService(_mockDbContext.Object, _mapper);

        // Act
        var insertedProduct = await service.InsertProductAsync(newProduct);

        // Assert
        _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        _mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        insertedProduct.Id.Should().NotBe(null);
        insertedProduct.Should().BeEquivalentTo(newProduct);
    }

    [Fact]
    public async void DeleteExistingProduct()
    {
        // Arrange
        var data = new List<Product>{ new Product("asd", 10) { Id = 1 } };
        var mock = data.AsQueryable().BuildMockDbSet();
        mock.Setup(set => set.Remove(It.IsAny<Product>()))
            .Callback((Product entity) => data.RemoveAll(p => p.Id == entity.Id));
        _mockDbContext.Setup(c => c.Products).Returns(mock.Object);
        var service = new ProductService(_mockDbContext.Object, _mapper);

        // Act
        await service.DeleteProductAsync(1);

        // Assert
        _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        _mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        data.Count.Should().Be(0);
    }

    [Fact]
    public async void DeleteNonExistingProduct()
    {
        // Arrange
        var data = new List<Product> { };
        var mock = data.AsQueryable().BuildMockDbSet();
        mock.Setup(set => set.Remove(It.IsAny<Product>()));
        _mockDbContext.Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws(new DbUpdateConcurrencyException());
        _mockDbContext.Setup(ctx => ctx.Products).Returns(mock.Object);
        var service = new ProductService(_mockDbContext.Object, _mapper);

        // Act/Assert
        Func<Task> asd = async () => await service.DeleteProductAsync(1);
        await asd.Should().ThrowAsync<EntityNotFoundException>();

        // Assert
        mock.Verify(set => set.Remove(It.IsAny<Product>()), Times.Once());
        _mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        _mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
}
