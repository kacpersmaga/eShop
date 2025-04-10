using eShop.Shared.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.Infrastructure.Configuration.Validation;

public static class ValidationConfiguration
{
    public static IServiceCollection ValidateConfiguration(
        this IServiceCollection services, 
        IConfiguration configuration, 
        IWebHostEnvironment environment)
    {
        if (EnvironmentHelpers.IsTestEnvironment(environment))
        {
            return services;
        }
        
        ValidateConnectionStrings(configuration);
        
        if (!environment.IsDevelopment() && !environment.IsStaging())
        {
            ValidateProductionSettings(configuration);
        }
        
        return services;
    }
    
    private static void ValidateConnectionStrings(IConfiguration configuration)
    {
        var requiredConnectionStrings = new[] { "DefaultConnection", "Redis" };
        foreach (var connectionStringName in requiredConnectionStrings)
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString(connectionStringName)))
            {
                throw new InvalidOperationException($"Required connection string '{connectionStringName}' is not configured.");
            }
        }
    }
    
    private static void ValidateProductionSettings(IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration["AzureBlobStorage:ConnectionString"]))
        {
            throw new InvalidOperationException("Azure Blob Storage connection string is required in production environments.");
        }
        
        if (string.IsNullOrWhiteSpace(configuration["FrontendOrigin"]))
        {
            throw new InvalidOperationException("FrontendOrigin must be explicitly configured in production environments.");
        }
        
        if (string.IsNullOrWhiteSpace(configuration["CsrfSettings:HeaderName"]))
        {
            throw new InvalidOperationException("CSRF header name must be configured in production environments.");
        }
    }
}