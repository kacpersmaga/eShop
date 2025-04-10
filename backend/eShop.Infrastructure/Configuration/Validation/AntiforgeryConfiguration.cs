using eShop.Shared.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Validation;

public static class AntiforgeryConfiguration
{
    public static IServiceCollection AddAntiforgeryPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CsrfSettings>(configuration.GetSection("CsrfSettings"));

        return services.AddAntiforgery(options =>
        {
            options.HeaderName = configuration["CsrfSettings:HeaderName"] ?? "X-CSRF-TOKEN";
            options.Cookie.HttpOnly = false;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy =
                System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
        });
    }
}