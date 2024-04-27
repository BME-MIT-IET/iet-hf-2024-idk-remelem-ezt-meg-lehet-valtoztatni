using FluentAssertions;
using System.Net.Http.Json;
using System.Transactions;
using WebShop.Bll.Dtos;
using WebShop.Dal.Entities;

namespace WebShop.Tests;

public partial class ProductIntegrationTests
{
    public class Post : ProductControllerTests
    {
        public Post(CustomWebApplicationFactory appFactory)
            : base(appFactory)
        {
        }
        
        [Fact]
        async Task ShouldSuccedWithCreated()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var client = _appFactory.CreateClient();
            //var dto = _dtoFaker.Generate();
            var productsIn = new List<ProductIn> {
                new ProductIn() {
                    Name = "Prod1",
                    Price = 100,
                    Description = "Desc",
                    CategoryId = 1,
                },
                new ProductIn() {
                    Name = "Prod2",
                    Price = 200,
                    CategoryId = 2,
                }
            };
            var adminUser = new UserIn() {
                Email = "admin@admin.com",
                Password = "admin"
            };

            // Act
            var loginResponse = await client.PostAsJsonAsync("api/user/login", adminUser);
            var makeProductResponse = await client.PostAsJsonAsync("/api/product", productsIn, _serializerOptions);
            var productsOut = await makeProductResponse.Content.ReadFromJsonAsync<IEnumerable<ProductOut>>(_serializerOptions);

            // Assert
            makeProductResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            productsOut.Should().HaveCount(2);
            productsOut.Should().BeEquivalentTo(productsIn);
            productsOut.ElementAt(0).Category.Should().NotBeNull();
            productsOut.ElementAt(0).Category.Id.Should().Be(productsIn[0].CategoryId);
            productsOut.ElementAt(1).Category.Should().NotBeNull();
            productsOut.ElementAt(1).Category.Id.Should().Be(productsIn[1].CategoryId);
        }
    }
}
