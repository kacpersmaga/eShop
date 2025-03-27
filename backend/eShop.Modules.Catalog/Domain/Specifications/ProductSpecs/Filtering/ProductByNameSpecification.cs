using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Filtering;

public class ProductByNameSpecification : BaseSpecification<Product>
{
    public ProductByNameSpecification(string name)
        : base(p => p.Name.Value == name)
    {
    }
}
