using eShop.Extensions;
using eShop.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.ConfigureSerilog();

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.ConfigureDatabase(builder.Configuration);
builder.Services.ConfigureBlobStorage(builder.Configuration);

// Register application services with scoped lifetime
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IImageService, ImageService>();

// Add AutoMapper for object-to-object mapping
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add in-memory caching for storing temporary data
builder.Services.AddMemoryCache();

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