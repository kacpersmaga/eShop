using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Shared.Configuration;

public static class CorsConfiguration
{
    private const string DefaultCorsPolicy = "Default";
    private const string DefaultFrontendOrigin = "http://localhost:3000";
    private const string FrontendOriginKey = "FrontendOrigin";
    
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(DefaultCorsPolicy, policy =>
            {
                policy.WithOrigins(configuration[FrontendOriginKey] ?? DefaultFrontendOrigin)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        return app.UseCors(DefaultCorsPolicy);
    }
}