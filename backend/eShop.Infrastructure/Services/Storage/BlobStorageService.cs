using Azure.Storage.Sas;
using eShop.Shared.Abstractions.Interfaces.Storage;
using eShop.Shared.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Services.Storage;

public class BlobStorageService : IBlobStorageService
{
    private readonly IBlobStorageServiceWrapper _blobWrapper;
    private readonly string _containerName;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IBlobStorageServiceWrapper blobWrapper,
                              IConfiguration configuration,
                              ILogger<BlobStorageService> logger)
    {
        _blobWrapper = blobWrapper ?? throw new ArgumentNullException(nameof(blobWrapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "images";
    }

    public async Task<Result<string>> UploadFileAsync(IFormFile file)
    {
        try
        {
            if (file == null)
            {
                return Result<string>.Failure("File cannot be null.");
            }

            if (file.Length == 0)
            {
                return Result<string>.Failure("File cannot be empty.");
            }

            var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!validImageTypes.Contains(file.ContentType))
                return Result<string>.Failure("Only image files are allowed.");

            var blobName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await _blobWrapper.UploadBlobAsync(_containerName, blobName, stream, file.ContentType);
            }

            _logger.LogInformation("File uploaded successfully: {BlobName}", blobName);
            return Result<string>.Success(blobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", file?.FileName);
            return Result<string>.Failure($"Error uploading file: {ex.Message}");
        }
    }

    public async Task<Result> DeleteFileAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Failure("File path cannot be null or empty.");
            }
            
            string blobName = Path.GetFileName(filePath);

            await _blobWrapper.DeleteBlobAsync(_containerName, blobName);
            _logger.LogInformation("File deleted successfully: {BlobName}", blobName);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FilePath}", filePath);
            return Result.Failure($"Error deleting file: {ex.Message}");
        }
    }

    public Result<string> GetBlobSasUri(string blobName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                return Result<string>.Failure("Blob name cannot be null or empty.");
            }

            var sasUri = _blobWrapper.GenerateBlobSasUri(_containerName, blobName, BlobSasPermissions.Read, TimeSpan.FromHours(1));
            _logger.LogInformation("Generated SAS URI for blob: {BlobName}", blobName);

            return Result<string>.Success(sasUri);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating SAS URI for blob {BlobName}", blobName);
            return Result<string>.Failure($"Error generating SAS URI: {ex.Message}");
        }
    }
}