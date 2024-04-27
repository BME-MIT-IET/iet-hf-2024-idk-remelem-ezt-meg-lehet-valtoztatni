using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using WebShop.Bll.Dtos;
using WebShop.Bll.Exceptions;
using WebShop.Bll.Interfaces;

namespace WebShop.Bll.Services;

public class CategoryService : ICategoryService
{
    private readonly Dal.AppDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(Dal.AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> GetCategoryCountAsync()
    {
        var categoryCount = await _context
            .Categories
            .CountAsync();
        return categoryCount;
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        var categories = await _context
            .Categories
            .ProjectTo<Category>(_mapper.ConfigurationProvider)
            .ToListAsync();
        return categories;
    }

    public async Task<Category> GetCategoryAsync(int id)
    {
        var category = await _context
            .Categories
            .ProjectTo<Category>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(c => c.Id == id)
            ?? throw new EntityNotFoundException("Nem található a kategória");
        return category;
    }
    public async Task<Category> InsertCategoryAsync(Category category)
    {
        var efCategory = new Dal.Entities.Category(category.Name);
        _context.Categories.Add(efCategory);
        await _context.SaveChangesAsync();
        return await GetCategoryAsync(efCategory.Id);
    }

    public async Task<Category> UpdateCategoryAsync(int id, Category category)
    {
        var efProduct = _mapper.Map<Dal.Entities.Category>(category);
        efProduct.Id = id;
        var entry = _context.Attach(efProduct);
        entry.State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return await GetCategoryAsync(id);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        _context.Categories.Remove(new Dal.Entities.Category(null!) { Id = id });
        await _context.SaveChangesAsync();
    }
}
