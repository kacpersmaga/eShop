using eShop.Controllers;
using eShop.Mappers;
using eShop.Models;
using eShop.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public class AdminControllerTests
{
    private readonly Mock<IItemService> _mockItemService;
    private readonly Mock<IBlobStorageService> _mockBlobStorageService;
    private readonly Mock<ILogger<AdminController>> _mockLogger;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _mockItemService = new Mock<IItemService>();
        _mockBlobStorageService = new Mock<IBlobStorageService>();
        _mockLogger = new Mock<ILogger<AdminController>>();
        _controller = new AdminController(
            _mockItemService.Object,
            _mockBlobStorageService.Object,
            _mockLogger.Object
        );
    }

    private void InitializeControllerContext()
    {
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public void Index_ReturnsViewResult()
    {
        // Act
        var result = _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void AddItem_Get_ReturnsViewWithModel()
    {
        // Act
        var result = _controller.AddItem();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ShopItemFormModel>(viewResult.Model);
    }

    [Fact]
    public async Task AddItem_Post_InvalidModelState_ReturnsViewWithModel()
    {
        // Arrange
        InitializeControllerContext();
        var model = new ShopItemFormModel { Name = "" }; // Invalid model
        _controller.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await _controller.AddItem(model, null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid model state")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
    }

    [Fact]
    public async Task AddItem_Post_ValidModel_AddsItemAndRedirects()
    {
        // Arrange
        InitializeControllerContext();
    
        // Setup TempData
        var tempDataProvider = new Mock<ITempDataProvider>().Object;
        var tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider);
        var tempData = tempDataDictionaryFactory.GetTempData(new DefaultHttpContext());
        _controller.TempData = tempData;

        var model = new ShopItemFormModel
        {
            Name = "Test Item",
            Category = "Test Category",
            Price = 10.0m,
            Description = "Test Description"
        };

        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        _mockBlobStorageService
            .Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync("test/path/image.jpg");

        _mockItemService
            .Setup(x => x.AddItem(It.IsAny<ShopItem>()))
            .Verifiable();

        // Act
        var result = await _controller.AddItem(model, mockFile.Object);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("AddItem", redirectResult.ActionName);

        _mockItemService.Verify(x => x.AddItem(It.IsAny<ShopItem>()), Times.Once);
        _mockBlobStorageService.Verify(x => x.UploadFileAsync(mockFile.Object), Times.Once);
    }
    [Fact]
    public async Task AddItem_Post_ExceptionDuringProcessing_ReturnsViewWithError()
    {
        // Arrange
        InitializeControllerContext();
        var model = new ShopItemFormModel { Name = "Test Item" };
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1);
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

        _mockBlobStorageService
            .Setup(x => x.UploadFileAsync(It.IsAny<IFormFile>()))
            .ThrowsAsync(new Exception("Blob storage error"));

        // Act
        var result = await _controller.AddItem(model, mockFile.Object);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error occurred while adding item")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);

        Assert.True(_controller.ModelState.ContainsKey(""));
    }
}
