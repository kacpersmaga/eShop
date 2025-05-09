﻿using eShop.Modules.Catalog.Application.Queries.GetAll;
using eShop.Modules.Catalog.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Modules.Catalog;

public static class CatalogModuleConfiguration
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCatalogServices();
        
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(GetAllItemsQuery).Assembly);
        });
        
        
        return services;
    }
    
}