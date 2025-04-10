using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

namespace eShop.Host.Configuration;

public static class ForwardedHeadersConfiguration
{
    public static IServiceCollection AddForwardedHeadersIfConfigured(this IServiceCollection services, IConfiguration configuration)
    {
        var caddyProxyIp = configuration["CADDY_PROXY_IP"];

        if (!string.IsNullOrWhiteSpace(caddyProxyIp) && IPAddress.TryParse(caddyProxyIp, out var proxyIp))
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
                options.KnownProxies.Add(proxyIp);
            });
        }

        return services;
    }
}