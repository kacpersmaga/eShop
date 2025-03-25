using eShop.Modules.Catalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Configuration.Database;

public static class DatabaseMigration
{
    public static void ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseMigration");

        try
        {
            var catalogDbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            catalogDbContext.Database.Migrate();
            logger.LogInformation("CatalogDbContext migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the CatalogDbContext database.");
            throw;
        }
    }
}