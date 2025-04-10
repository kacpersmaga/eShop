using eShop.Shared.Abstractions.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace eShop.Modules.Catalog.Infrastructure.Events;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IServiceProvider serviceProvider,
        ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task DispatchEventsAsync(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            await DispatchEventAsync(@event);
        }
    }

    private async Task DispatchEventAsync(IDomainEvent domainEvent)
    {
        var eventType = domainEvent.GetType();
        var eventTypeName = eventType.Name;

        _logger.LogInformation("Dispatching domain event: {EventType}", eventTypeName);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            
            await mediator.Publish(domainEvent, CancellationToken.None);
            
            _logger.LogInformation("Successfully dispatched domain event: {EventType}", eventTypeName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching domain event: {EventType}", eventTypeName);
            throw;
        }
    }
}