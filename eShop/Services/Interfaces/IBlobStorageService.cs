namespace eShop.Services.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(IFormFile file);
    string GetBlobSasUri(string blobName);
}