using FluentAssertions;
using System.Net.Http.Json;
using System.Transactions;
using WebShop.Bll.Dtos;

namespace WebShop.Tests.IntegrationTests;

public partial class ProductTests
{
    public class Del : ProductControllerTests
    {
        public Del(CustomWebApplicationFactory appFactory)
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
            var adminUser = new UserIn() {
                Email = "admin@admin.com",
                Password = "ADMIN_admin"
            };

            // Act
            var getResponseBefore = await client.GetAsync("/api/product/1");
            var loginResponse = await client.PostAsJsonAsync("api/user/login", adminUser);
            var delResponse = await client.DeleteAsync("/api/product/1");
            var getResponseAfter = await client.GetAsync("/api/product/1");

            // Assert
            getResponseBefore.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            delResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Accepted);
            getResponseAfter.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
