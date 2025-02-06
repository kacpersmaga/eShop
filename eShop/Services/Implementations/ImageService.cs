using eShop.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace eShop.Services.Implementations;

public class ImageService(IBlobStorageService blobStorageService, ILogger<ImageService> logger, IMemoryCache cache)
    : IImageService
{
    private readonly IBlobStorageService _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
    private readonly ILogger<ImageService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public string GetImageUri(string? imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            _logger.LogWarning("ImagePath is null or empty. Returning default image.");
            return "/images/default.jpg";
        }

        try
        {
            if (_cache.TryGetValue(imagePath, out string? cachedUri) && cachedUri is not null)
            {
                _logger.LogInformation("Cache hit for imagePath: {ImagePath}", imagePath);
                return cachedUri;
            }
            
            _logger.LogInformation("Cache miss for imagePath: {ImagePath}. Generating new URI.", imagePath);
            string imageUri = _blobStorageService.GetBlobSasUri(imagePath);
            
            _cache.Set(imagePath, imageUri, TimeSpan.FromMinutes(10));
            _logger.LogInformation("Image URI cached for imagePath: {ImagePath}", imagePath);

            return imageUri;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate image URI for path: {ImagePath}", imagePath);
            return "/images/default.jpg";
        }
    }
}