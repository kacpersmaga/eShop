using eShop.Models;

namespace eShop.Services;

public interface IItemService
{
    Task<IEnumerable<ShopItem>> GetAllItems();
    Task AddItem(ShopItem item);
}