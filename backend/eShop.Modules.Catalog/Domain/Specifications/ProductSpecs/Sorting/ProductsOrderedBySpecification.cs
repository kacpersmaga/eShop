using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Sorting;

public class ProductsOrderedBySpecification : BaseSpecification<Product>
{
    public ProductsOrderedBySpecification(string orderByProperty, bool ascending = true)
        : base(p => p.IsAvailable)
    {
        switch (orderByProperty.ToLower())
        {
            case "name":
                if (ascending)
                    ApplyOrderBy(p => p.Name.Value);
                else
                    ApplyOrderByDescending(p => p.Name.Value);
                break;
            case "price":
                if (ascending)
                    ApplyOrderBy(p => p.Price.Value);
                else
                    ApplyOrderByDescending(p => p.Price.Value);
                break;
            case "created":
                if (ascending)
                    ApplyOrderBy(p => p.CreatedAt);
                else
                    ApplyOrderByDescending(p => p.CreatedAt);
                break;
            case "updated":
                if (ascending)
                    ApplyOrderBy(p => p.UpdatedAt);
                else
                    ApplyOrderByDescending(p => p.UpdatedAt);
                break;
            default:
                if (ascending)
                    ApplyOrderBy(p => p.Id);
                else
                    ApplyOrderByDescending(p => p.Id);
                break;
        }
    }
}