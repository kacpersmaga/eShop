using System.IdentityModel.Tokens.Jwt;
using System.Text;
using eShop.Shared.Interfaces.Security;
using eShop.Shared.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace eShop.Infrastructure.Configuration.Security;

public static class JwtAuthenticationConfiguration
{
    public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                          ?? throw new InvalidOperationException("JWT Settings are not configured properly.");

        services.AddAuthentication(options =>
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
            });

        return services;
    }
}
