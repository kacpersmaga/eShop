using eShop.Shared.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eShop.Shared.Configuration;

public static class SharedServicesConfiguration
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
            .ConfigureApiBehavior()
            .Configure<RateLimitingSettings>(configuration.GetSection("RateLimiting"))
            .AddMemoryCache()
            .Configure<CsrfSettings>(configuration.GetSection("CsrfSettings"))
            .AddAntiforgery(options =>
            {
                options.HeaderName = configuration["CsrfSettings:HeaderName"] ?? "X-CSRF-TOKEN";
                options.Cookie.HttpOnly = false;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy =
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                        ? CookieSecurePolicy.SameAsRequest
                        : CookieSecurePolicy.Always;
            })
            .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
    
    private static IServiceCollection ConfigureApiBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ApiBehaviorOptions>>();
                logger.LogWarning("Validation failed: {Errors}", string.Join("; ",
                    context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

                return new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
            };
        });
        
        return services;
    }
}