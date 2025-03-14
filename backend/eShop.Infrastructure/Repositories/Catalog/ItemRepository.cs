using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Modules.Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Repositories.Catalog;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all products from the database");
        return await _context.Products.AsNoTracking().ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching product with ID {ProductId} from the database", id);
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        _logger.LogInformation("Fetching products in category '{Category}' from the database", category);
        return await _context.Products
            .Where(p => p.Category.Value == category && p.IsAvailable)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        
        _logger.LogInformation("Adding a new product: {Name}", product.Name.Value);
        await _context.Products.AddAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        
        _logger.LogInformation("Updating product with ID {ProductId}: {Name}", product.Id, product.Name.Value);
        
        _context.Entry(product).State = EntityState.Modified;
    }

    public async Task DeleteAsync(Product product)
    {
        if (product == null) throw new ArgumentNullException(nameof(product));
        
        _logger.LogInformation("Deleting product with ID {ProductId}", product.Id);
        _context.Products.Remove(product);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }
}