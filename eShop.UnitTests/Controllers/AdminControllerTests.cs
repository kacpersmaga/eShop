using eShop.Controllers;
using eShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace eShop.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IItemService> _mockItemService;
        private readonly Mock<IBlobStorageService> _mockBlobStorageService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockItemService = new Mock<IItemService>();
            _mockBlobStorageService = new Mock<IBlobStorageService>();

            // Default mock setups
            _mockBlobStorageService.Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("mock-image-path");
            _mockBlobStorageService.Setup(s => s.GetBlobSasUri(It.IsAny<string>()))
                .Returns("mock-sas-uri");

            // Initialize TempData for the controller
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller = new AdminController(_mockItemService.Object, _mockBlobStorageService.Object)
            {
                TempData = tempData
            };
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }

        [Fact]
        public void AddItem_Get_ReturnsViewWithShopItemFormModel()
        {
            // Act
            var result = _controller.AddItem();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ShopItemFormModel>(viewResult.Model);
        }

        [Fact]
        public async Task AddItem_Post_ReturnsRedirectToAction_WhenModelIsValidAndImageProvided()
        {
            // Arrange
            var formModel = new ShopItemFormModel
            {
                Name = "Test Item",
                Price = 10.99m,
                Category = "Test Category",
                Description = "Test Description"
            };

            var fileMock = CreateMockFile("test-image.jpg", "Fake content");

            _mockBlobStorageService.Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("uploaded-image-path");

            _mockItemService.Setup(s => s.AddItem(It.IsAny<ShopItem>()));

            // Act
            var result = await _controller.AddItem(formModel, fileMock.Object);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddItem", redirectResult.ActionName);

            _mockBlobStorageService.Verify(s => s.UploadFileAsync(It.IsAny<IFormFile>()), Times.Once);
            _mockItemService.Verify(s => s.AddItem(It.IsAny<ShopItem>()), Times.Once);
        }

        [Fact]
        public async Task AddItem_Post_ReturnsViewWithErrors_WhenModelIsInvalid()
        {
            // Arrange
            var formModel = new ShopItemFormModel
            {
                Name = "",
                Price = 10.99m,
                Category = "Test Category",
                Description = "Test Description"
            };

            _controller.ModelState.AddModelError("Name", "The Name field is required.");

            // Act
            var result = await _controller.AddItem(formModel, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ShopItemFormModel>(viewResult.Model);

            Assert.Equal(formModel, model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task AddItem_Post_HandlesNullImage()
        {
            // Arrange
            var formModel = new ShopItemFormModel
            {
                Name = "Test Item",
                Price = 10.99m,
                Category = "Test Category",
                Description = "Test Description"
            };

            _mockItemService.Setup(s => s.AddItem(It.IsAny<ShopItem>()));

            // Act
            var result = await _controller.AddItem(formModel, null);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddItem", redirectResult.ActionName);

            _mockItemService.Verify(s => s.AddItem(It.IsAny<ShopItem>()), Times.Once);
            _mockBlobStorageService.Verify(s => s.UploadFileAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task AddItem_Post_ReturnsViewWithErrorMessage_WhenBlobStorageFails()
        {
            // Arrange
            var formModel = new ShopItemFormModel
            {
                Name = "Test Item",
                Price = 10.99m,
                Category = "Test Category",
                Description = "Test Description"
            };

            var fileMock = CreateMockFile("test-image.jpg", "Fake content");

            _mockBlobStorageService.Setup(s => s.UploadFileAsync(It.IsAny<IFormFile>()))
                .ThrowsAsync(new IOException("Blob storage upload failed."));

            // Act
            var result = await _controller.AddItem(formModel, fileMock.Object);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ShopItemFormModel>(viewResult.Model);

            Assert.Equal(formModel, model);
            _mockBlobStorageService.Verify(s => s.UploadFileAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        private Mock<IFormFile> CreateMockFile(string fileName, string content)
        {
            var fileMock = new Mock<IFormFile>();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("image/jpeg");

            return fileMock;
        }
    }
}
