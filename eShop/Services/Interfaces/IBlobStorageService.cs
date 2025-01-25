namespace eShop.Services;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(IFormFile file);
    string GetBlobSasUri(string blobName);
}