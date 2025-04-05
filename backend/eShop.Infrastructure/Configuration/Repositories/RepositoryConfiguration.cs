using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Infrastructure.Repositories.Catalog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eShop.Shared.Abstractions.Interfaces.Persistence;

namespace eShop.Infrastructure.Configuration.Repositories;

public static class RepositoryConfiguration
{
    private const string UseCachingKey = "UseCaching";
    private const bool DefaultUseCachingValue = false;
    
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        bool useCaching = configuration.GetValue(UseCachingKey, DefaultUseCachingValue);
        
        services.AddScoped<ProductRepository>();
        services.AddScoped<CachedProductRepository>();
        
        services.AddScoped<IProductRepository>(sp => 
            useCaching
                ? sp.GetRequiredService<CachedProductRepository>()
                : sp.GetRequiredService<ProductRepository>());
        
        if (useCaching)
        {
            services.AddScoped<ICacheInvalidator>(sp => 
                sp.GetRequiredService<CachedProductRepository>());
        }

        return services;
    }
}