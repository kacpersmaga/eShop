using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Specifications.Builders;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Filtering;

public class ProductByNameSpecification : BaseSpecification<Product>
{
    public ProductByNameSpecification(string name)
        : base(p => p.Name.Value == name)
    {
    }
}
