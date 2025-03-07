using Microsoft.Extensions.Caching.Distributed;
using eShop.Services.Interfaces;

namespace eShop.Services.Implementations;

public class ImageService(
    IBlobStorageService blobStorageService,
    ILogger<ImageService> logger,
    IDistributedCache cache)
    : IImageService
{
    private readonly IBlobStorageService _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
    private readonly ILogger<ImageService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public string GetImageUri(string? imagePath)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                _logger.LogWarning("ImagePath is null or empty. Returning default image.");
                return "/images/default.jpg";
            }

            var cachedUri = _cache.GetString(imagePath);
            if (cachedUri is not null)
            {
                _logger.LogInformation("Cache hit for imagePath: {ImagePath}", imagePath);
                return cachedUri;
            }

            _logger.LogInformation("Cache miss for imagePath: {ImagePath}. Generating new URI.", imagePath);
            var imageUri = _blobStorageService.GetBlobSasUri(imagePath);

            _cache.SetString(imagePath, imageUri, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            _logger.LogInformation("Image URI cached for imagePath: {ImagePath}", imagePath);

            return imageUri;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate or retrieve image URI for path: {ImagePath}", imagePath);
            return "/images/default.jpg";
        }
    }
}