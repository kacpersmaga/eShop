using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace eShop.Shared.Configuration;

public static class EnvironmentConfiguration
{
    private const string LogFilePathFormat = "logs/log-.txt";

    public static WebApplicationBuilder ConfigureEnvironment<T>(this WebApplicationBuilder builder) where T : class
    {
        if (builder.Environment.IsDevelopment() || EnvironmentHelpers.IsTestEnvironment(builder.Environment))
        {
            builder.Configuration.AddUserSecrets<T>();
        }

        return builder;
    }

    public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(LogFilePathFormat, rollingInterval: RollingInterval.Day);

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.UseSerilog();

        return builder;
    }
}