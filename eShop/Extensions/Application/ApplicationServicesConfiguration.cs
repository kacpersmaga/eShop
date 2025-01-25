using eShop.Services;

namespace eShop.Extensions;

public static class ApplicationServicesConfiguration
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddMemoryCache();
        return services;
    }
}
