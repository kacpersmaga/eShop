using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Searching;

public class ProductBySearchTermSpecification : BaseSpecification<Product>
{
    public ProductBySearchTermSpecification(string searchTerm)
        : base(p => 
            (p.Name.Value.Contains(searchTerm) || 
             p.Description.Value.Contains(searchTerm) || 
             p.Category.Value.Contains(searchTerm)) && 
            p.IsAvailable)
    {
    }
}