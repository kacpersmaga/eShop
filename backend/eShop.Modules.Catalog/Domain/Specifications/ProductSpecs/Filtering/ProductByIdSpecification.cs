using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Specifications.Builders;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Filtering;

public class ProductByIdSpecification : BaseSpecification<Product>
{
    public ProductByIdSpecification(int id)
        : base(p => p.Id == id)
    {
    }
}