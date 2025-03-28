using eShop.Modules.Catalog.Infrastructure.Persistence;
using eShop.Shared.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.Infrastructure.Configuration.Database;

public static class DatabaseConfiguration
{
    public static IServiceCollection ConfigureDatabase(
        this IServiceCollection services, 
        IConfiguration configuration, 
        IWebHostEnvironment env)
    {
        var connectionStringName = EnvironmentHelpers.IsTestEnvironment(env) ? "TestConnection" : "DefaultConnection";

        var connectionString = configuration.GetConnectionString(connectionStringName)
                               ?? throw new InvalidOperationException($"Database connection string '{connectionStringName}' is not configured.");
    
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}