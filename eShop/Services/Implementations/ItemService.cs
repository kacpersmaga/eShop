using eShop.Data;
using eShop.Models.Domain;
using eShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace eShop.Services.Implementations;

public class ItemService(ApplicationDbContext context, ILogger<ItemService> logger, IMemoryCache cache)
    : IItemService
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<ItemService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<IEnumerable<ShopItem>> GetAllItems()
    {
        const string cacheKey = "all_shop_items";
        
        if (_cache.TryGetValue(cacheKey, out IEnumerable<ShopItem>? cachedItems) && cachedItems != null)
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
