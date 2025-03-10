using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace eShop.Shared.Configuration;

public static class EnvironmentConfiguration
{
    public static WebApplicationBuilder ConfigureEnvironment<T>(this WebApplicationBuilder builder) where T : class
    {
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Test"))
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
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Test")
        {
            loggerConfiguration.WriteTo.TestCorrelator();
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.UseSerilog();
        
        return builder;
    }
}