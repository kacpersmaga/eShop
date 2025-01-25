using eShop.Data;
using eShop.Models;
using eShop.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eShop.Tests.Services
{
    public class ItemServiceTests : IAsyncLifetime
    {
        private readonly ApplicationDbContext _context;
        private readonly ItemService _itemService;
        private readonly Mock<ILogger<ItemService>> _mockLogger;

        public ItemServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<ItemService>>(); // Mock the logger
            _itemService = new ItemService(_context, _mockLogger.Object); // Pass both arguments
        }

        [Fact]
        public async Task GetAllItems_ShouldReturnAllExistingItems()
        {
            // Arrange
            var items = CreateTestItems();
            await _context.ShopItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            // Act
            var result = await _itemService.GetAllItems();

            // Assert
            Assert.Equal(items.Count, result.Count());
            foreach (var item in items)
            {
                Assert.Contains(result, r => r.Name == item.Name && r.Price == item.Price && r.Category == item.Category);
            }
        }

        [Fact]
        public async Task GetAllItems_ShouldReturnEmptyList_WhenNoItemsExist()
        {
            // Act
            var result = await _itemService.GetAllItems();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task AddItem_ShouldSuccessfullyAddNewItem()
        {
            // Arrange
            var newItem = new ShopItem
            {
                Name = "TestItem",
                Price = 100,
                Category = "TestCategory"
            };

            // Act
            await _itemService.AddItem(newItem);

            // Assert
            var itemInDb = await _context.ShopItems.FirstOrDefaultAsync(i => i.Name == "TestItem");
            Assert.NotNull(itemInDb);
            Assert.Equal(newItem.Name, itemInDb.Name);
            Assert.Equal(newItem.Price, itemInDb.Price);
            Assert.Equal(newItem.Category, itemInDb.Category);
        }

        [Fact]
        public async Task AddItem_ShouldThrowException_WhenAddingInvalidItem()
        {
            // Arrange
            var invalidItem = new ShopItem
            {
                Name = null, // Invalid: Name is required
                Price = -10, // Invalid: Price must be positive
                Category = "InvalidCategory"
            };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(async () => await _itemService.AddItem(invalidItem));
        }

        [Fact]
        public async Task AddItem_ShouldBePersistentAcrossContextUsage()
        {
            // Arrange
            var newItem = new ShopItem
            {
                Name = "PersistentItem",
                Price = 50,
                Category = "PersistentCategory"
            };

            // Act
            await _itemService.AddItem(newItem);

            // Simulate detached state
            _context.Entry(newItem).State = EntityState.Detached;

            var itemInDb = await _context.ShopItems.FirstOrDefaultAsync(i => i.Name == "PersistentItem");

            // Assert
            Assert.NotNull(itemInDb);
            Assert.Equal(newItem.Name, itemInDb.Name);
            Assert.Equal(newItem.Price, itemInDb.Price);
            Assert.Equal(newItem.Category, itemInDb.Category);
        }

        [Fact]
        public async Task GetAllItems_ShouldIncludeNewlyAddedItem()
        {
            // Arrange
            var newItem = new ShopItem
            {
                Name = "NewItem",
                Price = 200,
                Category = "NewCategory"
            };

            await _itemService.AddItem(newItem);

            // Act
            var result = await _itemService.GetAllItems();

            // Assert
            Assert.Contains(result, i => i.Name == "NewItem" && i.Price == 200 && i.Category == "NewCategory");
        }

        private List<ShopItem> CreateTestItems()
        {
            return new List<ShopItem>
            {
                new ShopItem { Id = 1, Name = "Item1", Price = 10, Category = "Category1" },
                new ShopItem { Id = 2, Name = "Item2", Price = 20, Category = "Category2" },
                new ShopItem { Id = 3, Name = "Item3", Price = 30, Category = "Category3" }
            };
        }

        public async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }
    }
}
