using WebShop.Bll.Dtos;

namespace WebShop.Bll.Interfaces;

public interface IProductService
{
    public Task<int> GetProductCountAsync();
    public Task<ProductOut> GetProductAsync(int productId);
    public Task<IEnumerable<ProductOut>> GetProductsAsync(int? categoryId, int? minPrice, int? maxPrice);
    public Task<ProductOut> InsertProductAsync(ProductIn newProduct);
    public Task<IEnumerable<ProductOut>> InsertProductsAsync(IEnumerable<ProductIn> newProducts);
    public Task UpdateProductAsync(int productId, ProductIn updatedProduct);
    public Task DeleteProductAsync(int productId);
}
