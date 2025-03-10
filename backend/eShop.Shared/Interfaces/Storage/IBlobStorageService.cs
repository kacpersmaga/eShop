using Microsoft.AspNetCore.Http;

namespace eShop.Shared.Interfaces.Storage;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(IFormFile file);
    string GetBlobSasUri(string blobName);
}