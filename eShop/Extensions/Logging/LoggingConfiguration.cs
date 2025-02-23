using Serilog;

namespace eShop.Extensions.Logging;

public static class LoggingConfiguration
{
    public static void ConfigureSerilog(this IHostBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
        {
            loggerConfiguration.WriteTo.TestCorrelator();
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.UseSerilog();
    }
}