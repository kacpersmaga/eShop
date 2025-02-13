using AutoMapper;
using eShop.Api;
using eShop.Mappers.Profiles;
using eShop.Models.Domain;
using eShop.Models.Dtos;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UnitTests.Fakes;

namespace UnitTests.Controllers;

public class ShopApiControllerTests
{
    private readonly Mock<IItemService> _mockItemService;
    private readonly Mock<ILogger<ShopApiController>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly ShopApiController _controller;

    public ShopApiControllerTests()
    {
        _mockItemService = new Mock<IItemService>();
        _mockLogger = new Mock<ILogger<ShopApiController>>();
        
        var fakeImageService = new FakeImageService();
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ShopItemMappingProfile());
            cfg.ConstructServicesUsing(type => type == typeof(ImageUriResolver) 
                ? new ImageUriResolver(fakeImageService) 
                : null);
        });

        _mapper = config.CreateMapper();

        _controller = new ShopApiController(_mockItemService.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task GetItems_ReturnsOkResult_WithItems()
    {
        // Arrange
        var items = new List<ShopItem>
        {
            new ShopItem { Id = 1, Name = "Item1", Price = 100, Description = "Desc1", Category = "Cat1", ImagePath = "img1.jpg" },
            new ShopItem { Id = 2, Name = "Item2", Price = 200, Description = "Desc2", Category = "Cat2", ImagePath = "img2.jpg" }
        };

        _mockItemService.Setup(service => service.GetAllItems()).ReturnsAsync(items);

        // Act
        var result = await _controller.GetItems();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var returnedItems = Assert.IsAssignableFrom<IEnumerable<ShopItemViewModel>>(okResult.Value).ToList();
        Assert.Equal(2, returnedItems.Count);
        Assert.Contains(returnedItems, i => i is { Id: 1, ImageUri: "https://images.com/img1.jpg" });
        Assert.Contains(returnedItems, i => i is { Id: 2, ImageUri: "https://images.com/img2.jpg" });
    }

    [Fact]
    public async Task GetItems_ReturnsEmptyList_WhenNoItemsExist()
    {
        // Arrange
        _mockItemService.Setup(service => service.GetAllItems()).ReturnsAsync(new List<ShopItem>());

        // Act
        var result = await _controller.GetItems();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result); // Use OkObjectResult
        Assert.Equal(200, okResult.StatusCode);

        var returnedItems = Assert.IsAssignableFrom<IEnumerable<ShopItemViewModel>>(okResult.Value);
        Assert.Empty(returnedItems);
    }

    [Fact]
    public async Task GetItems_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        _mockItemService.Setup(service => service.GetAllItems()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetItems();

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);

        var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal("An error occurred while retrieving shop items.", errorResponse.Error);
    }

    [Fact]
    public void ControllerConstructor_ThrowsArgumentNullException_WhenItemServiceIsNull()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ShopApiController(null!, _mapper, _mockLogger.Object));
    }

    [Fact]
    public void ControllerConstructor_ThrowsArgumentNullException_WhenMapperIsNull()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ShopApiController(_mockItemService.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void ControllerConstructor_ThrowsArgumentNullException_WhenLoggerIsNull()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ShopApiController(_mockItemService.Object, _mapper, null!));
    }
}

