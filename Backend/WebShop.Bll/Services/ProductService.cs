using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebShop.Bll.Dtos;
using WebShop.Bll.Exceptions;
using WebShop.Bll.Interfaces;
using WebShop.Dal;

namespace WebShop.Bll.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> GetProductCountAsync()
    {
        var productCount = await _context
            .Products
            .CountAsync();
        return productCount;
    }

    public async Task<ProductOut> GetProductAsync(int productId)
    {
        return await _context
            .Products
            .ProjectTo<ProductOut>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(p => p.Id == productId)
            ?? throw new EntityNotFoundException("Nem található a termék");
    }

    public async Task<IEnumerable<ProductOut>> GetProductsAsync(int? categoryId, int? minPrice, int? maxPrice)
    {
        IQueryable<Dal.Entities.Product> products = _context.Products;

        if (categoryId != null)
        {
            products = products.Where(products => products.CategoryId == categoryId);
        }

        if (minPrice != null)
        {
            products = products.Where(products => products.Price >= minPrice);
        }

        if (maxPrice != null)
        {
            products = products.Where(products => products.Price <= maxPrice);
        }

        return await products
            .ProjectTo<ProductOut>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    private async Task<IEnumerable<ProductOut>> GetProductsAsync(IEnumerable<int> ids)
    {
       var products = await _context
            .Products
            .ProjectTo<ProductOut>(_mapper.ConfigurationProvider)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
        return products;
    }

    public async Task<ProductOut> InsertProductAsync(ProductIn newProduct)
    {
        var efProduct = _mapper.Map<Dal.Entities.Product>(newProduct);
        await _context.Products.AddAsync(efProduct);
        await _context.SaveChangesAsync();
        return await GetProductAsync(efProduct.Id);
    }

    public async Task<IEnumerable<ProductOut>> InsertProductsAsync(IEnumerable<ProductIn> newProducts)
    {
        if (newProducts.Any(p => p.CategoryId == null)) {
            throw new FieldIsRequiredException("A categoryId megadása kötelező");
        }

        var efProducts = newProducts.Select(p => _mapper.Map<Dal.Entities.Product>(p)).ToList();
        efProducts.ForEach(p => _context.Products.Add(p));
        await _context.SaveChangesAsync();
        return await GetProductsAsync(efProducts.Select(p => p.Id));
    }

    public async Task UpdateProductAsync(int productId, ProductIn updatedProduct)
    {
        var efProduct = _mapper.Map<Dal.Entities.Product>(updatedProduct);
        efProduct.Id = productId;
        var entry = _context.Attach(efProduct);
        entry.State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Products.AnyAsync(p => p.Id == productId))
                throw new EntityNotFoundException("Nem található a termék");
            else
                throw;
        }
    }

    public async Task DeleteProductAsync(int productId)
    {
        _context.Products.Remove(new Dal.Entities.Product(null!, 0) { Id = productId });
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EntityNotFoundException("Nem található a termék");
        }
    }
}
