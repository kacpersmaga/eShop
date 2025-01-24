using Serilog;

namespace eShop.Extensions;

public static class LoggingConfiguration
{
    public static void ConfigureSerilog(this IHostBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.UseSerilog();
    }
}