using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Infrastructure.Repositories.Catalog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Repositories;

public static class RepositoryConfiguration
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        bool useCaching = configuration.GetValue("UseCaching", true);

        if (useCaching)
        {
            services.AddScoped<IProductRepository, CachedProductRepository>();
        }
        else
        {
            services.AddScoped<IProductRepository, ProductRepository>();
        }

        return services;
    }
}