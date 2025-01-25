using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eShop.Models;
using eShop.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class ItemServiceTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<ILogger<ItemService>> _mockLogger;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockLogger = new Mock<ILogger<ItemService>>();
        _itemService = new ItemService(_mockContext.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllItems_ShouldLogInformationAndReturnItems()
    {
        // Arrange
        var items = new List<ShopItem>
        {
            new ShopItem { Id = 1, Name = "Item1", Price = 10.0m, Category = "Category1" },
            new ShopItem { Id = 2, Name = "Item2", Price = 20.0m, Category = "Category2" }
        };

        var dbSetMock = MockDbSet(items);
        _mockContext.Setup(c => c.ShopItems).Returns(dbSetMock.Object);

        // Act
        var result = await _itemService.GetAllItems();

        // Assert
        Assert.Equal(items.Count, result.Count());
        Assert.Contains(result, item => item.Name == "Item1");
        Assert.Contains(result, item => item.Name == "Item2");
        
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetching all items from the database.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task AddItem_ShouldLogInformationAndAddItem()
    {
        // Arrange
        var newItem = new ShopItem
        {
            Id = 3,
            Name = "NewItem",
            Price = 30.0m,
            Category = "Category3"
        };

        var dbSetMock = MockDbSet(new List<ShopItem>());
        _mockContext.Setup(c => c.ShopItems).Returns(dbSetMock.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _itemService.AddItem(newItem);

        // Assert
        dbSetMock.Verify(d => d.AddAsync(newItem, It.IsAny<CancellationToken>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Adding a new item")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Successfully added item")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task AddItem_ShouldLogErrorWhenExceptionOccurs()
    {
        // Arrange
        var newItem = new ShopItem { Id = 3, Name = "ErrorItem", Price = 40.0m, Category = "ErrorCategory" };
        _mockContext.Setup(c => c.ShopItems.AddAsync(newItem, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _itemService.AddItem(newItem));

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

    private Mock<DbSet<T>> MockDbSet<T>(List<T> list) where T : class
    {
        var queryable = list.AsQueryable();
        var dbSetMock = new Mock<DbSet<T>>();
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        return dbSetMock;
    }
}
