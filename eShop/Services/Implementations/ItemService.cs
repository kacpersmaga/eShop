using eShop.Data;
using eShop.Models;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services;

public class ItemService : IItemService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IApplicationDbContext context, ILogger<ItemService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ShopItem>> GetAllItems()
    {
        try
        {
            _logger.LogInformation("Fetching all items from the database.");
            return await _context.ShopItems.AsNoTracking().ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching items.");
            throw;
        }
    }

    public async Task AddItem(ShopItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        try
        {
            _logger.LogInformation("Adding a new item: {@Item}", item);
            await _context.ShopItems.AddAsync(item);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added item: {@Item}", item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding item: {@Item}", item);
            throw;
        }
    }
}