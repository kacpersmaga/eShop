using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using eShop.Models.Settings;

namespace eShop.Extensions.Middlewares;

public class RateLimitingMiddleware(
    RequestDelegate next,
    IMemoryCache cache,
    IOptions<RateLimitingSettings> settings,
    ILogger<RateLimitingMiddleware> logger)
{
    private readonly RateLimitingSettings _settings = settings.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        
        if (path != null && path.StartsWith("/api/"))
        {
            var ipAddress = GetClientIpAddress(context);
            var key = $"{ipAddress}:{path}";
            
            cache.TryGetValue(key, out int hitCount);
            hitCount++;

            if (hitCount > _settings.MaxRequestsPerMinute)
            {
                logger.LogWarning("Rate limit exceeded for IP: {IpAddress}, Path: {Path}", ipAddress, path);
                
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers.Append("Retry-After", "60");
                await context.Response.WriteAsJsonAsync(new { message = "Too many requests. Please try again later." });
                return;
            }
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
            
            cache.Set(key, hitCount, cacheEntryOptions);
        }
        
        await next(context);
    }
    
    private string GetClientIpAddress(HttpContext context)
    {
        string? ip = null;
        var forwardedHeader = context.Request.Headers["X-Forwarded-For"].ToString();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            ip = forwardedHeader.Split(',')[0].Trim();
        }
        if (string.IsNullOrEmpty(ip) && context.Connection.RemoteIpAddress != null)
        {
            ip = context.Connection.RemoteIpAddress.ToString();
        }
        return ip ?? "unknown";
    }
}

