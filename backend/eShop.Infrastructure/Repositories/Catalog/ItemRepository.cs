using eShop.Infrastructure.Data;
using eShop.Models.Domain;
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

    public async Task AddAsync(ShopItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        
        _logger.LogInformation("Adding a new item: {@Item}", item);
        await _context.ShopItems.AddAsync(item);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
        _logger.LogInformation("Changes saved to database");
    }
}