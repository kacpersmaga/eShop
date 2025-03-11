using eShop.Modules.Catalog.Domain.Entities;
using eShop.Shared.Common;

namespace eShop.Modules.Catalog.Application.Services;

public interface IItemService
{
    Task<Result<IEnumerable<ShopItem>>> GetAllItems();
    Task<Result<ShopItem?>> GetItemById(int id);
    Task<Result> AddItem(ShopItem item);
    Task<Result> UpdateItem(ShopItem item);
    Task<Result> DeleteItem(int id);
}