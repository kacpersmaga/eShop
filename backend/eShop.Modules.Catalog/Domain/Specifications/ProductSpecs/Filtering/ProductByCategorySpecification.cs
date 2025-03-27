using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Filtering;

public class ProductByCategorySpecification : BaseSpecification<Product>
{
    public ProductByCategorySpecification(string category)
        : base(p => p.Category.Value == category)
    {
    }
    
    public ProductByCategorySpecification(string category, bool onlyAvailable) 
        : base(p => p.Category.Value == category && (!onlyAvailable || p.IsAvailable))
    {
    }
}