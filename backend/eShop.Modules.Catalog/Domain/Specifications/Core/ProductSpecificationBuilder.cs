using System.Linq.Expressions;
using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Specifications.Builders;
using eShop.Modules.Catalog.Domain.Specifications.ProductSpecs;

namespace eShop.Modules.Catalog.Domain.Specifications.Core;

public class ProductSpecificationBuilder
{
    private readonly List<Expression<Func<Product, bool>>> _criteria = new();
    private Expression<Func<Product, object>>? _orderByExpression;
    private Expression<Func<Product, object>>? _orderByDescendingExpression;
    private int _skip;
    private int _take = int.MaxValue;
    private bool _isPagingEnabled;

    public ProductSpecificationBuilder WithCriteria(Expression<Func<Product, bool>> criteria)
    {
        _criteria.Add(criteria);
        return this;
    }

    public ProductSpecificationBuilder ByCategory(string category)
    {
        _criteria.Add(p => p.Category.Value == category);
        return this;
    }

    public ProductSpecificationBuilder ByName(string name)
    {
        _criteria.Add(p => p.Name.Value == name);
        return this;
    }

    public ProductSpecificationBuilder BySearchTerm(string searchTerm)
    {
        _criteria.Add(p => 
            p.Name.Value.Contains(searchTerm) || 
            p.Description.Value.Contains(searchTerm) ||
            p.Category.Value.Contains(searchTerm));
        return this;
    }

    public ProductSpecificationBuilder ByPriceRange(decimal minPrice, decimal maxPrice)
    {
        _criteria.Add(p => p.Price.Value >= minPrice && p.Price.Value <= maxPrice);
        return this;
    }

    public ProductSpecificationBuilder OnlyAvailable()
    {
        _criteria.Add(p => p.IsAvailable);
        return this;
    }

    public ProductSpecificationBuilder RecentlyUpdated(int days)
    {
        _criteria.Add(p => p.UpdatedAt >= DateTime.UtcNow.AddDays(-days));
        return this;
    }

    public ProductSpecificationBuilder OrderBy(Expression<Func<Product, object>> orderByExpression)
    {
        _orderByExpression = orderByExpression;
        _orderByDescendingExpression = null;
        return this;
    }

    public ProductSpecificationBuilder OrderByDescending(Expression<Func<Product, object>> orderByDescendingExpression)
    {
        _orderByDescendingExpression = orderByDescendingExpression;
        _orderByExpression = null;
        return this;
    }

    public ProductSpecificationBuilder OrderByName(bool ascending = true)
    {
        return ascending 
            ? OrderBy(p => p.Name.Value) 
            : OrderByDescending(p => p.Name.Value);
    }

    public ProductSpecificationBuilder OrderByPrice(bool ascending = true)
    {
        return ascending 
            ? OrderBy(p => p.Price.Value) 
            : OrderByDescending(p => p.Price.Value);
    }

    public ProductSpecificationBuilder OrderByDate(bool ascending = true)
    {
        return ascending 
            ? OrderBy(p => p.CreatedAt) 
            : OrderByDescending(p => p.CreatedAt);
    }

    public ProductSpecificationBuilder WithPaging(int pageNumber, int pageSize)
    {
        _skip = (pageNumber - 1) * pageSize;
        _take = pageSize;
        _isPagingEnabled = true;
        return this;
    }

    public ISpecification<Product> Build()
    {
        Expression<Func<Product, bool>> combinedCriteria = CombineExpressions();
        
        return new CustomProductSpecification(
            combinedCriteria,
            _orderByExpression,
            _orderByDescendingExpression,
            _skip,
            _take,
            _isPagingEnabled);
    }

    private Expression<Func<Product, bool>> CombineExpressions()
    {
        if (_criteria.Count == 0)
        {
            return p => true;
        }

        var firstCriteria = _criteria[0];
        var parameter = firstCriteria.Parameters[0];
        var combinedExpr = firstCriteria.Body;

        for (int i = 1; i < _criteria.Count; i++)
        {
            var currentCriteria = _criteria[i];
            var visitor = new ParameterReplacementVisitor(currentCriteria.Parameters[0], parameter);
            var currentBody = visitor.Visit(currentCriteria.Body);
            combinedExpr = Expression.AndAlso(combinedExpr, currentBody);
        }

        return Expression.Lambda<Func<Product, bool>>(combinedExpr, parameter);
    }

    private class ParameterReplacementVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParam;
        private readonly ParameterExpression _newParam;

        public ParameterReplacementVisitor(ParameterExpression oldParam, ParameterExpression newParam)
        {
            _oldParam = oldParam;
            _newParam = newParam;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return ReferenceEquals(node, _oldParam) ? _newParam : base.VisitParameter(node);
        }
    }
}