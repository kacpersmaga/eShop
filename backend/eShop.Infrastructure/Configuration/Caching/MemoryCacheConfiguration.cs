using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Caching;

public static class MemoryCacheConfiguration
{
    public static IServiceCollection AddMemoryCaching(this IServiceCollection services)
    {
        return services.AddMemoryCache();
    }
}