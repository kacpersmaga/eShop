using MediatR;

namespace eShop.Shared.Abstractions.Events;

public abstract class DomainEvent : IDomainEvent, INotification
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}