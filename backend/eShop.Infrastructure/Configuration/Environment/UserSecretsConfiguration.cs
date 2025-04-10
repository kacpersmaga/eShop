using eShop.Shared.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace eShop.Infrastructure.Configuration.Environment;

public static class UserSecretsConfiguration
{
    public static WebApplicationBuilder AddUserSecretsIfNeeded<T>(this WebApplicationBuilder builder) where T : class
    {
        if (builder.Environment.IsDevelopment() || EnvironmentHelpers.IsTestEnvironment(builder.Environment))
        {
            builder.Configuration.AddUserSecrets<T>();
        }

        return builder;
    }
}