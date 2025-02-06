using Azure.Storage.Sas;

namespace eShop.Services.Interfaces;

public interface IBlobStorageServiceWrapper
{
    Task UploadBlobAsync(string containerName, string blobName, Stream content, string contentType);

    string GenerateBlobSasUri(string containerName, string blobName, BlobSasPermissions permissions,
        TimeSpan expiry);
}