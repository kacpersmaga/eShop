using eShop.Models;

namespace eShop.Controllers;

public interface IItemService
{
    IEnumerable<ShopItem> GetAllItems();
    void AddItem(ShopItem item);
}