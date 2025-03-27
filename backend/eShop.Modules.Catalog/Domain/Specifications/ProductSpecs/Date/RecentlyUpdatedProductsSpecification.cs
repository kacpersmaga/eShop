using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Date;

public class RecentlyUpdatedProductsSpecification : BaseSpecification<Product>
{
    public RecentlyUpdatedProductsSpecification(int days, bool onlyAvailable = true)
        : base(p => p.UpdatedAt >= DateTime.UtcNow.AddDays(-days) && (!onlyAvailable || p.IsAvailable))
    {
        ApplyOrderByDescending(p => p.UpdatedAt);
    }
}