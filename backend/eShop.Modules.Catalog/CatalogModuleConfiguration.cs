
using eShop.Modules.Catalog.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Modules.Catalog;

public static class CatalogModuleConfiguration
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Register module services
        services.AddCatalogServices();
        
        // Any other configuration for the Catalog module
        
        return services;
    }
    
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Endpoint configuration
        return endpoints;
    }
}