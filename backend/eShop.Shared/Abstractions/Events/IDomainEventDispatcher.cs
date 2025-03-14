namespace eShop.Shared.Abstractions.Events;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<IDomainEvent> events);
}