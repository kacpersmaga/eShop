using Azure.Storage.Sas;
using eShop.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

public class BlobStorageServiceTests
{
    private readonly Mock<IBlobStorageServiceWrapper> _mockBlobWrapper;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<BlobStorageService>> _mockLogger;
    private readonly BlobStorageService _service;
    private readonly string _testContainerName = "test-container";

    public BlobStorageServiceTests()
    {
        _mockBlobWrapper = new Mock<IBlobStorageServiceWrapper>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<BlobStorageService>>();

        _mockConfiguration.Setup(c => c["AzureBlobStorage:ContainerName"]).Returns(_testContainerName);

        _service = new BlobStorageService(_mockBlobWrapper.Object, _mockConfiguration.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task UploadFileAsync_NullFile_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UploadFileAsync(null));
        Assert.Equal("file", exception.ParamName);
    }

    [Fact]
    public async Task UploadFileAsync_EmptyFile_ThrowsArgumentException()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UploadFileAsync(mockFile.Object));
        Assert.Equal("file", exception.ParamName);
        Assert.Contains("File cannot be empty.", exception.Message);
    }

    [Fact]
    public async Task UploadFileAsync_ValidImageFile_UploadsSuccessfully()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.FileName).Returns("image.jpg");
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[1024]));

        // Act
        var blobName = await _service.UploadFileAsync(mockFile.Object);

        // Assert
        _mockBlobWrapper.Verify(b => b.UploadBlobAsync(_testContainerName, It.IsAny<string>(), It.IsAny<Stream>(), mockFile.Object.ContentType), Times.Once);
        Assert.NotNull(blobName);
        Assert.EndsWith(".jpg", blobName);
        _mockLogger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("File uploaded successfully")),
            null,
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task UploadFileAsync_NonImageFile_ThrowsArgumentException()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.FileName).Returns("document.pdf");
        mockFile.Setup(f => f.ContentType).Returns("application/pdf");
        mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[1024]));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _service.UploadFileAsync(mockFile.Object));
        Assert.Equal("file", exception.ParamName);
        Assert.Contains("Only image files are allowed.", exception.Message);
    }

    [Fact]
    public void GetBlobSasUri_NullBlobName_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _service.GetBlobSasUri(null));
        Assert.Equal("blobName", exception.ParamName);
    }

    [Fact]
    public void GetBlobSasUri_ValidBlobName_ReturnsSasUri()
    {
        // Arrange
        var blobName = "image.jpg";
        var expectedUri = "https://example.com/image.jpg?sas-token";
        _mockBlobWrapper.Setup(b => b.GenerateBlobSasUri(_testContainerName, blobName, BlobSasPermissions.Read, TimeSpan.FromHours(1))).Returns(expectedUri);

        // Act
        var sasUri = _service.GetBlobSasUri(blobName);

        // Assert
        Assert.Equal(expectedUri, sasUri);
        _mockLogger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Generated SAS URI for blob")),
            null,
            (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public void Constructor_NullBlobWrapper_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new BlobStorageService(null, _mockConfiguration.Object, _mockLogger.Object));
        Assert.Equal("blobWrapper", exception.ParamName);
    }

    [Fact]
    public void Constructor_NullConfiguration_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new BlobStorageService(_mockBlobWrapper.Object, null, _mockLogger.Object));
        Assert.Equal("configuration", exception.ParamName);
    }

    [Fact]
    public void Constructor_NullLogger_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => new BlobStorageService(_mockBlobWrapper.Object, _mockConfiguration.Object, null));
        Assert.Equal("logger", exception.ParamName);
    }
}
