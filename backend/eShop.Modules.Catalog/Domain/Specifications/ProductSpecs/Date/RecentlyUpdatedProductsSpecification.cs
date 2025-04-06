using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Specifications.Builders;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Date;

public class RecentlyUpdatedProductsSpecification : BaseSpecification<Product>
{
    public RecentlyUpdatedProductsSpecification(int days, bool onlyAvailable = true)
        : base(p => p.UpdatedAt >= DateTime.UtcNow.AddDays(-days) && (!onlyAvailable || p.IsAvailable))
    {
        ApplyOrderByDescending(p => p.UpdatedAt ?? DateTime.MinValue);
    }
}