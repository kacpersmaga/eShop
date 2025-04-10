using eShop.Modules.Catalog.Api;

namespace eShop.Host.Configuration;

public static class HostConfiguration
{
    public static IServiceCollection AddHostConfiguration(this IServiceCollection services)
    {
        services.AddControllers().AddApplicationPart(typeof(CatalogController).Assembly);
        services.AddEndpointsApiExplorer();
        services.AddAuthorization();

        return services;
    }
}