using Azure.Storage.Blobs;
using eShop.Services;

namespace eShop.Extensions;

public static class BlobStorageConfiguration
{
    public static IServiceCollection ConfigureBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var blobConnectionString = configuration["AzureBlobStorage:ConnectionString"]
                                   ?? throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");

        services.AddSingleton(serviceProvider => new BlobServiceClient(blobConnectionString));
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IBlobStorageServiceWrapper, BlobStorageServiceWrapper>();

        return services;
    }
}