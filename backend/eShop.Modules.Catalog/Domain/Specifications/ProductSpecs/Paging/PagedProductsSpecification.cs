using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Paging;

public class PagedProductsSpecification : BaseSpecification<Product>
{
    public PagedProductsSpecification(int pageNumber, int pageSize)
        : base(p => p.IsAvailable)
    {
        ApplyPaging((pageNumber - 1) * pageSize, pageSize);
        ApplyOrderBy(p => p.Name.Value);
    }
}