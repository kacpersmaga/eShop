using System.Linq.Expressions;
using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Specifications.Builders;

namespace eShop.Modules.Catalog.Domain.Specifications.ProductSpecs;

public class CustomProductSpecification : BaseSpecification<Product>
{
    public CustomProductSpecification(
        Expression<Func<Product, bool>> criteria,
        Expression<Func<Product, object>>? orderByExpression,
        Expression<Func<Product, object>>? orderByDescendingExpression,
        int skip,
        int take,
        bool isPagingEnabled)
        : base(criteria)
    {
        Initialize(orderByExpression, orderByDescendingExpression, skip, take, isPagingEnabled);
    }

    private void Initialize(
        Expression<Func<Product, object>>? orderByExpression,
        Expression<Func<Product, object>>? orderByDescendingExpression,
        int skip,
        int take,
        bool isPagingEnabled)
    {
        if (orderByExpression != null)
        {
            ApplyOrderBy(orderByExpression);
        }
        else if (orderByDescendingExpression != null)
        {
            ApplyOrderByDescending(orderByDescendingExpression);
        }

        if (isPagingEnabled)
        {
            ApplyPaging(skip, take);
        }
    }
}
