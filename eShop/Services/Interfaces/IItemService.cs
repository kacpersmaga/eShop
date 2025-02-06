using eShop.Models.Domain;

namespace eShop.Services.Interfaces;

public interface IItemService
{
    Task<IEnumerable<ShopItem>> GetAllItems();
    Task AddItem(ShopItem item);
}