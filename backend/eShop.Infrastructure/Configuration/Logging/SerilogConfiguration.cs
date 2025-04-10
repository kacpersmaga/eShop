using Microsoft.Extensions.Hosting;
using Serilog;

namespace eShop.Infrastructure.Configuration.Logging;

public static class SerilogConfiguration
{
    private const string LogFilePathFormat = "logs/log-.txt";

    public static IHostBuilder AddSerilogLogging(this IHostBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(LogFilePathFormat, rollingInterval: RollingInterval.Day);

        Log.Logger = loggerConfiguration.CreateLogger();

        return builder.UseSerilog();
    }
}