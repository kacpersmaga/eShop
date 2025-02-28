/*
using AutoMapper;
using eShop.Api;
using eShop.Models.Domain;
using eShop.Models.Dtos;
using eShop.Services.Interfaces;
using eShop.Validators.Dtos;
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
    public void Validate_InvalidNameAndCategory_ShouldHaveErrors()
    {
        // Arrange
        var validator = new ShopItemFormModelValidator();
        var model = new ShopItemFormModel 
        { 
            Name = new string('A', 101), 
            Category = new string('B', 51)
        };
    
        // Act
        var result = validator.Validate(model);
    
        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
        Assert.Contains(result.Errors, e => e.PropertyName == "Category");
    }
    [Fact]
    public async Task AddItem_ExceptionThrown_ThrowsException()
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

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _apiController.AddItem(formModel, null));
    }
}
*/

