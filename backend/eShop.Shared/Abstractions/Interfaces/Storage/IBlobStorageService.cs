using eShop.Shared.Abstractions.Primitives;
using Microsoft.AspNetCore.Http;

namespace eShop.Shared.Abstractions.Interfaces.Storage;

public interface IBlobStorageService
{
    Task<Result<string>> UploadFileAsync(IFormFile file);
    Task<Result> DeleteFileAsync(string filePath);
    Result<string> GetBlobSasUri(string blobName);
}