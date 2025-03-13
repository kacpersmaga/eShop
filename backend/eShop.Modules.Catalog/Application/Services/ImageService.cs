using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Interfaces.Storage;
using eShop.Shared.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Services;

public class ImageService : IImageService
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<ImageService> _logger;
    private readonly IDistributedCache _cache;

    public ImageService(
        IBlobStorageService blobStorageService,
        ILogger<ImageService> logger,
        IDistributedCache cache)
    {
        _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public Result<string> GetImageUri(string? imagePath)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                _logger.LogWarning("ImagePath is null or empty. Returning default image.");
                return Result<string>.Success("/images/default.jpg");
            }

            var cachedUri = _cache.GetString(imagePath);
            if (cachedUri is not null)
            {
                _logger.LogInformation("Cache hit for imagePath: {ImagePath}", imagePath);
                return Result<string>.Success(cachedUri);
            }

            _logger.LogInformation("Cache miss for imagePath: {ImagePath}. Generating new URI.", imagePath);
            var uriResult = _blobStorageService.GetBlobSasUri(imagePath);
            
            if (!uriResult.Succeeded)
            {
                _logger.LogWarning("Failed to generate SAS URI for {ImagePath}: {Error}", 
                    imagePath, string.Join(", ", uriResult.Errors));
                return Result<string>.Success("/images/default.jpg");
            }

            var imageUri = uriResult.Data;
            _cache.SetString(imagePath, imageUri, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            _logger.LogInformation("Image URI cached for imagePath: {ImagePath}", imagePath);

            return Result<string>.Success(imageUri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate or retrieve image URI for path: {ImagePath}", imagePath);
            return Result<string>.Success("/images/default.jpg");
        }
    }
}