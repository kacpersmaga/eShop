using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Storage;

public static class BlobStorageConfiguration
{
    public static IServiceCollection ConfigureBlobStorage(this IServiceCollection services, IConfiguration configuration)
    {
        try
        {
            var blobConnectionString = configuration["AzureBlobStorage:ConnectionString"]
                                       ?? throw new InvalidOperationException("Azure Blob Storage connection string is not configured.");

            services.AddSingleton(new BlobServiceClient(blobConnectionString));

            return services;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to configure BlobStorage services.", ex);
        }
    }
}
