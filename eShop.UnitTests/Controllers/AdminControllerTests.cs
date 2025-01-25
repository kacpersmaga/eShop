using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using eShop.Controllers;
using eShop.Models;
using eShop.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eShop.Tests
{
    public class AdminControllerTests
    {
        private readonly Mock<IItemService> _itemServiceMock;
        private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
        private readonly Mock<ILogger<AdminController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _itemServiceMock = new Mock<IItemService>();
            _blobStorageServiceMock = new Mock<IBlobStorageService>();
            _loggerMock = new Mock<ILogger<AdminController>>();
            _mapperMock = new Mock<IMapper>();
            _controller = new AdminController(
                _itemServiceMock.Object,
                _blobStorageServiceMock.Object,
                _loggerMock.Object,
                _mapperMock.Object
            );

            // Initialize TempData
            _controller.TempData = new Mock<ITempDataDictionary>().Object;
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void AddItem_Get_ReturnsViewResultWithModel()
        {
            // Act
            var result = _controller.AddItem();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ShopItemFormModel>(viewResult.Model);
            Assert.Equal("", model.Name);
            Assert.Equal("", model.Category);
            Assert.Null(model.Description);
        }

        [Fact]
        public async Task AddItem_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var formModel = new ShopItemFormModel { Name = "Test Item" };

            // Act
            var result = await _controller.AddItem(formModel, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(formModel, viewResult.Model);
            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    null,
                    (Func<object, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task AddItem_Post_ValidModelWithImage_AddsItemAndRedirects()
        {
            // Arrange
            var formModel = new ShopItemFormModel { Name = "Test Item", Category = "Test Category" };
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1);
            fileMock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());
            fileMock.Setup(f => f.FileName).Returns("test.jpg");

            _blobStorageServiceMock
                .Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("image/path/test.jpg");

            var shopItem = new ShopItem();
            _mapperMock.Setup(m => m.Map<ShopItem>(formModel)).Returns(shopItem);

            var tempDataMock = new Mock<ITempDataDictionary>();
            tempDataMock.SetupSet(t => t["SuccessMessage"] = $"Item '{formModel.Name}' added successfully!").Verifiable();
            _controller.TempData = tempDataMock.Object;

            // Act
            var result = await _controller.AddItem(formModel, fileMock.Object);

            // Assert
            _blobStorageServiceMock.Verify(s => s.UploadFileAsync(fileMock.Object), Times.Once);
            _itemServiceMock.Verify(s => s.AddItem(shopItem), Times.Once);
            Assert.Equal("image/path/test.jpg", shopItem.ImagePath);

            tempDataMock.Verify();

            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Item 'Test Item' added successfully")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddItem", redirectResult.ActionName);
        }

        [Fact]
        public async Task AddItem_Post_ValidModelWithoutImage_AddsItemAndRedirects()
        {
            // Arrange
            var formModel = new ShopItemFormModel { Name = "Test Item", Category = "Test Category" };

            var shopItem = new ShopItem();
            _mapperMock.Setup(m => m.Map<ShopItem>(formModel)).Returns(shopItem);

            var tempDataMock = new Mock<ITempDataDictionary>();
            tempDataMock.SetupSet(t => t["SuccessMessage"] = $"Item '{formModel.Name}' added successfully!").Verifiable();
            _controller.TempData = tempDataMock.Object;

            // Act
            var result = await _controller.AddItem(formModel, null);

            // Assert
            _blobStorageServiceMock.Verify(s => s.UploadFileAsync(It.IsAny<IFormFile>()), Times.Never);
            _itemServiceMock.Verify(s => s.AddItem(shopItem), Times.Once);
            Assert.Null(shopItem.ImagePath);

            tempDataMock.Verify();

            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Item 'Test Item' added successfully")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddItem", redirectResult.ActionName);
        }

        [Fact]
        public async Task AddItem_Post_ThrowsException_ReturnsViewWithError()
        {
            // Arrange
            var formModel = new ShopItemFormModel { Name = "Test Item", Category = "Test Category" };

            _mapperMock.Setup(m => m.Map<ShopItem>(formModel)).Returns(new ShopItem());
            _itemServiceMock
                .Setup(s => s.AddItem(It.IsAny<ShopItem>()))
                .ThrowsAsync(new Exception("Test Exception"));

            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            // Act
            var result = await _controller.AddItem(formModel, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(formModel, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey(string.Empty));
            Assert.Equal("An error occurred while processing your request. Please try again later.",
                _controller.ModelState[string.Empty].Errors[0].ErrorMessage);

            _loggerMock.Verify(
                log => log.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    (Func<object, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }
    }
}