using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Shared.Abstractions.Events;

namespace eShop.Modules.Catalog.Domain.Events;

public class ProductUpdatedDomainEvent : DomainEvent
{
    public Product Product { get; }
    
    public ProductUpdatedDomainEvent(Product product)
    {
        Product = product;
    }
}