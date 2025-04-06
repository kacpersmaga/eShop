using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace eShop.Infrastructure.Configuration.Caching;

public static class RedisConfiguration
{
    public static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "eShop_";
        });
        
        services.AddSingleton<ConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(redisConnectionString));

        return services;
    }
}