using eShop.Models.Settings;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Options;

namespace eShop.Extensions.Middlewares;

public class CsrfProtectionMiddleware(RequestDelegate next,
    IAntiforgery antiforgery,
    IOptions<CsrfSettings> settings,
    ILogger<CsrfProtectionMiddleware> logger)
{
    private readonly CsrfSettings _settings = settings.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "GET" && 
            (context.Request.Path.Value?.EndsWith(".html") == true || 
             !context.Request.Path.Value?.Contains('.') == true))
        {
            var tokens = antiforgery.GetAndStoreTokens(context);
            if (tokens.RequestToken != null)
            {
                context.Response.Cookies.Append(
                    _settings.CookieName,
                    tokens.RequestToken,
                    new CookieOptions
                    {
                        HttpOnly = false,
                        SameSite = SameSiteMode.Strict,
                        Secure = context.Request.IsHttps,
                        Path = "/"
                    }
                );
            }
        }
        else if (context.Request.Method != "GET" &&
                 context.Request.Method != "HEAD" &&
                 context.Request.Method != "OPTIONS" &&
                 context.Request.Method != "TRACE")
        {
            try
            {
                await antiforgery.ValidateRequestAsync(context);
            }
            catch (AntiforgeryValidationException ex)
            {
                logger.LogWarning(ex, "CSRF validation failed for {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid or missing CSRF token" });
                return;
            }
        }
        
        await next(context);
    }
}