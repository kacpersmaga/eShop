using Azure.Storage.Blobs;
using eShop.Controllers;
using eShop.Data;
using eShop.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


// Add services to the container.
builder.Host.UseSerilog();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IItemService, ItemService>();

var connectionStringName = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? "LocalConnection";
var connectionString = builder.Configuration.GetConnectionString(connectionStringName);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IBlobStorageServiceWrapper, BlobStorageServiceWrapper>();
builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration["AzureBlobStorage:ConnectionString"];
    return new BlobServiceClient(connectionString);
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

app.Run();