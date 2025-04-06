using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Configuration.Database;

public static class DatabaseInitializer
{
    public static void WaitForDatabaseAndEnsureExists(string connectionString, ILogger logger)
    {
        const int maxRetries = 10;
        const int delayMilliseconds = 3000;
        
        var originalConnectionBuilder = new SqlConnectionStringBuilder(connectionString);
        var databaseName = originalConnectionBuilder.InitialCatalog;
        
        var masterConnectionBuilder = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = "master"
        };
        
        var masterConnectionString = masterConnectionBuilder.ConnectionString;

        for (int i = 1; i <= maxRetries; i++)
        {
            try
            {
                using (var connection = new SqlConnection(masterConnectionString))
                {
                    logger.LogInformation("Attempting to connect to SQL Server (attempt {Attempt}/{MaxRetries})...", i, maxRetries);
                    connection.Open();
                    logger.LogInformation("Connected to SQL Server master database successfully.");
                    
                    using (var command = connection.CreateCommand())
                    {
                        logger.LogInformation("Checking if database '{Database}' exists...", databaseName);
                        command.CommandText = $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}') CREATE DATABASE [{databaseName}]";
                        command.ExecuteNonQuery();
                        logger.LogInformation("Ensured database '{Database}' exists.", databaseName);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to connect to SQL Server (attempt {Attempt}/{MaxRetries})", i, maxRetries);
                
                if (i == maxRetries)
                {
                    logger.LogError(ex, "Failed to connect to SQL Server after {MaxRetries} attempts", maxRetries);
                    throw new Exception($"Failed to connect to SQL Server after {maxRetries} attempts", ex);
                }
                
                logger.LogInformation("Waiting {DelayMs}ms before next attempt...", delayMilliseconds);
                Thread.Sleep(delayMilliseconds);
            }
        }
    }
}