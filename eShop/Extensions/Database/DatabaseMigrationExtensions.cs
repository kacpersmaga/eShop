using eShop.Data;
using Microsoft.EntityFrameworkCore;

namespace eShop.Extensions.Database;

public static class DatabaseMigrationExtensions
{
    public static void ApplyDatabaseMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseMigration");

        try
        {
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            if (env.IsEnvironment("Test"))
            {
                logger.LogInformation("Applying migrations for Test environment...");
                dbContext.Database.EnsureDeleted();
                dbContext.Database.Migrate();
                logger.LogInformation("Migrations applied successfully for Test environment.");
            }
            else
            {
                dbContext.Database.Migrate();
                logger.LogInformation("Migrations applied successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }


}