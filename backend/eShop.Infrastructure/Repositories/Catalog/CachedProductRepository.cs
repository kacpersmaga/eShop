using System.Text.Json;
using eShop.Infrastructure.Repositories.Caching;
using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Filtering;
using eShop.Modules.Catalog.Infrastructure.Persistence;
using eShop.Shared.Abstractions.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Repositories.Catalog;

public class CachedProductRepository : CachedSpecificationRepository<Product>, IProductRepository, ICacheInvalidator
{
    private readonly CatalogDbContext _catalogContext;
    private readonly string _cacheKeyPrefix = "Product_";

    public CachedProductRepository(
        CatalogDbContext context, 
        ILogger<CachedProductRepository> logger,
        IDistributedCache cache) 
        : base(context, logger, cache, TimeSpan.FromMinutes(30))
    {
        _catalogContext = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        var cacheKey = $"{_cacheKeyPrefix}GetAll";
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Cache hit for GetAllAsync");
            return JsonSerializer.Deserialize<List<Product>>(cachedData) ?? new List<Product>();
        }

        _logger.LogInformation("Cache miss for GetAllAsync");
        var products = await _catalogContext.Products.AsNoTracking().ToListAsync();
        
        if (products.Any())
        {
            await CacheDataAsync(cacheKey, products, TimeSpan.FromMinutes(30));
        }
        
        return products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var cacheKey = $"{_cacheKeyPrefix}Id_{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Cache hit for GetByIdAsync: {ProductId}", id);
            return JsonSerializer.Deserialize<Product>(cachedData);
        }

        _logger.LogInformation("Cache miss for GetByIdAsync: {ProductId}", id);
        var product = await _catalogContext.Products.FindAsync(id);
        
        if (product != null)
        {
            await CacheDataAsync(cacheKey, product, TimeSpan.FromMinutes(30));
        }
        
        return product;
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        var spec = new ProductByCategorySpecification(category, true);
        var cacheKey = $"{_cacheKeyPrefix}Category_{category}";
        
        return await ListWithCacheAsync(spec, cacheKey);
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        var spec = new ProductByNameSpecification(name);
        var cacheKey = $"{_cacheKeyPrefix}Name_{name}";
        
        return await GetBySpecWithCacheAsync(spec, cacheKey);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _catalogContext.Products.AnyAsync(p => p.Id == id);
    }
    
    public async Task AddAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        
        _logger.LogInformation("Adding a new product: {Name}", product.Name.Value);
        await _catalogContext.Products.AddAsync(product);
    }
    
    public Task UpdateAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        
        _logger.LogInformation("Updating product with ID {ProductId}: {Name}", product.Id, product.Name.Value);
        _catalogContext.Entry(product).State = EntityState.Modified;
        
        return Task.CompletedTask;
    }
    
    public Task DeleteAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        
        _logger.LogInformation("Deleting product with ID {ProductId}", product.Id);
        _catalogContext.Products.Remove(product);
        
        return Task.CompletedTask;
    }
    
    public async Task InvalidateCacheAsync()
    {
        _logger.LogInformation("Invalidating product cache after database changes");
        await InvalidateCacheAsync($"{_cacheKeyPrefix}*");
    }
}