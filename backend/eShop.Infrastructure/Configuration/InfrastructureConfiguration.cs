﻿using eShop.Infrastructure.Configuration.Caching;
using eShop.Infrastructure.Configuration.Database;
using eShop.Infrastructure.Configuration.Events;
using eShop.Infrastructure.Configuration.Repositories;
using eShop.Infrastructure.Configuration.Storage;
using eShop.Infrastructure.Configuration.Swagger;
using eShop.Shared.Configuration;
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
        if (!EnvironmentHelpers.IsTestEnvironment(env))
        {
            services.ConfigureDatabase(configuration, env);
            services.ConfigureBlobStorage(configuration);
        }

        services.ConfigureRedis(configuration);
        services.ConfigureRepositories(configuration);
        services.AddStorageServices();
        services.AddEventDispatching();
        services.AddSwaggerDocs();

        return services;
    }
}