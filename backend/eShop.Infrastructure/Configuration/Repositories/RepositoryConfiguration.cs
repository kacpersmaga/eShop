using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Infrastructure.Repositories.Catalog;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Repositories;

public static class RepositoryConfiguration
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}