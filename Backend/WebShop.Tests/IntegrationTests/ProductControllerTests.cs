using Bogus;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebShop.Bll.Dtos;
using WebShop.Dal.Entities;

namespace WebShop.Tests;

public partial class ProductControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly CustomWebApplicationFactory _appFactory;
    protected readonly Faker<ProductIn> _dtoFaker;
    protected readonly JsonSerializerOptions _serializerOptions;

    public ProductControllerTests(CustomWebApplicationFactory appFactory)
    {
        _appFactory = appFactory;
        _dtoFaker = new Faker<ProductIn>()
            .RuleFor(p => p.Name, f => f.Commerce.Product())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Price, f => f.Random.Int(200, 2000));

        _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _serializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
}
