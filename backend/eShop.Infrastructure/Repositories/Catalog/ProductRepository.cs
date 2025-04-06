using eShop.Infrastructure.Repositories.Base;
using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Modules.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Repositories.Catalog;

public class ProductRepository : SpecificationRepository<Product>, IProductRepository
{
    private readonly CatalogDbContext _catalogContext;

    public ProductRepository(CatalogDbContext context, ILogger<ProductRepository> logger)
        : base(context, logger)
    {
        _catalogContext = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        Logger.LogInformation("Fetching all products from the database");
        return await _catalogContext.Products.AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        Logger.LogInformation("Fetching product with ID {ProductId} from the database", id);
        return await _catalogContext.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        Logger.LogInformation("Fetching products in category '{Category}' from the database", category);
        return await _catalogContext.Products
            .Where(p => p.Category.Value == category && p.IsAvailable)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        Logger.LogInformation("Fetching product with name '{ProductName}'", name);
        return await _catalogContext.Products
            .FirstOrDefaultAsync(p => p.Name.Value == name);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _catalogContext.Products.AnyAsync(p => p.Id == id);
    }
    

    public async Task AddAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        
        Logger.LogInformation("Adding a new product: {Name}", product.Name.Value);
        await _catalogContext.Products.AddAsync(product);
    }

    public Task UpdateAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        
        Logger.LogInformation("Updating product with ID {ProductId}: {Name}", product.Id, product.Name.Value);
        
        _catalogContext.Entry(product).State = EntityState.Modified;
        
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        
        Logger.LogInformation("Deleting product with ID {ProductId}", product.Id);
        _catalogContext.Products.Remove(product);
        
        return Task.CompletedTask;
    }
}