using System.Net;
using System.Text.Json;
using eShop.Api;
using eShop.Models.Domain;
using eShop.Models.Dtos;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace IntegrationTests.Controllers
{
    public class ShopApiControllerTests : IClassFixture<WebApplicationFactory<ShopApiController>>
    {
        private readonly HttpClient _client;
        private readonly Mock<IItemService> _mockItemService;
        private readonly Mock<IImageService> _mockImageService;
        private readonly Mock<ILogger<ShopApiController>> _mockLogger;

        public ShopApiControllerTests(WebApplicationFactory<ShopApiController> factory)
        {
            _mockItemService = new Mock<IItemService>();
            _mockImageService = new Mock<IImageService>();
            _mockLogger = new Mock<ILogger<ShopApiController>>();

            var customFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(_mockItemService.Object);
                    services.AddSingleton(_mockImageService.Object);
                    services.AddSingleton(_mockLogger.Object);
                });
            });

            _client = customFactory.CreateClient();
        }

        [Fact]
        public async Task GetItems_ReturnsItemsSuccessfully()
        {
            // Arrange
            var items = new List<ShopItem>
            {
                new ShopItem { Id = 1, Name = "Laptop", Price = 1500.00m, Description = "Gaming Laptop", Category = "Electronics", ImagePath = "laptop.jpg" },
                new ShopItem { Id = 2, Name = "Smartphone", Price = 800.00m, Description = "Flagship phone", Category = "Electronics", ImagePath = "phone.jpg" }
            };
            
            _mockItemService.Setup(s => s.GetAllItems()).ReturnsAsync(items);
            _mockImageService.Setup(s => s.GetImageUri(It.IsAny<string>())).Returns<string>(img => $"https://cdn.eshop.com/{img}");

            // Act
            var response = await _client.GetAsync("api/shop/items");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ShopItemViewModel>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Laptop", result[0].Name);
            Assert.Equal("https://cdn.eshop.com/laptop.jpg", result[0].ImageUri);
        }

        [Fact]
        public async Task GetItems_Returns500_WhenServiceFails()
        {
            // Arrange
            _mockItemService.Setup(s => s.GetAllItems()).ThrowsAsync(new Exception("Database failure"));

            // Act
            var response = await _client.GetAsync("api/shop/items");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            
            Assert.Contains("error", responseString);
            
            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error fetching items for the shop.")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Once);
        }
    }
}
