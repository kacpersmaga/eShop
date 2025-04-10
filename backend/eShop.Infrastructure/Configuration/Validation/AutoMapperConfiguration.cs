using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Validation;

public static class AutoMapperConfiguration
{
    public static IServiceCollection AddAutoMapperSupport(this IServiceCollection services)
    {
        return services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}