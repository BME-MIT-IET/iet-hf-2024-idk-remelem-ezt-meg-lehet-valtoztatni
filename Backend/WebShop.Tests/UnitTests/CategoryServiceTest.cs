using AutoMapper;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;
using WebShop.Dal;
using WebShop.Bll.Services;
using FluentAssertions;
using WebShop.Bll.Exceptions;
using WebShop.Dal.Entities;

namespace WebShop.Tests.UnitTests;

public class CategoryServiceTest
{
    private List<Category> categories;
    private Faker<Category> categoryFaker;
    private Mock<AppDbContext> mockDbContext;
    private Mock<DbSet<Category>> mockDbSet;
    private TestHelper<Category> testHelper;
    private CategoryService categoryService;
    private IMapper mapper;

    public CategoryServiceTest()
    {
        categories = new List<Category>();
        mockDbSet = categories.AsQueryable().BuildMockDbSet();
        mockDbContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());

        categoryFaker = new Faker<Category>()
            .CustomInstantiator(f =>
                new Category(f.Commerce.ProductAdjective())
            );

        var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Bll.Dtos.WebApiProfile());
            }
        );

        testHelper = new TestHelper<Category>(mockDbContext, mockDbSet, categoryFaker);

        mapper = new Mapper(mapperConfiguration);
        categoryService = new CategoryService(mockDbContext.Object, mapper);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public async void GetProductCount(int count)
    {
        // Arrange
        categories.AddRange(GenerateCategories(count));
        ConfigureDbContextToReturnCategories();

        // Act
        var resultCount = await categoryService.GetCategoryCountAsync();

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        resultCount.Should().Be(count);
    }

    [Fact]
    public async void GetCategories()
    {
        // Arrange
        GenerateAndAddCategoriesWithId(1, 2, 3, 4, 5);
        ConfigureDbContextToReturnCategories();

        // Act
        var outProducts = await categoryService.GetCategoriesAsync();

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        testHelper.VerifyAllItemsAreTheSame(categories, outProducts);
    }

    [Fact]
    public async void GetExistingCategory()
    {
        // Arrange
        var categoryId = 3;
        GenerateAndAddCategoriesWithId(1, 2, categoryId, 4, 5);
        var category = categories[categoryId - 1];
        ConfigureDbContextToReturnCategories();

        // Act
        var outProducts = await categoryService.GetCategoryAsync(categoryId);

        // Assert
        testHelper.VerifySaveChangesIsNotCalled();
        testHelper.VerifyIsItemTheSame(category, outProducts);
    }

    [Fact]
    public async void GetNonExistingCategory()
    {
        // Arrange
        ConfigureDbContextToReturnCategories();

        // Act
        var outCategory = async () => await categoryService.GetCategoryAsync(1);

        // Assert
        await outCategory.Should().ThrowAsync<EntityNotFoundException>();
        testHelper.VerifySaveChangesIsNotCalled();
    }

    [Fact]
    public async void InsertCategory()
    {
        // Arrange
        var categoryToInsert = new Bll.Dtos.Category() { Id = 1, Name = "Inserted"};
        ConfigureDbContextToReturnCategories();
        ConfigureDbContextToInsertCategory();

        // Act
        var outCategory = await categoryService.InsertCategoryAsync(categoryToInsert);

        // Assert
        testHelper.VerifyInsertIsCalledOnce();
        testHelper.VerifySaveChangesCalledOnce();
        outCategory.Name.Should().Be(categoryToInsert.Name);
    }

    [Fact]
    public async void DeleteExistingCategory()
    {
        // Arrange
        var idToDelete = 1;
        GenerateAndAddCategoriesWithId(idToDelete);
        ConfigureDbContextToReturnCategories();
        ConfigureDbSetToBeAbleToRemoveCategory();

        // Act
        await categoryService.DeleteCategoryAsync(idToDelete);

        // Assert
        testHelper.VerifySaveChangesCalledOnce();
        categories.Count.Should().Be(0);
    }

    [Fact]
    public async void DeleteNonExistingCategory()
    {
        // Arrange
        ConfigureDbSetToBeAbleToRemoveCategory();
        ConfigureDbContextToThrowErrorOnSaveChanges();
        ConfigureDbContextToReturnCategories();

        // Act
        Func<Task> call = async () => await categoryService.DeleteCategoryAsync(1);

        // Assert
        await call.Should().ThrowAsync<EntityNotFoundException>();
        testHelper.VerifyRemoveIsCalledOnce();
        testHelper.VerifySaveChangesCalledOnce();
    }

    private void ConfigureDbContextToReturnCategories()
    {
        mockDbContext
            .Setup(ctx => ctx.Categories)
            .Returns(mockDbSet.Object);
    }

    private void ConfigureDbSetToBeAbleToRemoveCategory()
    {
        mockDbSet
            .Setup(set => set.Remove(It.IsAny<Category>()))
            .Callback((Category entity) => categories.RemoveAll(c => c.Id == entity.Id));
    }

    private void ConfigureDbContextToThrowErrorOnSaveChanges()
    {
        mockDbContext
            .Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new DbUpdateConcurrencyException());
    }

    private void ConfigureDbContextToInsertCategory()
    {
        mockDbSet
            .Setup(set => set.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Callback((Category entity, CancellationToken _) => categories.Add(entity));
    }

    private void GenerateAndAddCategories(int count)
    {
        categories.AddRange(GenerateCategories(count));
    }

    private void GenerateAndAddCategoriesWithId(params int[] ids)
    {
        categories.AddRange(GenerateCategoriesWithId(ids));
    }

    private List<Category> GenerateCategories(int count)
    {
        var ids = Enumerable.Range(1, count).ToArray();
        return GenerateCategoriesWithId(ids);
    }

    private List<Category> GenerateCategoriesWithId(params int[] ids)
    {
        return testHelper.GenerateItemsWithParameters((p, id) => p.Id = id, ids);
    }
}
