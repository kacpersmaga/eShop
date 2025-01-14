using eShop.Controllers;
using eShop.Data;
using eShop.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace eShop.Tests.Services
{
    public class ItemServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ItemService _itemService;

        public ItemServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
            _itemService = new ItemService(_context);
        }

        [Fact]
        public void GetAllItems_ShouldReturnAllItems()
        {
            // Arrange
            var items = new List<ShopItem>
            {
                new ShopItem { Id = 1, Name = "Item1", Price = 10, Category = "Category1" },
                new ShopItem { Id = 2, Name = "Item2", Price = 20, Category = "Category2" }
            };

            _context.ShopItems.AddRange(items);
            _context.SaveChanges();

            // Act
            var result = _itemService.GetAllItems();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, i => i.Name == "Item1");
            Assert.Contains(result, i => i.Name == "Item2");
        }

        [Fact]
        public void GetAllItems_ShouldReturnEmpty_WhenNoItemsExist()
        {
            // Act
            var result = _itemService.GetAllItems();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void AddItem_ShouldAddNewItem()
        {
            // Arrange
            var newItem = new ShopItem
            {
                Name = "NewItem",
                Price = 15,
                Category = "Category3"
            };

            // Act
            _itemService.AddItem(newItem);

            // Assert
            var itemInDb = _context.ShopItems.FirstOrDefault(i => i.Name == "NewItem");
            Assert.NotNull(itemInDb);
            Assert.Equal("NewItem", itemInDb.Name);
            Assert.Equal(15, itemInDb.Price);
            Assert.Equal("Category3", itemInDb.Category);
        }

        [Fact]
        public void AddItem_ShouldThrowException_WhenInvalidData()
        {
            // Arrange
            var invalidItem = new ShopItem
            {
                Name = null, // Name is required
                Price = -10, // Price must be positive
                Category = "Category3"
            };

            // Act & Assert
            Assert.Throws<DbUpdateException>(() => _itemService.AddItem(invalidItem));
        }

        [Fact]
        public void AddItem_ShouldPersistItemInDatabase()
        {
            // Arrange
            var newItem = new ShopItem
            {
                Name = "PersistentItem",
                Price = 30,
                Category = "Category4"
            };

            // Act
            _itemService.AddItem(newItem);

            // Detach from context and reload to simulate fresh retrieval
            _context.Entry(newItem).State = EntityState.Detached;
            var itemInDb = _context.ShopItems.FirstOrDefault(i => i.Name == "PersistentItem");

            // Assert
            Assert.NotNull(itemInDb);
            Assert.Equal("PersistentItem", itemInDb.Name);
            Assert.Equal(30, itemInDb.Price);
            Assert.Equal("Category4", itemInDb.Category);
        }

        [Fact]
        public void GetAllItems_ShouldIncludeNewlyAddedItems()
        {
            // Arrange
            var newItem = new ShopItem
            {
                Name = "NewlyAdded",
                Price = 50,
                Category = "Category5"
            };

            _itemService.AddItem(newItem);

            // Act
            var result = _itemService.GetAllItems();

            // Assert
            Assert.Contains(result, i => i.Name == "NewlyAdded");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
