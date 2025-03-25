using eShop.Infrastructure.Configuration.Caching;
using eShop.Infrastructure.Configuration.Database;
using eShop.Infrastructure.Configuration.Repositories;
using eShop.Infrastructure.Configuration.Security;
using eShop.Infrastructure.Configuration.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.Infrastructure.Configuration;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration, 
        IWebHostEnvironment env)
    {
        if (!env.IsEnvironment("Test"))
        {
            services.ConfigureDatabase(configuration, env);
            services.ConfigureBlobStorage(configuration);
        }

        services.ConfigureRedis(configuration);
        services.ConfigureJwtAuthentication(configuration);
        services.ConfigureRepositories();

        return services;
    }
}
