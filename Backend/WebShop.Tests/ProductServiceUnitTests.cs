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
    private List<Product> products;
    private Faker<Product> productFaker;
    private Mock<AppDbContext> mockDbContext;
    private Mock<DbSet<Product>> mockDbSet;
    private IMapper mapper;

    public ProductServiceUnitTests()
    {
        products = new List<Product>();
        mockDbSet = products.AsQueryable().BuildMockDbSet();
        mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());

        productFaker = new Faker<Product>()
            .CustomInstantiator(f =>
                new Product(
                    f.Commerce.Product(),
                    10
                )
            )
            .RuleFor(p => p.Name, f => f.Commerce.Product())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => f.Random.Int(200, 2000)
        );

        var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new WebApiProfile());
            }
        );

        mapper = new Mapper(mapperConfiguration);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async void GetProductCount(int count)
    {
        // Arrange
        GenerateAndAddProducts(count);
        ConfigureDbContextToReturnProducts();
        var service = new ProductService(mockDbContext.Object, mapper);

        // Act
        var resultCount = await service.GetProductCountAsync();

        // Assert
        VerifySaveChangesIsNotCalled();
        resultCount.Should().Be(count);
    }

    [Fact]
    public async void GetProductAsync()
    {
        // Arrange
        var idToAdd = 1;
        AddProductsWithId(idToAdd);
        ConfigureDbContextToReturnProducts();
        var service = new ProductService(mockDbContext.Object, mapper);

        // Act
        var outProduct = await service.GetProductAsync(1);

        // Assert
        VerifySaveChangesIsNotCalled();
        VerifyCorrectProductReturned(outProduct);
    }

    [Fact]
    public async void GetProductsAsync()
    {
        // Arrange
        ConfigureDbContextToReturnProducts();
        var service = new ProductService(mockDbContext.Object, mapper);

        // Act
        var outProducts = await service.GetProductsAsync();

        // Assert
        VerifySaveChangesIsNotCalled();
        VerifyAllProductReturned(outProducts);
    }

    [Fact]
    public async void InsertProduct()
    {
        // Arrange
        var newProduct = new ProductIn() { Name = "asd", CategoryId = 1, Description = "asd", Price = 1000};
        ConfigureDbContextToInsertProduct();
        ConfigureDbContextToReturnProducts();
        var service = new ProductService(mockDbContext.Object, mapper);

        // Act
        var outProduct = await service.InsertProductAsync(newProduct);

        // Assert
        VerifyInsertAsyncIsCalledOnce();
        VerifySaveChangesAsyncCalledOnce();
        outProduct.Id.Should().NotBe(null);
        outProduct.Should().BeEquivalentTo(newProduct);
    }

    private void ConfigureDbContextToInsertProduct()
    {
        mockDbSet
            .Setup(set => set.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback((Product entity, CancellationToken _) => products.Add(entity));
    }

    [Fact]
    public async void DeleteExistingProduct()
    {
        // Arrange
        var idToDelete = 1;
        AddProductsWithId(idToDelete);
        ConfigureDbContextToReturnProducts();
        ConfigureDbSetToBeAbleToRemoveProduct();
        var service = new ProductService(mockDbContext.Object, mapper);

        // Act
        await service.DeleteProductAsync(idToDelete);

        // Assert
        VerifySaveChangesAsyncCalledOnce();
        products.Count.Should().Be(0);
    }

    [Fact]
    public async void DeleteNonExistingProduct()
    {
        // Arrange
        ConfigureDbSetToBeAbleToRemoveProduct();
        ConfigureDbContextToThrowErrorOnSaveChanges();
        ConfigureDbContextToReturnProducts();
        var service = new ProductService(mockDbContext.Object, mapper);

        // Act/Assert
        Func<Task> call = async () => await service.DeleteProductAsync(1);

        // Assert
        await call.Should().ThrowAsync<EntityNotFoundException>();
        VerifyRemoveIsCalledOnce();
        VerifySaveChangesAsyncCalledOnce();
    }

    private void ConfigureDbContextToReturnProducts()
    {
        mockDbContext
            .Setup(ctx => ctx.Products)
            .Returns(mockDbSet.Object);
    }

    private void ConfigureDbContextToThrowErrorOnSaveChanges()
    {
        mockDbContext
            .Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new DbUpdateConcurrencyException());
    }

    private void ConfigureDbSetToBeAbleToRemoveProduct()
    {
        mockDbSet
            .Setup(set => set.Remove(It.IsAny<Product>()))
            .Callback((Product entity) => products.RemoveAll(p => p.Id == entity.Id));
    }
    private void GenerateAndAddProducts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddProductsWithId(i + 1);
        }
    }

    private void AddProductsWithId(params int[] ids)
    {
        products.AddRange(
            ids.Select(id =>
            {
                var product = productFaker.Generate();
                product.Id = id;
                return product;
            })
        );
    }

    private void VerifyInsertAsyncIsCalledOnce()
    {
        mockDbSet.Verify(set => set.Add(It.IsAny<Product>()), Times.Never());
        mockDbSet.Verify(set => set.AddAsync(It.IsAny<Product>() ,It.IsAny<CancellationToken>()), Times.Once());
    }

    private void VerifyRemoveIsCalledOnce()
    {
        mockDbSet.Verify(set => set.Remove(It.IsAny<Product>()), Times.Once());
    }

    private void VerifySaveChangesIsNotCalled()
    {
        mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
    }

    private void VerifySaveChangesAsyncCalledOnce()
    {
        mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    private void VerifyCorrectProductReturned(ProductOut outProduct)
    {
        outProduct.Should().BeEquivalentTo(
            products[0],
            opt => opt.ExcludingMissingMembers()
        );
    }

    private void VerifyAllProductReturned(IEnumerable<ProductOut> outProducts)
    {
        outProducts.Should().BeEquivalentTo(
            products,
            opt => opt.ExcludingMissingMembers()
        );
    }
}
