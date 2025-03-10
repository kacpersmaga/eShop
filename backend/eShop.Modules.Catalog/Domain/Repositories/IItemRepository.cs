using eShop.Models.Domain;
using eShop.Modules.Catalog.Domain.Entities;

namespace eShop.Modules.Catalog.Domain.Repositories;

public interface IItemRepository
{
    Task<IEnumerable<ShopItem>> GetAllAsync();
    Task AddAsync(ShopItem item);
    Task SaveChangesAsync();
}