using eShop.Infrastructure.Repositories.Catalog;
using eShop.Infrastructure.Services.Storage;
using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Modules.Catalog.Infrastructure.Events;
using eShop.Shared.Abstractions.Events;
using eShop.Shared.Abstractions.Interfaces.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Repositories;

public static class RepositoryConfiguration
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddScoped<IBlobStorageServiceWrapper, BlobStorageServiceWrapper>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        
        
        return services;
    }
}