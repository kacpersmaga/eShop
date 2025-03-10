using eShop.Infrastructure.Configuration;
using eShop.Infrastructure.Configuration.Database;
using eShop.Modules.Catalog;
using eShop.Shared.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Configure environment and logging
    builder.ConfigureEnvironment<eShop.Host.Program>();
    builder.Host.ConfigureSerilog();

    // Register shared services
    builder.Services.AddSharedServices(builder.Configuration);
    
    // Register infrastructure services
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
    
    // Register module services
    builder.Services.AddCatalogModule(builder.Configuration);
    
    var app = builder.Build();

    // Configure database
    if (!builder.Environment.IsEnvironment("Test"))
    {
        app.ApplyDatabaseMigrations();
    }
    
    // Configure middleware pipeline
    app.UseSharedMiddlewares();
    
    // Configure module endpoints
    // app.MapAuthEndpoints();
    // app.MapCatalogEndpoints();
    // app.MapPaymentEndpoints();
    // app.MapAdminEndpoints();

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