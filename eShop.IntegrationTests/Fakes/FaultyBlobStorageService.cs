/*using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace IntegrationTests.Fakes;

public class FaultyBlobStorageService : IBlobStorageService
{
    public Task<string> UploadFileAsync(IFormFile file)
    {
        throw new Exception("Simulated exception in FaultyBlobStorageService for UploadFileAsync.");
    }

    public string GetBlobSasUri(string blobName)
    {
        throw new Exception("Simulated exception in FaultyBlobStorageService for GetBlobSasUri.");
    }
}*/