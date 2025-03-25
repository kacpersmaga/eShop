using eShop.Infrastructure.Configuration;
using eShop.Infrastructure.Configuration.Database;
using eShop.Modules.Catalog;
using eShop.Shared.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.ConfigureEnvironment<eShop.Host.Program>();
    builder.Host.ConfigureSerilog();
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSharedServices(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
    builder.Services.AddCatalogModule(builder.Configuration);

    var app = builder.Build();

    if (!builder.Environment.IsEnvironment("Test"))
    {
        app.ApplyDatabaseMigrations();
    }
    
    if (builder.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSharedMiddlewares();
    app.MapCatalogEndpoints();

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