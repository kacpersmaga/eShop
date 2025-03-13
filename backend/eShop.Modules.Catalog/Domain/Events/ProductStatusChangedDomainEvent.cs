using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Shared.Abstractions.Events;

namespace eShop.Modules.Catalog.Domain.Events;

public class ProductStatusChangedDomainEvent : DomainEvent
{
    public Product Product { get; }
    public bool IsAvailable { get; }
    
    public ProductStatusChangedDomainEvent(Product product, bool isAvailable)
    {
        Product = product;
        IsAvailable = isAvailable;
    }
}