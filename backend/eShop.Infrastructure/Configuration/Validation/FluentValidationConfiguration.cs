using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Validation;

public static class FluentValidationConfiguration
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        return services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
    }
}