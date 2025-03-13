namespace eShop.Shared.Abstractions.Events;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
}