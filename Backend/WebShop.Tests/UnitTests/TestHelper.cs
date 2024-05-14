using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using WebShop.Bll.Dtos;
using WebShop.Dal;
using WebShop.Dal.Entities;

namespace WebShop.Tests.UnitTests;

public class TestHelper<T> where T : class
{
    private Mock<DbSet<T>> mockDbSet;
    private Mock<AppDbContext> mockDbContext;
    private Faker<T>? faker;
    public Expression<Func<AppDbContext, DbSet<T>>> SetupFunc { get; set; }
    public Func<T, T, bool> RemoveFunc { get; set; }

    public TestHelper(Mock<AppDbContext> mockDbContext, Mock<DbSet<T>> mockDbSet, Faker<T>? faker = null)
    {
        this.mockDbContext = mockDbContext;
        this.mockDbSet = mockDbSet;
        this.faker = faker;
    }

    public void VerifyInsertIsCalledOnce()
    {
        mockDbSet.Verify(set => set.Add(It.IsAny<T>()), Times.Never());
        mockDbSet.Verify(set => set.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    public void VerifyRemoveIsCalledOnce()
    {
        mockDbSet.Verify(set => set.Remove(It.IsAny<T>()), Times.Once());
    }

    public void VerifySaveChangesIsNotCalled()
    {
        mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
    }

    public void VerifySaveChangesCalledOnce()
    {
        mockDbContext.Verify(ctx => ctx.SaveChanges(), Times.Never());
        mockDbContext.Verify(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    public void VerifyIsItemTheSame<TReturned>(T existingItem, TReturned returnedItem)
    {
        existingItem.Should().BeEquivalentTo(
            returnedItem,
            opt => opt.ExcludingMissingMembers()
        );
    }

    public void VerifyAllItemsAreTheSame<TReturned>(IEnumerable<T> existingItems, IEnumerable<TReturned> returnedItems)
    {
        existingItems.Should().BeEquivalentTo(
            returnedItems,
            opt => opt.ExcludingMissingMembers()
        );
    }

    public List<T> GenerateItemsWithParameters<TParam>(Action<T, TParam> apply, params TParam[] paramters)
    {
        if (faker == null)
        {
            throw new Exception("Faker must be initialized to use this method");
        }

        return paramters.Select(parameter =>
        {
            var item = faker.Generate();
            apply(item, parameter);
            return item;
        }).ToList();
    }

    public void ConfigureDbContextToInsertItem(List<T> container)
    {
        mockDbSet
            .Setup(set => set.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Callback((T entity, CancellationToken _) => container.Add(entity));
    }

    public void ConfigureDbContextToReturnItems()
    {
        mockDbContext
            .Setup(SetupFunc)
            .Returns(mockDbSet.Object);
    }

    public void ConfigureDbContextToThrowErrorOnSaveChanges()
    {
        mockDbContext
            .Setup(ctx => ctx.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new DbUpdateConcurrencyException());
    }

    public void ConfigureDbSetToBeAbleToRemoveItem(List<T> container)
    {
        mockDbSet
            .Setup(set => set.Remove(It.IsAny<T>()))
            .Callback((T entity) => container.RemoveAll(p => RemoveFunc(p, entity)));
    }

    // Nem lehet mockolni
    public void ConfigureDbSetToBeAbleToAttach()
    {
        var productEntityEntry = new Mock<EntityEntry<T>>();

        mockDbSet
            .Setup(set => set.Attach(It.IsAny<T>()))
            .Returns(productEntityEntry.Object);
    }
}
