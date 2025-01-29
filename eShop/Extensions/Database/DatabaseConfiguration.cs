using eShop.Data;
using eShop.Services;
using Microsoft.EntityFrameworkCore;

namespace eShop.Extensions;

public static class DatabaseConfiguration
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStringName = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? "LocalConnection";
        var connectionString = configuration.GetConnectionString(connectionStringName)
                               ?? throw new InvalidOperationException("Database connection string is not configured.");

        // Register ApplicationDbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        

        return services;
    }
}