using eShop.Models;

namespace eShop.Controllers;

public class ItemService
{
    private readonly List<ShopItem> items = new List<ShopItem>();

    public IEnumerable<ShopItem> GetAllItems()
    {
        return items;
    }

    public void AddItem(ShopItem item)
    {
        items.Add(item);
    }
}