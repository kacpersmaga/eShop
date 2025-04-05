namespace eShop.Shared.Abstractions.Interfaces.Persistence;

public interface ICacheInvalidator
{
    Task InvalidateCacheAsync();
}