using eShop.Extensions.Database;
using eShop.Extensions.Logging;
using eShop.Extensions.Storage;
using eShop.Services.Implementations;
using eShop.Services.Interfaces;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add user secrets for local development
if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Test"))
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Configure Serilog
builder.Host.ConfigureSerilog();

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.ConfigureDatabase(builder.Configuration, builder.Environment);
builder.Services.ConfigureBlobStorage(builder.Configuration);

// Register application services with scoped lifetime
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IImageService, ImageService>();

// Add AutoMapper for object-to-object mapping
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add in-memory caching for storing temporary data
builder.Services.AddMemoryCache();

// Add FluentValidation for model validation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Apply database migrations on startup
app.ApplyDatabaseMigrations();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Configure endpoints
app.MapDefaultControllerRoute();

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();

namespace eShop
{
    public partial class Program { }
}
