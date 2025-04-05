using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Configuration;

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
        
        if (IsTestEnvironment())
        {
            ConfigureTestLogging(loggerConfiguration);
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.UseSerilog();
        
        return builder;
    }
    
    private static bool IsTestEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentHelpers.TestEnvironmentName;
    }
    
    private static void ConfigureTestLogging(LoggerConfiguration loggerConfiguration)
    {
#if DEBUG
        try
        {
            var testCorrelatorType = Type.GetType("Serilog.Sinks.TestCorrelator.TestCorrelatorLoggerConfigurationExtensions, Serilog.Sinks.TestCorrelator");
            if (testCorrelatorType != null)
            {
                var writeToTestCorrelatorMethod = testCorrelatorType.GetMethod(
                    "TestCorrelator", 
                    new[] { typeof(LoggerSinkConfiguration) });
                
                if (writeToTestCorrelatorMethod != null)
                {
                    var sinkConfiguration = typeof(LoggerConfiguration)
                        .GetProperty("WriteTo")?.GetValue(loggerConfiguration) as LoggerSinkConfiguration;
                    
                    writeToTestCorrelatorMethod.Invoke(null, new[] { sinkConfiguration });
                }
            }
        }
        catch (Exception)
        {
            
        }
#endif
    }
}