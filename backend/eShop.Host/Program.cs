using System.Net;
using eShop.Infrastructure.Configuration;
using eShop.Infrastructure.Configuration.Database;
using eShop.Infrastructure.Configuration.Swagger;
using eShop.Modules.Catalog;
using eShop.Modules.Catalog.Api;
using eShop.Shared.Configuration;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.ConfigureEnvironment<eShop.Host.Program>();
    builder.Host.ConfigureSerilog();
    
    var caddyProxyIp = builder.Configuration["CADDY_PROXY_IP"];

    if (!string.IsNullOrWhiteSpace(caddyProxyIp) && IPAddress.TryParse(caddyProxyIp, out var proxyIp))
    {
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
            options.KnownProxies.Add(proxyIp);
        });

    }

    builder.Services.AddControllers().AddApplicationPart(typeof(CatalogController).Assembly);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddAuthorization();

    builder.Services.AddSharedServices(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
    builder.Services.AddCatalogModule(builder.Configuration);

    var app = builder.Build();

    if (!EnvironmentHelpers.IsTestEnvironment(builder.Environment))
    {
        app.EnsureDatabaseExists();
        app.ApplyDatabaseMigrations();
    }

    
    app.UseForwardedHeaders();
    
    app.UseCorsPolicy(); 
    app.UseSharedMiddlewares();
    app.UseSwaggerDocs(builder.Environment);

    app.MapControllers();

    app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException && ex.Source != "Microsoft.EntityFrameworkCore.Design")
{
    Log.Fatal(ex, "Application startup failed");
    Environment.Exit(1);
}
finally
{
    Log.CloseAndFlush();
}

namespace eShop.Host
{
    public partial class Program { }
}