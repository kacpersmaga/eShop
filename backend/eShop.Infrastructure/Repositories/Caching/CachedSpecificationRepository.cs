using eShop.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using eShop.Modules.Catalog.Domain.Specifications;
using eShop.Modules.Catalog.Domain.Specifications.Builders;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;

namespace eShop.Infrastructure.Repositories.Caching;

/// <summary>
/// A base repository that implements the specification pattern with caching support.
/// </summary>
/// <typeparam name="T">The entity type this repository works with</typeparam>
public abstract class CachedSpecificationRepository<T> : SpecificationRepository<T> where T : class
{
    protected readonly IDistributedCache _cache;
    private readonly ConnectionMultiplexer _redisConnection;
    private readonly TimeSpan _defaultCacheExpiration;

    protected CachedSpecificationRepository(
        DbContext context, 
        ILogger logger, 
        IDistributedCache cache,
        ConnectionMultiplexer redisConnection,
        TimeSpan? defaultCacheExpiration = null) 
        : base(context, logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        _defaultCacheExpiration = defaultCacheExpiration ?? TimeSpan.FromMinutes(10);
    }

    /// <summary>
    /// Gets a single entity that matches the specification.
    /// Overrides the base implementation to use caching.
    /// </summary>
    public override async Task<T?> GetBySpecAsync(ISpecification<T> spec)
    {
        return await GetBySpecWithCacheAsync(spec);
    }

    /// <summary>
    /// Gets a list of entities that match the specification.
    /// Overrides the base implementation to use caching.
    /// </summary>
    public override async Task<IEnumerable<T>> ListAsync(ISpecification<T> spec)
    {
        return await ListWithCacheAsync(spec);
    }

    /// <summary>
    /// Counts entities that match the specification.
    /// Overrides the base implementation to use caching.
    /// </summary>
    public override async Task<int> CountAsync(ISpecification<T> spec)
    {
        return await CountWithCacheAsync(spec);
    }

    /// <summary>
    /// Gets a single entity that matches the specification with caching support.
    /// </summary>
    protected async Task<T?> GetBySpecWithCacheAsync(
        ISpecification<T> spec, 
        string? cacheKey = null, 
        TimeSpan? cacheTime = null)
    {
        cacheKey ??= GenerateCacheKey(spec, "GetBySpec");
        cacheTime ??= _defaultCacheExpiration;

        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<T>(cachedData);
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
        var result = await base.GetBySpecAsync(spec);

        if (result != null)
        {
            await CacheDataAsync(cacheKey, result, cacheTime.Value);
        }

        return result;
    }

    /// <summary>
    /// Gets a list of entities that match the specification with caching support.
    /// </summary>
    protected async Task<IEnumerable<T>> ListWithCacheAsync(
        ISpecification<T> spec, 
        string? cacheKey = null, 
        TimeSpan? cacheTime = null)
    {
        cacheKey ??= GenerateCacheKey(spec, "ListAsync");
        cacheTime ??= _defaultCacheExpiration;

        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<List<T>>(cachedData) ?? new List<T>();
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
        var result = await base.ListAsync(spec);

        var resultList = result.ToList();
        if (resultList.Any())
        {
            await CacheDataAsync(cacheKey, resultList, cacheTime.Value);
        }

        return resultList;
    }

    /// <summary>
    /// Counts entities that match the specification with caching support.
    /// </summary>
    protected async Task<int> CountWithCacheAsync(
        ISpecification<T> spec, 
        string? cacheKey = null, 
        TimeSpan? cacheTime = null)
    {
        cacheKey ??= GenerateCacheKey(spec, "CountAsync");
        cacheTime ??= _defaultCacheExpiration;

        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return JsonSerializer.Deserialize<int>(cachedData);
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
        var result = await base.CountAsync(spec);

        await CacheDataAsync(cacheKey, result, cacheTime.Value);

        return result;
    }

    /// <summary>
    /// Invalidates all cache entries for this entity type.
    /// </summary>
    public virtual async Task InvalidateCacheAsync(string pattern)
    {
        _logger.LogInformation("Invalidating cache entries with pattern: {Pattern}", pattern);
    
        var server = _redisConnection.GetServer(_redisConnection.GetEndPoints().First());
        var keys = server.Keys(pattern: $"*{pattern}*").ToArray();
    
        if (keys.Any())
        {
            var db = _redisConnection.GetDatabase();
            await db.KeyDeleteAsync(keys);
            _logger.LogInformation("Invalidated {Count} cache entries matching pattern: {Pattern}", keys.Length, pattern);
        }
    }

    /// <summary>
    /// Invalidates specific cache key.
    /// </summary>
    /// <param name="cacheKey">The cache key to invalidate</param>
    public virtual async Task InvalidateCacheKeyAsync(string cacheKey)
    {
        _logger.LogInformation("Invalidating cache key: {CacheKey}", cacheKey);
        await _cache.RemoveAsync(cacheKey);
    }

    /// <summary>
    /// Generates a cache key from a specification.
    /// </summary>
    /// <param name="spec">The specification</param>
    /// <param name="method">The method name</param>
    /// <returns>A cache key string</returns>
    protected virtual string GenerateCacheKey(ISpecification<T> spec, string method)
    {
        var typeName = typeof(T).Name;
        var specTypeName = spec.GetType().Name;
        
        string criteriaHash = "none";
        if (spec.Criteria != null)
        {
            criteriaHash = spec.Criteria.ToString()?.GetHashCode().ToString() ?? "unknown";
        }
        
        string pagingInfo = spec.IsPagingEnabled 
            ? $"_skip{spec.Skip}_take{spec.Take}" 
            : string.Empty;
        
        string orderInfo = spec.OrderBy != null 
            ? "_orderby" 
            : spec.OrderByDescending != null 
                ? "_orderbydesc" 
                : string.Empty;

        return $"{typeName}_{specTypeName}_{method}_{criteriaHash}{pagingInfo}{orderInfo}";
    }

    /// <summary>
    /// Caches data with the specified key and expiration time.
    /// </summary>
    /// <param name="cacheKey">The cache key</param>
    /// <param name="data">The data to cache</param>
    /// <param name="cacheTime">The cache expiration time</param>
    protected virtual async Task CacheDataAsync<TData>(string cacheKey, TData data, TimeSpan cacheTime)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheTime
        };

        var serializedData = JsonSerializer.Serialize(data);
        await _cache.SetStringAsync(cacheKey, serializedData, options);
        _logger.LogInformation("Data cached with key: {CacheKey}, expires in: {CacheTime}", cacheKey, cacheTime);
    }
}