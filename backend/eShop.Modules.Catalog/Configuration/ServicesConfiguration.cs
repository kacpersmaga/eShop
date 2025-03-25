﻿using eShop.Modules.Catalog.Application.Mapping;
using eShop.Modules.Catalog.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Modules.Catalog.Configuration;

public static class ServicesConfiguration
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        
        services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);
        
        return services;
    }
}