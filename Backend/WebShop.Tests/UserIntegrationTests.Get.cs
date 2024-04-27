using System.Net.Http.Json;
using System.Net;
using System.Transactions;
using WebShop.Bll.Dtos;
using FluentAssertions;

namespace WebShop.Tests;

public partial class UserIntegrationTests
{
    public class Get : UserControllerTests
    {
        public Get(CustomWebApplicationFactory appFactory)
            : base(appFactory)
        {
        }

        [Fact]
        async Task GetUserExistingUserAsAdmin()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var client = _appFactory.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("api/user/login", _adminUser);
            var adminUser = await loginResponse.Content.ReadFromJsonAsync<UserOut>();
            var getResponse = await client.GetAsync("api/user/1");
            var getUser = await getResponse.Content.ReadFromJsonAsync<UserOut>();


            // Assert
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            adminUser.Should().BeEquivalentTo(getUser);
        }

        [Fact]
        async Task GetUserNonExistingUserAsAdmin()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var client = _appFactory.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("api/user/login", _adminUser);
            var getResponse = await client.GetAsync("api/user/100000");

            // Assert
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        async Task GetUserAsNormalUser()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var client = _appFactory.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("api/user/login", _normalUser);
            var getResponse = await client.GetAsync("api/user/1");


            // Assert
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
