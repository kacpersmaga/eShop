using Azure.Storage.Blobs;
using eShop.Services.Implementations;
using eShop.Services.Interfaces;

namespace eShop.Extensions.Storage;

public static class BlobStorageConfiguration
{
    public static IServiceCollection ConfigureBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var blobConnectionString = configuration["AzureBlobStorage:ConnectionString"]
                                   ?? throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");

        services.AddSingleton(new BlobServiceClient(blobConnectionString));
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IBlobStorageServiceWrapper, BlobStorageServiceWrapper>();

        return services;
    }
}