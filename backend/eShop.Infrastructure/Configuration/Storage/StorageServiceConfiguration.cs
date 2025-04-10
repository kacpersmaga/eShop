using eShop.Infrastructure.Services.Storage;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Interfaces.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Storage;

public static class StorageServiceConfiguration
{
    public static IServiceCollection AddStorageServices(this IServiceCollection services)
    {
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IBlobStorageServiceWrapper, BlobStorageServiceWrapper>();
        return services;
    }
}