using eShop.Modules.Catalog.Infrastructure.Events;
using eShop.Shared.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace eShop.Infrastructure.Configuration.Events;

public static class EventDispatcherConfiguration
{
    public static IServiceCollection AddEventDispatching(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        return services;
    }
}