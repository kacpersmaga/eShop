using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Availability;

public class AvailableProductsSpecification : BaseSpecification<Product>
{
    public AvailableProductsSpecification()
        : base(p => p.IsAvailable)
    {
    }
}