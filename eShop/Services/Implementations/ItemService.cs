using eShop.Data;
using eShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace eShop.Services;

public class ItemService : IItemService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ItemService> _logger;
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public ItemService(ApplicationDbContext context, ILogger<ItemService> logger, IMemoryCache cache)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<IEnumerable<ShopItem>> GetAllItems()
    {
        const string cacheKey = "all_shop_items";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<ShopItem> cachedItems))
        {
            _logger.LogInformation("Returning cached shop items.");
            return cachedItems;
        }

        try
        {
            _logger.LogInformation("Fetching all items from the database.");
            var items = await _context.ShopItems.AsNoTracking().ToListAsync();


            _cache.Set(cacheKey, items, _cacheDuration);

            return items;
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
            
            _cache.Remove("all_shop_items");

            _logger.LogInformation("Successfully added item: {@Item}", item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while adding item: {@Item}", item);
            throw;
        }
    }
}
