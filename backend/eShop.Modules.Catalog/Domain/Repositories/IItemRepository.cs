using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Domain.Repositories;

public interface IItemRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product item);
    Task UpdateAsync(Product item);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}