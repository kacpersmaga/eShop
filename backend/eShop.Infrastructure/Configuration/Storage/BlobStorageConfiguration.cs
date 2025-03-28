using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Storage;

public static class BlobStorageConfiguration
{
    public static IServiceCollection ConfigureBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var blobConnectionString = configuration["AzureBlobStorage:ConnectionString"]
                                   ?? throw new InvalidOperationException("Azure Blob Storage connection string is not configured in 'AzureBlobStorage:ConnectionString'.");

        services.AddSingleton(new BlobServiceClient(blobConnectionString));

        return services;
    }
}