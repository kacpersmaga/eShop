using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.ValueObjects;
using eShop.Shared.Abstractions.Events;

namespace eShop.Modules.Catalog.Domain.Events;

public class ProductPriceChangedDomainEvent : DomainEvent
{
    public Product Product { get; }
    public Money OldPrice { get; }
    public Money NewPrice { get; }
    
    public ProductPriceChangedDomainEvent(Product product, Money oldPrice, Money newPrice)
    {
        Product = product;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}