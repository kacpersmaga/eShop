using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eShop.Data;
using eShop.Models;
using eShop.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class ItemServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<ItemService>> _mockLogger;
    private readonly IMemoryCache _cache;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<ItemService>>();
        _cache = new MemoryCache(new MemoryCacheOptions());

        _itemService = new ItemService(_context, _mockLogger.Object, _cache);
    }



    [Fact]
    public async Task GetAllItems_ShouldReturnItemsFromCache_WhenCacheIsAvailable()
    {

        // Arrange
        const string cacheKey = "all_shop_items";
        var cachedItems = new List<ShopItem>
        {
            new ShopItem { Id = 1, Name = "CachedItem1", Price = 10.0m, Category = "Category1" },
            new ShopItem { Id = 2, Name = "CachedItem2", Price = 20.0m, Category = "Category2" }
        };

        _cache.Set(cacheKey, cachedItems);

        // Act
        var result = await _itemService.GetAllItems();

        // Assert
        Assert.Equal(cachedItems.Count, result.Count());
        Assert.Contains(result, item => item.Name == "CachedItem1");
        Assert.Contains(result, item => item.Name == "CachedItem2");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Returning cached shop items.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );

        Assert.Empty(await _context.ShopItems.ToListAsync()); // Ensure database was NOT queried
    }

    [Fact]
    public async Task GetAllItems_ShouldFetchFromDatabase_WhenCacheIsEmpty()
    {

        // Arrange
        _context.ShopItems.AddRange(new List<ShopItem>
        {
            new ShopItem { Id = 1, Name = "DBItem1", Price = 10.0m, Category = "Category1" },
            new ShopItem { Id = 2, Name = "DBItem2", Price = 20.0m, Category = "Category2" }
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _itemService.GetAllItems();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, item => item.Name == "DBItem1");
        Assert.Contains(result, item => item.Name == "DBItem2");

        Assert.True(_cache.TryGetValue("all_shop_items", out _)); // Verify cache was updated
    }

    [Fact]
    public async Task AddItem_ShouldInvalidateCache()
    {

        // Arrange
        var newItem = new ShopItem
        {
            Id = 3,
            Name = "NewItem",
            Price = 30.0m,
            Category = "Category3"
        };

        _cache.Set("all_shop_items", new List<ShopItem>());

        // Act
        await _itemService.AddItem(newItem);

        // Assert
        var items = await _context.ShopItems.ToListAsync();
        Assert.Single(items);
        Assert.Equal("NewItem", items[0].Name);

        Assert.False(_cache.TryGetValue("all_shop_items", out _)); // Verify cache invalidation
    }

    [Fact]
    public async Task AddItem_ShouldLogErrorWhenExceptionOccurs()
    {

        // Arrange
        var newItem = new ShopItem { Id = 3, Name = "ErrorItem", Price = 40.0m, Category = "ErrorCategory" };
        _context.Dispose(); // Force an error by disposing the database

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => _itemService.AddItem(newItem));

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while adding item")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }
    
}
