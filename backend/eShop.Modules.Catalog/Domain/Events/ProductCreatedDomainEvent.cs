using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Shared.Abstractions.Events;

namespace eShop.Modules.Catalog.Domain.Events;

public class ProductCreatedDomainEvent : DomainEvent
{
    public Product Product { get; }

    public ProductCreatedDomainEvent(Product product)
    {
        Product = product;
    }
}