using Bogus;
using System.Text.Json.Serialization;
using System.Text.Json;
using WebShop.Bll.Dtos;

namespace WebShop.Tests.IntegrationTests;

public partial class UserControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly CustomWebApplicationFactory _appFactory;
    protected readonly JsonSerializerOptions _serializerOptions;
    protected readonly UserIn _adminUser = new UserIn() { Name = "admin", Email = "admin@admin.com", Password = "ADMIN_admin" };
    protected readonly UserIn _normalUser = new UserIn() { Name = "user", Email = "user@user.com", Password = "USER_user" };

    public UserControllerTests(CustomWebApplicationFactory appFactory)
    {
        _appFactory = appFactory;

        _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
}
