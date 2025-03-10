using eShop.Models.Domain;
using eShop.Modules.Catalog.Domain.Entities;

namespace eShop.Modules.Catalog.Application.Services;

public interface IItemService
{
    Task<IEnumerable<ShopItem>> GetAllItems();
    Task AddItem(ShopItem item);
}