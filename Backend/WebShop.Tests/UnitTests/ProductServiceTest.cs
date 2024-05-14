using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Bogus;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.Moq;
using Moq;
using WebShop.Bll.Dtos;
using WebShop.Bll.Exceptions;
using WebShop.Bll.Services;
using WebShop.Dal;
using WebShop.Dal.Entities;

namespace WebShop.Tests.UnitTests;

public class ProductServiceTest
{
    private List<Product> products;
    private Faker<Product> productFaker;
    private Mock<AppDbContext> mockDbContext;
    private Mock<DbSet<Product>> mockDbSet;
    private TestHelper<Product> testHelper;
    private ProductService productService;
    private IMapper mapper;

    public ProductServiceTest()
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
            .RuleFor(p => p.Price, f => f.Random.Int(200, 2000));

        testHelper = new TestHelper<Product>(mockDbContext, mockDbSet, productFaker);
        testHelper.SetupFunc = ctx => ctx.Products;
        testHelper.RemoveFunc = (p1, p2) => p1.Id == p2.Id;

        var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new WebApiProfile());
            }
        );

        mapper = new Mapper(mapperConfiguration);
        productService = new ProductService(mockDbContext.Object, mapper);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestData.ProductCountTestData), MemberType = typeof(ProductServiceTestData))]
    public async void GetProductCount(int count)
    {
        // Arrange
        GenerateAndAddProducts(count);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var resultCount = await productService.GetProductCountAsync();

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        resultCount.Should().Be(count);
    }

    [Fact]
    public async void GetExistingProduct()
    {
        // Arrange
        var productId = 1;
        GenerateAndAddProductsWithId(productId, 2, 3, 4);
        var product = products[0];
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outProduct = await productService.GetProductAsync(productId);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        testHelper.VerifyIsItemTheSame(product, outProduct);
    }

    [Fact]
    public async void GetNonExistingProduct()
    {
        // Arrange
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outProduct = async () => await productService.GetProductAsync(1);

        // Assert
        await outProduct.Should().ThrowAsync<EntityNotFoundException>();
        testHelper.VerifySaveChangesIsNotCalled();
    }

    [Fact]
    public async void GetProducts()
    {
        // Arrange
        GenerateAndAddProductsWithId(1, 2, 3, 4, 5);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outProducts = await productService.GetProductsAsync();

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        testHelper.VerifyAllItemsAreTheSame(products, outProducts);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestData.CategoryIdTestData), MemberType = typeof(ProductServiceTestData))]
    public async void GetProductsWithCategoryFilter(int searchedCategoryId)
    {
        // Arrange
        GenerateAndAddProductsWithCategoryIds(1, 1, 2, 2, 3, 3, 3);
        testHelper.ConfigureDbContextToReturnItems();
        var searchedCount = products.Count(p => p.CategoryId == searchedCategoryId);

        // Act
        var outProducts = await productService.GetProductsAsync(searchedCategoryId, null, null);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        outProducts.Count().Should().Be(searchedCount);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestData.MinPriceTestData), MemberType = typeof(ProductServiceTestData))]
    public async void GetProductsWithMinPriceFilter(int minPrice)
    {
        // Arrange
        GenerateAndAddProductsWithPrice(ProductServiceTestData.existingPrices);
        testHelper.ConfigureDbContextToReturnItems();
        var searchedCount = products.Count(p => p.Price >= minPrice);

        // Act
        var outProducts = await productService.GetProductsAsync(null, minPrice, null);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        outProducts.Count().Should().Be(searchedCount);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestData.MaxPriceTestData), MemberType = typeof(ProductServiceTestData))]
    public async void GetProductsWithMaxPriceFilter(int maxPrice)
    {
        // Arrange
        GenerateAndAddProductsWithPrice(ProductServiceTestData.existingPrices);
        testHelper.ConfigureDbContextToReturnItems();
        var searchedCount = products.Count(p => p.Price <= maxPrice);

        // Act
        var outProducts = await productService.GetProductsAsync(null, null, maxPrice);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        outProducts.Count().Should().Be(searchedCount);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestData.PriceTestData), MemberType = typeof(ProductServiceTestData))]
    public async void GetProductsWithPriceFilters(int minPrice, int maxPrice)
    {
        // Arrange
        GenerateAndAddProductsWithPrice(ProductServiceTestData.existingPrices);
        testHelper.ConfigureDbContextToReturnItems();
        var searchedCount = products.Count(p => p.Price >= minPrice && p.Price <= maxPrice);

        // Act
        var outProducts = await productService.GetProductsAsync(null, minPrice, maxPrice);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        outProducts.Count().Should().Be(searchedCount);
    }

    [Theory]
    [MemberData(nameof(ProductServiceTestData.MixedTestData), MemberType = typeof(ProductServiceTestData))]
    public async void GetProductsWithMixedFilters(int categoryId, int minPrice, int maxPrice)
    {
        // Arrange
        GenerateAndAddProductsWithCategoryIdAndPrice(ProductServiceTestData.existingCategoryAndPrices);
        testHelper.ConfigureDbContextToReturnItems();
        var searchedCount = products.Count(p => p.CategoryId == categoryId && p.Price >= minPrice && p.Price <= maxPrice);

        // Act
        var outProducts = await productService.GetProductsAsync(categoryId, minPrice, maxPrice);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        outProducts.Count().Should().Be(searchedCount);
    }

    [Fact]
    public async void InsertProduct()
    {
        // Arrange
        var newProduct = GenerateProductInWithCategoryId();
        testHelper.ConfigureDbContextToInsertItem(products);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outProduct = await productService.InsertProductAsync(newProduct);

        // Assert
        testHelper.VerifyInsertIsCalledOnce();
        testHelper.VerifySaveChangesCalledOnce();
        outProduct.Id.Should().NotBe(null);
        outProduct.Should().BeEquivalentTo(newProduct);
    }

    [Fact]
    public async void InsertProductWithoutCategoryId()
    {
        // Arrange
        var newProduct = GenerateProductInWithoutCategoryId();
        testHelper.ConfigureDbContextToInsertItem(products);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outProduct = async () => await productService.InsertProductAsync(newProduct);

        // Assert
        await outProduct.Should().ThrowAsync<FieldIsRequiredException>();
        testHelper.VerifySaveChangesIsNotCalled();
    }

    [Fact]
    public async void InsertProducts()
    {
        // Arrange
        var newProducts = GenerateProductInsWithCategoryIds();
        testHelper.ConfigureDbContextToInsertItem(products);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outProducts = await productService.InsertProductsAsync(newProducts);

        // Assert
        testHelper.VerifySaveChangesCalledOnce();
        outProducts.Count().Should().Be(newProducts.Count);
    }

    [Fact]
    public async void InsertProductsWithoutCategoryId()
    {
        // Arrange
        var newProducts = GenerateProductInsWithoutCategoryIds();
        testHelper.ConfigureDbContextToInsertItem(products);
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        var outProducts = async () => await productService.InsertProductsAsync(newProducts);

        // Assert
        await outProducts.Should().ThrowAsync<FieldIsRequiredException>();
        testHelper.VerifySaveChangesIsNotCalled();
    }

    // Az attach metódust nem lehet mockolni
    // [Fact]
    public async void Update()
    {
        // Arrange
        var idToUpdate = 1;
        var updatedValue = GenerateProductInWithCategoryId();
        GenerateAndAddProductsWithId(idToUpdate);
        testHelper.ConfigureDbSetToBeAbleToAttach();
        testHelper.ConfigureDbContextToReturnItems();
        testHelper.ConfigureDbSetToBeAbleToRemoveItem(products);

        // Act
        await productService.UpdateProductAsync(idToUpdate, updatedValue);

        // Assert
        testHelper.VerifySaveChangesCalledOnce();
        testHelper.VerifyIsItemTheSame(products[0], updatedValue);
    }

    [Fact]
    public async void DeleteExistingProduct()
    {
        // Arrange
        var idToDelete = 1;
        GenerateAndAddProductsWithId(idToDelete);
        testHelper.ConfigureDbContextToReturnItems();
        testHelper.ConfigureDbSetToBeAbleToRemoveItem(products);

        // Act
        await productService.DeleteProductAsync(idToDelete);

        // Assert
        testHelper.VerifySaveChangesCalledOnce();
        products.Count.Should().Be(0);
    }

    [Fact]
    public async void DeleteNonExistingProduct()
    {
        // Arrange
        testHelper.ConfigureDbSetToBeAbleToRemoveItem(products);
        testHelper.ConfigureDbContextToThrowErrorOnSaveChanges();
        testHelper.ConfigureDbContextToReturnItems();

        // Act
        Func<Task> call = async () => await productService.DeleteProductAsync(1);

        // Assert
        await call.Should().ThrowAsync<EntityNotFoundException>();
        testHelper.VerifyRemoveIsCalledOnce();
        testHelper.VerifySaveChangesCalledOnce();
    }

    private void GenerateAndAddProducts(int count)
    {
        products.AddRange(GenerateProducts(count));
    }

    private List<Product> GenerateProducts(int count)
    {
        var ids = Enumerable.Range(1, count).ToArray();
        return GenerateProductsWithId(ids);
    }

    private ProductIn GenerateProductInWithCategoryId()
    {
        return new ProductIn { Name = "", Price = 100, Description = "", CategoryId = 1};
    }

    private ProductIn GenerateProductInWithoutCategoryId()
    {
        return new ProductIn { Name = "", Price = 100, Description = "" };
    }

    private List<ProductIn> GenerateProductInsWithCategoryIds()
    {
        return new List<ProductIn> {
            GenerateProductInWithCategoryId(),
            GenerateProductInWithCategoryId(),
            GenerateProductInWithCategoryId(),
        };
    }

    private List<ProductIn> GenerateProductInsWithoutCategoryIds()
    {
        return new List<ProductIn> {
            GenerateProductInWithoutCategoryId(),
            GenerateProductInWithoutCategoryId(),
            GenerateProductInWithoutCategoryId(),
        };
    }

    private void GenerateAndAddProductsWithId(params int[] ids)
    {
        products.AddRange(GenerateProductsWithId(ids));
    }

    private void GenerateAndAddProductsWithCategoryIds(params int[] categoryIds)
    {
        products.AddRange(GenerateProductsWithCategoryIds(categoryIds));
    }

    private void GenerateAndAddProductsWithPrice(params int[] prices)
    {
        products.AddRange(GenerateProductsWithPrice(prices));
    }

    private void GenerateAndAddProductsWithCategoryIdAndPrice(params Tuple<int, int>[] categoryIdAndPrices)
    {
        products.AddRange(GenerateProductsWithCategoryIdAndPrice(categoryIdAndPrices));
    }

    private List<Product> GenerateProductsWithId(params int[] ids)
    {
        return testHelper.GenerateItemsWithParameters((p, id) => p.Id = id, ids);
    }

    private List<Product> GenerateProductsWithCategoryIds(params int[] categoryIds)
    {
        return testHelper.GenerateItemsWithParameters((p, categoryId) => p.CategoryId = categoryId, categoryIds);
    }

    private List<Product> GenerateProductsWithPrice(params int[] prices)
    {
        return testHelper.GenerateItemsWithParameters((p, price) => p.Price = price, prices);
    }

    private List<Product> GenerateProductsWithCategoryIdAndPrice(params Tuple<int, int>[] categoryIdAndPrices)
    {
        return testHelper.GenerateItemsWithParameters((p, categoryIdAndPrice) =>
            {
                p.CategoryId = categoryIdAndPrice.Item1;
                p.Price = categoryIdAndPrice.Item2;
            },
            categoryIdAndPrices
        );
    }
}
