using System.IdentityModel.Tokens.Jwt;
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
using eShop.Validators.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        options.Cookie.HttpOnly = false;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
            ? CookieSecurePolicy.SameAsRequest 
            : CookieSecurePolicy.Always;
    });
    
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
        {
            options.Password.RequiredLength = 12;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            
            options.User.RequireUniqueEmail = true;
            
            options.SignIn.RequireConfirmedEmail = true;
            
            options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<AuthenticatorTokenProvider<ApplicationUser>>(TokenOptions.DefaultAuthenticatorProvider);

    // Add distributed cache for token revocation
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
        options.InstanceName = "eShop_";
    });

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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var revocationService = context.HttpContext.RequestServices
                        .GetRequiredService<ITokenRevocationService>();
                    
                    var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                    
                    if (!string.IsNullOrEmpty(jti) && await revocationService.IsTokenJtiRevokedAsync(jti))
                    {
                        context.Fail("Token has been revoked");
                    }
                },
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["authToken"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                }
                
                
            };
        })
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            options.SlidingExpiration = true;
        });

    // Register application services with scoped lifetime
    builder.Services.AddScoped<IItemService, ItemService>();
    builder.Services.AddScoped<IImageService, ImageService>();
    builder.Services.AddScoped<ITokenRevocationService, TokenRevocationService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IPhoneService, PhoneService>();

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
    
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("TwilioSettings"));

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

