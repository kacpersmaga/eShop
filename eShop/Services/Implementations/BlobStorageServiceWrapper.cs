using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using eShop.Services.Interfaces;

namespace eShop.Services.Implementations;

public class BlobStorageServiceWrapper(BlobServiceClient blobServiceClient) : IBlobStorageServiceWrapper
{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));

    public async Task UploadBlobAsync(string containerName, string blobName, Stream content, string contentType)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });
    }

    public string GenerateBlobSasUri(string containerName, string blobName, BlobSasPermissions permissions,
        TimeSpan expiry)
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
        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
}
