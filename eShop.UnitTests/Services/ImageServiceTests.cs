using eShop.Services.Implementations;
using eShop.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.Services;

public class ImageServiceTests
{
    private readonly Mock<IBlobStorageService> _mockBlobStorageService;
    private readonly Mock<ILogger<ImageService>> _mockLogger;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly ImageService _imageService;

    public ImageServiceTests()
    {
        _mockBlobStorageService = new Mock<IBlobStorageService>();
        _mockLogger = new Mock<ILogger<ImageService>>();
        _mockCache = new Mock<IMemoryCache>();

        _imageService = new ImageService(_mockBlobStorageService.Object, _mockLogger.Object, _mockCache.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GetImageUri_ShouldReturnDefaultImage_WhenImagePathIsNullOrEmpty(string? imagePath)
    {
        // Act
        var result = _imageService.GetImageUri(imagePath);

        // Assert
        Assert.Equal("/images/default.jpg", result);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ImagePath is null or empty")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public void GetImageUri_ShouldReturnCachedUri_WhenCacheIsAvailable()
    {
        // Arrange
        const string imagePath = "test.jpg";
        const string cachedUri = "https://storage.com/test.jpg";
        
        object? cacheEntry = cachedUri;
        _mockCache.Setup(x => x.TryGetValue(imagePath, out cacheEntry)).Returns(true);

        // Act
        var result = _imageService.GetImageUri(imagePath);

        // Assert
        Assert.Equal(cachedUri, result);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cache hit for imagePath")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public void GetImageUri_ShouldFetchFromBlobStorage_WhenCacheIsEmpty()
    {
        // Arrange
        const string imagePath = "new-image.jpg";
        const string expectedUri = "https://storage.com/new-image.jpg";
        
        object? cacheEntry = null;
        _mockCache.Setup(x => x.TryGetValue(imagePath, out cacheEntry)).Returns(false);
        
        var cacheEntryMock = new Mock<ICacheEntry>();
        _mockCache.Setup(m => m.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);

        _mockBlobStorageService.Setup(x => x.GetBlobSasUri(imagePath)).Returns(expectedUri);

        // Act
        var result = _imageService.GetImageUri(imagePath);

        // Assert
        Assert.Equal(expectedUri, result);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Cache miss for imagePath")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );

        _mockCache.Verify(x => x.CreateEntry(imagePath), Times.Once);
    }

    [Fact]
    public void GetImageUri_ShouldReturnDefaultImage_WhenBlobStorageThrowsException()
    {
        // Arrange
        const string imagePath = "error-image.jpg";

        object? cacheEntry = null;
        _mockCache.Setup(x => x.TryGetValue(imagePath, out cacheEntry)).Returns(false);

        _mockBlobStorageService.Setup(x => x.GetBlobSasUri(imagePath)).Throws(new Exception("Blob storage error"));

        // Act
        var result = _imageService.GetImageUri(imagePath);

        // Assert
        Assert.Equal("/images/default.jpg", result);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to generate image URI")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }
}