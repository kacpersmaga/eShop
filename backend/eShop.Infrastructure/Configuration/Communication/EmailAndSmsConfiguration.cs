using eShop.Shared.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Communication;

public static class EmailAndSmsConfiguration
{
    public static IServiceCollection ConfigureEmailAndSms(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<TwilioSettings>(configuration.GetSection("TwilioSettings"));

        return services;
    }
}
