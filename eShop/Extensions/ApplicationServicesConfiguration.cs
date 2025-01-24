using eShop.Services;

namespace eShop.Extensions;

public static class ApplicationServicesConfiguration
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IItemService, ItemService>();
        return services;
    }
}