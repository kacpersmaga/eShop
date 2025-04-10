using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Specifications.Builders;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Price;

public class ProductsByPriceRangeSpecification : BaseSpecification<Product>
{
    public ProductsByPriceRangeSpecification(decimal minPrice, decimal maxPrice)
        : base(p => p.Price.Value >= minPrice && p.Price.Value <= maxPrice && p.IsAvailable)
    {
    }
}