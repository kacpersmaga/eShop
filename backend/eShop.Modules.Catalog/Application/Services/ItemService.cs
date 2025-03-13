using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Entities;
using eShop.Modules.Catalog.Domain.Repositories;
using Microsoft.Extensions.Logging;
using eShop.Shared.Common;

namespace eShop.Modules.Catalog.Application.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ItemService> _logger;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
    private const string AllItemsCacheKey = "all_shop_items";

    public ItemService(
        IItemRepository itemRepository,
        ILogger<ItemService> logger,
        IDistributedCache cache)
    {
        _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<Result<IEnumerable<Product>>> GetAllItems()
    {
        try
        {
            var cachedItems = await _cache.GetStringAsync(AllItemsCacheKey);
            if (cachedItems is not null)
            {
                _logger.LogInformation("Returning cached shop items");
                var cachedResult = JsonSerializer.Deserialize<IEnumerable<Product>>(cachedItems);
                return Result<IEnumerable<Product>>.Success(cachedResult!);
            }

            var dbItems = await _itemRepository.GetAllAsync();

            await _cache.SetStringAsync(
                AllItemsCacheKey,
                JsonSerializer.Serialize(dbItems),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                });

            return Result<IEnumerable<Product>>.Success(dbItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all shop items from DB or cache");
            return Result<IEnumerable<Product>>.Failure($"Failed to retrieve items: {ex.Message}");
        }
    }

    public async Task<Result<Product?>> GetItemById(int id)
    {
        try
        {
            var cacheKey = $"shop_item_{id}";
            
            var cachedItem = await _cache.GetStringAsync(cacheKey);
            if (cachedItem is not null)
            {
                _logger.LogInformation("Returning cached shop item with ID {ItemId}", id);
                var cachedResult = JsonSerializer.Deserialize<Product>(cachedItem);
                return Result<Product?>.Success(cachedResult);
            }

            var dbItem = await _itemRepository.GetByIdAsync(id);
            if (dbItem == null)
            {
                return Result<Product?>.Success(null);
            }

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(dbItem),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                });

            return Result<Product?>.Success(dbItem);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shop item with ID {ItemId}", id);
            return Result<Product?>.Failure($"Failed to retrieve item: {ex.Message}");
        }
    }

    public async Task<Result> AddItem(Product item)
    {
        try
        {
            if (item == null) 
                return Result.Failure("Item cannot be null");

            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveChangesAsync();
            
            await _cache.RemoveAsync(AllItemsCacheKey);

            _logger.LogInformation("Successfully added item: {@Item}", item);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding new item {@Item}", item);
            return Result.Failure($"Failed to add item: {ex.Message}");
        }
    }

    public async Task<Result> UpdateItem(Product item)
    {
        try
        {
            if (item == null) 
                return Result.Failure("Item cannot be null");

            var existingItem = await _itemRepository.GetByIdAsync(item.Id);
            if (existingItem == null)
            {
                return Result.Failure($"Item with ID {item.Id} not found");
            }

            await _itemRepository.UpdateAsync(item);
            await _itemRepository.SaveChangesAsync();
            
            await _cache.RemoveAsync(AllItemsCacheKey);
            await _cache.RemoveAsync($"shop_item_{item.Id}");

            _logger.LogInformation("Successfully updated item with ID {ItemId}: {@Item}", item.Id, item);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item with ID {ItemId}", item?.Id);
            return Result.Failure($"Failed to update item: {ex.Message}");
        }
    }

    public async Task<Result> DeleteItem(int id)
    {
        try
        {
            var existingItem = await _itemRepository.GetByIdAsync(id);
            if (existingItem == null)
            {
                return Result.Failure($"Item with ID {id} not found");
            }

            await _itemRepository.DeleteAsync(id);
            await _itemRepository.SaveChangesAsync();
            
            await _cache.RemoveAsync(AllItemsCacheKey);
            await _cache.RemoveAsync($"shop_item_{id}");

            _logger.LogInformation("Successfully deleted item with ID {ItemId}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item with ID {ItemId}", id);
            return Result.Failure($"Failed to delete item: {ex.Message}");
        }
    }
}