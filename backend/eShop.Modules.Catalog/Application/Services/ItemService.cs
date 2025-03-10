using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using eShop.Models.Domain;
using eShop.Modules.Catalog.Domain.Entities;
using eShop.Modules.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ItemService> _logger;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public ItemService(
        IItemRepository itemRepository,
        ILogger<ItemService> logger,
        IDistributedCache cache)
    {
        _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<IEnumerable<ShopItem>> GetAllItems()
    {
        try
        {
            const string cacheKey = "all_shop_items";

            var cachedItems = await _cache.GetStringAsync(cacheKey);
            if (cachedItems is not null)
            {
                _logger.LogInformation("Returning cached shop items");
                return JsonSerializer.Deserialize<IEnumerable<ShopItem>>(cachedItems)!;
            }

            var items = await _itemRepository.GetAllAsync();

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(items),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                });

            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all shop items from DB or cache");
            throw;
        }
    }

    public async Task AddItem(ShopItem item)
    {
        try
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync();
            
            await _cache.RemoveAsync("all_shop_items");

            _logger.LogInformation("Successfully added item: {@Item}", item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding new item {@Item}", item);
            throw;
        }
    }
}