using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Transactions;
using WebShop.Bll.Dtos;

namespace WebShop.Tests.IntegrationTests;

public partial class UserTests
{
    public class Post : UserControllerTests
    {
        public Post(CustomWebApplicationFactory appFactory)
            : base(appFactory)
        {
        }

        [Fact]
        async Task LoginAsAdminUser()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            var client = _appFactory.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("api/user/login", _adminUser);
            var user = await loginResponse.Content.ReadFromJsonAsync<UserOut>();


            // Assert
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            user.Id.Should().Be(1);
            user.IsAdmin.Should().BeTrue();
            user.Name.Should().Be(_adminUser.Name);
        }

        [Fact]
        async Task LoginAsNormalUser()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var client = _appFactory.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("api/user/login", _normalUser);
            var user = await loginResponse.Content.ReadFromJsonAsync<UserOut>();


            // Assert
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            user.Id.Should().Be(2);
            user.IsAdmin.Should().BeFalse();
            user.Name.Should().Be(_normalUser.Name);
        }

        [Fact]
        async Task LoginWithIncorrectLoginDetails()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            var client = _appFactory.CreateClient();

            // Act
            var loginResponse1 = await client.PostAsJsonAsync("api/user/login", new UserIn());
            var loginResponse2 = await client.PostAsJsonAsync("api/user/login", new UserIn { Email = "asd", Password = "asdasdasd" });
            var loginResponse3 = await client.PostAsJsonAsync("api/user/login", new UserIn { Email = "asd@gmail.com", Password = "asd" });
            var loginResponse4 = await client.PostAsJsonAsync("api/user/login", new UserIn { Email = "asd@gmail.com", Password = new string('a', 100) });
            var loginResponse5 = await client.PostAsJsonAsync("api/user/login", new UserIn { Email = "asd@gmail.com", Password = "asdasdasd" });
            var problemDetail1 = await loginResponse1.Content.ReadFromJsonAsync<ProblemDetails>();
            var problemDetail2 = await loginResponse2.Content.ReadFromJsonAsync<ProblemDetails>();
            var problemDetail3 = await loginResponse3.Content.ReadFromJsonAsync<ProblemDetails>();
            var problemDetail4 = await loginResponse4.Content.ReadFromJsonAsync<ProblemDetails>();
            var problemDetail5 = await loginResponse5.Content.ReadFromJsonAsync<ProblemDetails>();

            // Assert
            loginResponse1.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            loginResponse2.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            loginResponse3.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            loginResponse4.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            loginResponse5.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        async Task Logout()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;

            var client = _appFactory.CreateClient();

            // Act
            var loginResponse = await client.PostAsJsonAsync("api/user/login", _normalUser);
            var user = await loginResponse.Content.ReadFromJsonAsync<UserOut>();
            var logoutResponse = await client.PostAsync("api/user/logout", null);


            // Assert
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
