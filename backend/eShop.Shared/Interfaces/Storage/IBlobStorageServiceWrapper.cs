using Azure.Storage.Sas;

namespace eShop.Shared.Interfaces.Storage;

public interface IBlobStorageServiceWrapper
{
    Task UploadBlobAsync(string containerName, string blobName, Stream content, string contentType);
    Task DeleteBlobAsync(string containerName, string blobName);
    string GenerateBlobSasUri(string containerName, string blobName, BlobSasPermissions permissions, TimeSpan expiry);
}