using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Repositories;

public static class RepositoryConfiguration
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<eShop.Modules.Catalog.Domain.Repositories.IProductRepository, 
            eShop.Infrastructure.Repositories.Catalog.ProductRepository>();
        
        
        return services;
    }
}