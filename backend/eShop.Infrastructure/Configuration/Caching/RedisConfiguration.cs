using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Caching;

public static class RedisConfiguration
{
    public static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            options.InstanceName = "eShop_";
        });

        return services;
    }
}
