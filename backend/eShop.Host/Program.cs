using eShop.Host.Configuration;
using eShop.Infrastructure.Configuration;
using eShop.Infrastructure.Configuration.Cors;
using eShop.Infrastructure.Configuration.Database;
using eShop.Infrastructure.Configuration.Environment;
using eShop.Infrastructure.Configuration.Logging;
using eShop.Infrastructure.Configuration.Swagger;
using eShop.Modules.Catalog;
using eShop.Shared.Helpers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder.AddUserSecretsIfNeeded<eShop.Host.Program>();
    builder.Host.AddSerilogLogging();

    builder.Services
        .AddForwardedHeadersIfConfigured(builder.Configuration)
        .AddHostConfiguration()
        .AddInfrastructure(builder.Configuration, builder.Environment)
        .AddCatalogModule(builder.Configuration);

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