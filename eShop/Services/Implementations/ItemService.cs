using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using eShop.Data;
using eShop.Models.Domain;
using eShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services.Implementations;

public class ItemService(
    ApplicationDbContext context,
    ILogger<ItemService> logger,
    IDistributedCache cache)
    : IItemService
{
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<ItemService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<IEnumerable<ShopItem>> GetAllItems()
    {
        const string cacheKey = "all_shop_items";
        
        var cachedItems = await _cache.GetStringAsync(cacheKey);
        if (cachedItems is not null)
        {
            _logger.LogInformation("Returning cached shop items.");
            return JsonSerializer.Deserialize<IEnumerable<ShopItem>>(cachedItems)!;
        }

        _logger.LogInformation("Fetching all items from the database.");
        var items = await _context.ShopItems.AsNoTracking().ToListAsync();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(items),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            });

        return items;
    }

    public async Task AddItem(ShopItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        _logger.LogInformation("Adding a new item: {@Item}", item);
        await _context.ShopItems.AddAsync(item);
        await _context.SaveChangesAsync();
        
        await _cache.RemoveAsync("all_shop_items");

        _logger.LogInformation("Successfully added item: {@Item}", item);
    }
}