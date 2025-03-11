using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using eShop.Shared.Interfaces.Storage;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Services.Storage;

public class BlobStorageServiceWrapper : IBlobStorageServiceWrapper
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobStorageServiceWrapper> _logger;

    public BlobStorageServiceWrapper(BlobServiceClient blobServiceClient, ILogger<BlobStorageServiceWrapper> logger)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task UploadBlobAsync(string containerName, string blobName, Stream content, string contentType)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading blob '{BlobName}' to container '{ContainerName}'", blobName, containerName);
            throw new IOException($"Error uploading blob '{blobName}' to container '{containerName}'", ex);
        }
    }

    public async Task DeleteBlobAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                await blobClient.DeleteAsync();
                _logger.LogInformation("Blob '{BlobName}' deleted from container '{ContainerName}'", blobName, containerName);
            }
            else
            {
                _logger.LogWarning("Blob '{BlobName}' not found in container '{ContainerName}' for deletion", blobName, containerName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting blob '{BlobName}' from container '{ContainerName}'", blobName, containerName);
            throw new IOException($"Error deleting blob '{blobName}' from container '{containerName}'", ex);
        }
    }

    public string GenerateBlobSasUri(string containerName, string blobName, BlobSasPermissions permissions, TimeSpan expiry)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!blobClient.Exists())
            {
                throw new FileNotFoundException($"Blob '{blobName}' not found.");
            }

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(expiry)
            };

            sasBuilder.SetPermissions(permissions);

            var sasUri = blobClient.GenerateSasUri(sasBuilder).ToString();

            return sasUri.Replace("http://azurite:10000", "http://localhost:10000");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating SAS URI for blob '{BlobName}' in container '{ContainerName}'", blobName, containerName);
            throw new IOException($"Error generating SAS URI for blob '{blobName}' in container '{containerName}'", ex);
        }
    }
}