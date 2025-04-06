using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Specifications.Builders;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Availability;

public class AvailableProductsSpecification : BaseSpecification<Product>
{
    public AvailableProductsSpecification()
        : base(p => p.IsAvailable)
    {
    }
}