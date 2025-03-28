using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Infrastructure.Repositories.Catalog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Repositories;

public static class RepositoryConfiguration
{
    private const string UseCachingKey = "UseCaching";
    private const bool DefaultUseCachingValue = true;
    
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        bool useCaching = configuration.GetValue(UseCachingKey, DefaultUseCachingValue);

        services.AddScoped<IProductRepository>(serviceProvider => 
            useCaching
                ? serviceProvider.GetRequiredService<CachedProductRepository>()
                : serviceProvider.GetRequiredService<ProductRepository>());
        
        services.AddScoped<ProductRepository>();
        services.AddScoped<CachedProductRepository>();

        return services;
    }
}