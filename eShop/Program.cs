using System.Text;
using eShop.Data;
using eShop.Extensions.Database;
using eShop.Extensions.Logging;
using eShop.Extensions.Middlewares;
using eShop.Extensions.Storage;
using eShop.Models.Domain;
using eShop.Models.Settings;
using eShop.Services.Implementations;
using eShop.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
    // Add user secrets for local development
    if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Test"))
    {
        builder.Configuration.AddUserSecrets<Program>();
    }

    // Configure Serilog
    builder.Host.ConfigureSerilog();

    // Add services to the container
    builder.Services.AddControllersWithViews();
    
    if (!builder.Environment.IsEnvironment("Test"))
    {
        builder.Services.ConfigureDatabase(builder.Configuration, builder.Environment);
        builder.Services.ConfigureBlobStorage(builder.Configuration);
    }
    
    // Configure rate limiting
    builder.Services.Configure<RateLimitingSettings>(builder.Configuration.GetSection("RateLimiting"));
    builder.Services.AddMemoryCache();
    
    // Configure CSRF protection
    builder.Services.Configure<CsrfSettings>(builder.Configuration.GetSection("CsrfSettings"));
    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = builder.Configuration["CsrfSettings:HeaderName"] ?? "X-CSRF-TOKEN";
        options.Cookie.Name = builder.Configuration["CsrfSettings:CookieName"] ?? "XSRF-TOKEN";
        options.Cookie.HttpOnly = false;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
    
    // Configure Identity with IdentityDbContext
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    // Configure JWT settings
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
                      ?? throw new InvalidOperationException("JWT Settings are not configured properly.");
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "JwtBearer";
            options.DefaultChallengeScheme = "JwtBearer";
        })
        .AddJwtBearer("JwtBearer", options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };
        });

    // Register application services with scoped lifetime
    builder.Services.AddScoped<IItemService, ItemService>();
    builder.Services.AddScoped<IImageService, ImageService>();

    // Add AutoMapper for object-to-object mapping
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // Add in-memory caching for storing temporary data
    builder.Services.AddMemoryCache();

    // Add FluentValidation for model validation
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // Configure validation error response
    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Validation failed: {Errors}", string.Join("; ",
                context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));

            return new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
        };
    });

    var app = builder.Build();

    // Apply database migrations on startup

    if (!builder.Environment.IsEnvironment("Test"))
    {
        app.ApplyDatabaseMigrations();
    }
    
    // Add middleware to handle exceptions
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    
    // Add rate limiting middleware
    app.UseMiddleware<RateLimitingMiddleware>();
    
    // Add CSRF protection middleware
    app.UseMiddleware<CsrfProtectionMiddleware>();

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    // Configure endpoints
    app.MapDefaultControllerRoute();

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

namespace eShop
{
    public partial class Program { }
}

