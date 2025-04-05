using eShop.Extensions.Middlewares;
using eShop.Shared.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace eShop.Shared.Configuration;

public static class MiddlewareConfiguration
{
    public static WebApplication UseSharedMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        
        // app.UseMiddleware<RateLimitingMiddleware>();
        
        if (!app.Environment.IsDevelopment())
        {
            app.UseMiddleware<CsrfProtectionMiddleware>();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }
}