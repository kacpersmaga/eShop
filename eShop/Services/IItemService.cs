using eShop.Models;

namespace eShop.Services;

public interface IItemService
{
    IEnumerable<ShopItem> GetAllItems();
    void AddItem(ShopItem item);
}