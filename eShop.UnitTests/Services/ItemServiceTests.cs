using eShop.Data;
using eShop.Models.Domain;
using eShop.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Services;

public class ItemServiceUnitTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<ItemService>> _mockLogger;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly ItemService _itemService;

    public ItemServiceUnitTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _mockLogger = new Mock<ILogger<ItemService>>();
        _mockCache = new Mock<IMemoryCache>();

        _itemService = new ItemService(_context, _mockLogger.Object, _mockCache.Object);
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

        object? cacheEntry = cachedItems;
        _mockCache.Setup(x => x.TryGetValue(cacheKey, out cacheEntry)).Returns(true);

        // Act
        var result = (await _itemService.GetAllItems()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, item => item.Name == "CachedItem1");
        Assert.Contains(result, item => item.Name == "CachedItem2");

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Returning cached shop items.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
        
        Assert.Empty(_context.ShopItems);
    }

    [Fact]
    public async Task GetAllItems_ShouldFetchFromDatabase_WhenCacheIsEmpty()
    {
        // Arrange
        _context.ShopItems.AddRange(new List<ShopItem>
        {
            new ShopItem { Id = 1, Name = "Item1", Price = 10.0m, Category = "Category1" },
            new ShopItem { Id = 2, Name = "Item2", Price = 20.0m, Category = "Category2" }
        });
        await _context.SaveChangesAsync();

        const string cacheKey = "all_shop_items";
        object? cacheEntry = null;
        _mockCache.Setup(x => x.TryGetValue(cacheKey, out cacheEntry)).Returns(false);

        var cacheEntryMock = new Mock<ICacheEntry>();
        _mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

        // Act
        var result = (await _itemService.GetAllItems()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, item => item.Name == "Item1");
        Assert.Contains(result, item => item.Name == "Item2");
        
        _mockCache.Verify(x => x.CreateEntry(cacheKey), Times.Once);
    }

    [Fact]
    public async Task AddItem_ShouldInvalidateCache()
    {
        // Arrange
        var newItem = new ShopItem { Id = 3, Name = "NewItem", Price = 30.0m, Category = "Category3" };

        // Act
        await _itemService.AddItem(newItem);
        var savedItem = await _context.ShopItems.FirstOrDefaultAsync(i => i.Name == "NewItem");

        // Assert
        Assert.NotNull(savedItem);
        Assert.Equal("NewItem", savedItem.Name);
        Assert.Equal(30.0m, savedItem.Price);
        
        _mockCache.Verify(x => x.Remove("all_shop_items"), Times.Once);
    }

    [Fact]
    public async Task AddItem_ShouldThrowException_WhenServiceFails()
    {
        // Arrange
        var newItem = new ShopItem { Id = 3, Name = "ErrorItem", Price = 40.0m, Category = "ErrorCategory" };
    
        await _context.DisposeAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => _itemService.AddItem(newItem));
    }
}