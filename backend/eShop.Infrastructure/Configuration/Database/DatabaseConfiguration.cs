using eShop.Infrastructure.Data;
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
        var connectionStringName = env.IsEnvironment("Test") ? "TestConnection" : "DefaultConnection";

        var connectionString = configuration.GetConnectionString(connectionStringName)
                               ?? throw new InvalidOperationException("Database connection string is not configured.");
    
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
