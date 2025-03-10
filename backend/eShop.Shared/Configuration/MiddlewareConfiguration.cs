using eShop.Shared.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace eShop.Shared.Configuration;

public static class MiddlewareConfiguration
{
    public static WebApplication UseSharedMiddlewares(this WebApplication app)
    {
        // Add middleware to handle exceptions
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        // Add rate limiting middleware
        app.UseMiddleware<RateLimitingMiddleware>();
        
        // Add CSRF protection middleware
        app.UseMiddleware<CsrfProtectionMiddleware>();

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}