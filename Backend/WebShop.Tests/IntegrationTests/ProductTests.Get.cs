using FluentAssertions;
using System.Net.Http.Json;
using System.Transactions;
using WebShop.Bll.Dtos;

namespace WebShop.Tests.IntegrationTests;

public partial class ProductTests
{
    public class Get : ProductControllerTests
    {
        public Get(CustomWebApplicationFactory appFactory)
            : base(appFactory)
        {}

        [Fact]
        async Task GetSeedProducts()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var client = _appFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/product");
            var p = await response.Content.ReadFromJsonAsync<IEnumerable<ProductOut>>(_serializerOptions);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            p.Count().Should().Be(11);
        }

        [Fact]
        async Task GetExistingProduct()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var client = _appFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/product/1");
            var p = await response.Content.ReadFromJsonAsync<ProductOut>(_serializerOptions);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            p.Id.Should().Be(1);
            p.Name.Should().Be("Alma");
            p.Price.Should().Be(250);
            p.Description.Should().Be("Az orvost távol tartja");
            p.CategoryId.Should().Be(1);
            p.Category.Id.Should().Be(1);
            p.Category.Name.Should().Be("Élelmiszer");
        }

        [Fact]
        async Task GetNonExistingProduct()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var client = _appFactory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/product/1000000");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        async Task GetProductsWithCategoryFilter()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var client = _appFactory.CreateClient();
            var categoryId = 1;

            // Act
            var response = await client.GetAsync($"/api/product?categoryId={1}");
            var products = await response.Content.ReadFromJsonAsync<List<ProductOut>>(_serializerOptions);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            foreach (var product in products)
            {
                product.CategoryId.Should().Be(categoryId);
            }
        }

        [Fact]
        async Task GetProductsWithMinPrice()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var client = _appFactory.CreateClient();
            var minPrice = 10000;

            // Act
            var response = await client.GetAsync($"/api/product?minPrice={minPrice}");
            var products = await response.Content.ReadFromJsonAsync<List<ProductOut>>(_serializerOptions);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            foreach (var product in products)
            {
                product.Price.Should().BeGreaterThanOrEqualTo(minPrice);
            }
        }

        [Fact]
        async Task GetProductsWithMaxPrice()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var client = _appFactory.CreateClient();
            var maxPrice = 10000;

            // Act
            var response = await client.GetAsync($"/api/product?maxPrice={maxPrice}");
            var products = await response.Content.ReadFromJsonAsync<List<ProductOut>>(_serializerOptions);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            foreach (var product in products)
            {
                product.Price.Should().BeLessThanOrEqualTo(maxPrice);
            }
        }

        [Fact]
        async Task GetProductsWithMinPriceAndMaxPrice()
        {
            // Arrange
            _appFactory.Server.PreserveExecutionContext = true;
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var client = _appFactory.CreateClient();
            var minPrice = 10000;
            var maxPrice = 50000;

            // Act
            var response = await client.GetAsync($"/api/product?minPrice={minPrice}&maxPrice={maxPrice}");
            var products = await response.Content.ReadFromJsonAsync<List<ProductOut>>(_serializerOptions);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            foreach (var product in products)
            {
                product.Price.Should().BeGreaterThanOrEqualTo(minPrice);
                product.Price.Should().BeLessThanOrEqualTo(maxPrice);
            }
        }
    }
}
