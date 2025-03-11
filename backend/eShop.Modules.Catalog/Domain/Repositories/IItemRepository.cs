using eShop.Modules.Catalog.Domain.Entities;

namespace eShop.Modules.Catalog.Domain.Repositories;

public interface IItemRepository
{
    Task<IEnumerable<ShopItem>> GetAllAsync();
    Task<ShopItem?> GetByIdAsync(int id);
    Task AddAsync(ShopItem item);
    Task UpdateAsync(ShopItem item);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}