using WebShop.Bll.Dtos;

namespace WebShop.Bll.Interfaces;

public interface ICategoryService
{
    public Task<int> GetCategoryCountAsync();
    public Task<IEnumerable<Category>> GetCategoriesAsync();
    public Task<Category> GetCategoryAsync(int id);
    public Task<Category> InsertCategoryAsync(Category category);
    public Task<Category> UpdateCategoryAsync(int id, Category category);
    public Task DeleteCategoryAsync(int id);
}
