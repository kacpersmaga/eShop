using eShop.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.ConfigureSerilog();

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureBlobStorage(builder.Configuration);
builder.Services.ConfigureApplicationServices();

var app = builder.Build();

// Apply database migrations on startup
app.ApplyDatabaseMigrations();

// Configure middleware
app.ConfigureMiddleware(app.Environment);

// Configure endpoints
app.UseEndpoints(endpoints => endpoints.ConfigureRoutes());

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();