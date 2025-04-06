using eShop.Shared.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace eShop.Shared.Configuration;

public static class MiddlewareConfiguration
{
    public static WebApplication UseSharedMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        app.UseMiddleware<RateLimitingMiddleware>();
        
        app.UseMiddleware<CsrfProtectionMiddleware>();


        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}