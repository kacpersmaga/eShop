using eShop.Modules.Catalog.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Modules.Catalog.Configuration;

public static class ServicesConfiguration
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services)
    {
        // Register all Catalog module services
        services.AddScoped<IItemService, ItemService>();
        
        // Add other service registrations as the module grows
        
        return services;
    }
}