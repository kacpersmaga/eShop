using Azure.Storage.Sas;
using eShop.Services.Interfaces;

namespace eShop.Services.Implementations;

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

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        try
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), "File cannot be null.");
            }

            if (file.Length == 0)
            {
                throw new ArgumentException("File cannot be empty.", nameof(file));
            }

            var validImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!validImageTypes.Contains(file.ContentType))
                throw new ArgumentException("Only image files are allowed.", nameof(file));

            var blobName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            using (var stream = file.OpenReadStream())
            {
                await _blobWrapper.UploadBlobAsync(_containerName, blobName, stream, file.ContentType);
            }

            _logger.LogInformation("File uploaded successfully: {BlobName}", blobName);
            return blobName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", file?.FileName);
            throw;
        }
    }

    public string GetBlobSasUri(string blobName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("Blob name cannot be null or empty.", nameof(blobName));
            }

            var sasUri = _blobWrapper.GenerateBlobSasUri(_containerName, blobName, BlobSasPermissions.Read, TimeSpan.FromHours(1));
            _logger.LogInformation("Generated SAS URI for blob: {BlobName}", blobName);

            return sasUri;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating SAS URI for blob {BlobName}", blobName);
            throw;
        }
    }
}
