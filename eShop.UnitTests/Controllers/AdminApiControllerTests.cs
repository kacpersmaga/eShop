using AutoMapper;
using eShop.Api;
using eShop.Models.Domain;
using eShop.Models.Dtos;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Controllers;
    
public class AdminApiControllerTests
{
    private readonly Mock<IItemService> _itemServiceMock;
    private readonly Mock<IBlobStorageService> _blobServiceMock;
    private readonly AdminApiController _apiController;

    public AdminApiControllerTests()
    {
        _itemServiceMock = new Mock<IItemService>();
        _blobServiceMock = new Mock<IBlobStorageService>();
        var loggerMock = new Mock<ILogger<AdminApiController>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ShopItemFormModel, ShopItem>();
        });
        var mapper = config.CreateMapper();

        _apiController = new AdminApiController(
            _itemServiceMock.Object,
            _blobServiceMock.Object,
            mapper,
            loggerMock.Object
        );
    }

    [Fact]
    public async Task AddItem_ValidModel_NoImage_ReturnsOkWithSuccessResponse()
    {
        // Arrange
        var formModel = new ShopItemFormModel
        {
            Name = "TestItem",
            Price = 9.99m,
            Category = "Book"
        };

        IFormFile? image = null;

        // Act
        var result = await _apiController.AddItem(formModel, image);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var successResponse = Assert.IsType<SuccessResponse>(okResult.Value);
        Assert.Equal("Item 'TestItem' added successfully!", successResponse.Message);

        _blobServiceMock.Verify(x => x.UploadFileAsync(It.IsAny<IFormFile>()), Times.Never);
        _itemServiceMock.Verify(x => x.AddItem(It.Is<ShopItem>(si =>
            si.Name == "TestItem" &&
            si.Price == 9.99m &&
            si.Category == "Book" &&
            si.ImagePath == null
        )), Times.Once);
    }

    [Fact]
    public async Task AddItem_ValidModel_WithImage_ReturnsOkWithSuccessResponse()
    {
        // Arrange
        var formModel = new ShopItemFormModel
        {
            Name = "TestItemWithImage",
            Price = 19.99m,
            Category = "Electronics"
        };

        var imageMock = new Mock<IFormFile>();
        imageMock.Setup(f => f.Length).Returns(100);
        imageMock.Setup(f => f.FileName).Returns("testimage.png");

        _blobServiceMock
            .Setup(bs => bs.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("https://fake.blob.com/testimage.png");

        // Act
        var result = await _apiController.AddItem(formModel, imageMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var successResponse = Assert.IsType<SuccessResponse>(okResult.Value);
        Assert.Equal("Item 'TestItemWithImage' added successfully!", successResponse.Message);

        _blobServiceMock.Verify(x => x.UploadFileAsync(It.Is<IFormFile>(f => f.FileName == "testimage.png")), Times.Once);
        _itemServiceMock.Verify(x => x.AddItem(It.Is<ShopItem>(si =>
            si.Name == "TestItemWithImage" &&
            si.Price == 19.99m &&
            si.Category == "Electronics" &&
            si.ImagePath == "https://fake.blob.com/testimage.png"
        )), Times.Once);
    }

    [Fact]
    public async Task AddItem_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var formModel = new ShopItemFormModel
        {
            Name = new string('A', 101),
            Price = 100001,
            Category = new string('B', 51)
        };

        _apiController.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _apiController.AddItem(formModel, null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);

        _blobServiceMock.Verify(x => x.UploadFileAsync(It.IsAny<IFormFile>()), Times.Never);
        _itemServiceMock.Verify(x => x.AddItem(It.IsAny<ShopItem>()), Times.Never);
    }

    [Fact]
    public async Task AddItem_ExceptionThrown_ReturnsInternalServerErrorWithErrorResponse()
    {
        // Arrange
        var formModel = new ShopItemFormModel
        {
            Name = "ThrowTest",
            Price = 9.99m,
            Category = "Books"
        };

        _itemServiceMock
            .Setup(s => s.AddItem(It.IsAny<ShopItem>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _apiController.AddItem(formModel, null);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);

        var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
        Assert.Equal("An error occurred. Please try again later.", errorResponse.Error);
    }
}

