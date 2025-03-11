using eShop.Infrastructure.Data;
using eShop.Modules.Catalog.Domain.Entities;
using eShop.Modules.Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Repositories.Catalog;

public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ItemRepository> _logger;

    public ItemRepository(ApplicationDbContext context, ILogger<ItemRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ShopItem>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all items from the database");
        return await _context.ShopItems.AsNoTracking().ToListAsync();
    }

    public async Task<ShopItem?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching item with ID {ItemId} from the database", id);
        return await _context.ShopItems.FindAsync(id);
    }

    public async Task AddAsync(ShopItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        
        _logger.LogInformation("Adding a new item: {@Item}", item);
        await _context.ShopItems.AddAsync(item);
    }

    public async Task UpdateAsync(ShopItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        
        _logger.LogInformation("Updating item with ID {ItemId}: {@Item}", item.Id, item);
        
        _context.Entry(item).State = EntityState.Modified;
        item.UpdatedAt = DateTime.UtcNow;
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting item with ID {ItemId}", id);
        
        var item = await _context.ShopItems.FindAsync(id);
        if (item != null)
        {
            _context.ShopItems.Remove(item);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
        _logger.LogInformation("Changes saved to database");
    }
}