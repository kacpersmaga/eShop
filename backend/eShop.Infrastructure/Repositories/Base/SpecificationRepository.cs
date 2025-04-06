using eShop.Modules.Catalog.Domain.Specifications;
using eShop.Modules.Catalog.Domain.Specifications.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eShop.Infrastructure.Repositories.Base;

/// <summary>
/// A base generic repository class that implements specification pattern functionality.
/// This allows derived repositories to easily support querying with specifications.
/// </summary>
/// <typeparam name="T">The entity type this repository works with</typeparam>
public abstract class SpecificationRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly ILogger _logger;

    protected SpecificationRepository(DbContext context, ILogger logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a single entity that matches the specification.
    /// </summary>
    /// <param name="spec">The specification to apply</param>
    /// <returns>The matching entity or null if not found</returns>
    public virtual async Task<T?> GetBySpecAsync(ISpecification<T> spec)
    {
        _logger.LogInformation("Fetching entity by specification");
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets a list of entities that match the specification.
    /// </summary>
    /// <param name="spec">The specification to apply</param>
    /// <returns>A list of matching entities</returns>
    public virtual async Task<IEnumerable<T>> ListAsync(ISpecification<T> spec)
    {
        _logger.LogInformation("Fetching entities list by specification");
        return await ApplySpecification(spec).ToListAsync();
    }

    /// <summary>
    /// Counts entities that match the specification.
    /// </summary>
    /// <param name="spec">The specification to apply</param>
    /// <returns>The count of matching entities</returns>
    public virtual async Task<int> CountAsync(ISpecification<T> spec)
    {
        _logger.LogInformation("Counting entities by specification");
        return await ApplySpecification(spec).CountAsync();
    }

    /// <summary>
    /// Applies the specification to the queryable.
    /// </summary>
    /// <param name="spec">The specification to apply</param>
    /// <returns>An IQueryable with the specification applied</returns>
    protected virtual IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
    }
}