using eShop.Shared.Abstractions.Domain;
using eShop.Shared.Abstractions.Events;
using eShop.Shared.Abstractions.Interfaces.Persistence;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly CatalogDbContext _dbContext;
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IEnumerable<ICacheInvalidator> _cacheInvalidators;

    public UnitOfWork(
        CatalogDbContext dbContext,
        IDomainEventDispatcher eventDispatcher,
        ILogger<UnitOfWork> logger,
        IEnumerable<ICacheInvalidator> cacheInvalidators = null)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cacheInvalidators = cacheInvalidators ?? Enumerable.Empty<ICacheInvalidator>();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var aggregates = _dbContext.ChangeTracker
                .Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = aggregates
                .SelectMany(a => a.DomainEvents)
                .ToList();
            
            _logger.LogInformation("Collected {Count} domain events before saving changes", domainEvents.Count);
            
            var result = await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved {Count} changes to the database", result);
            
            if (_cacheInvalidators.Any())
            {
                _logger.LogInformation("Invalidating caches after database changes");
                var invalidationTasks = _cacheInvalidators.Select(invalidator => invalidator.InvalidateCacheAsync());
                await Task.WhenAll(invalidationTasks);
            }
            
            if (domainEvents.Any())
            {
                _logger.LogInformation("Dispatching {Count} domain events", domainEvents.Count);
                await _eventDispatcher.DispatchEventsAsync(domainEvents);
                
                foreach (var aggregate in aggregates)
                {
                    aggregate.ClearDomainEvents();
                }
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to the database");
            throw;
        }
    }
}