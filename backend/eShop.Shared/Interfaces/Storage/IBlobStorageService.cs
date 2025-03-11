using eShop.Shared.Common;
using Microsoft.AspNetCore.Http;

namespace eShop.Shared.Interfaces.Storage;

public interface IBlobStorageService
{
    Task<Result<string>> UploadFileAsync(IFormFile file);
    Task<Result> DeleteFileAsync(string filePath);
    Result<string> GetBlobSasUri(string blobName);
}