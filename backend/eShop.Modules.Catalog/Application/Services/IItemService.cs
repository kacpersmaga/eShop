using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Shared.Common;

namespace eShop.Modules.Catalog.Application.Services;

public interface IItemService
{
    Task<Result<IEnumerable<Product>>> GetAllItems();
    Task<Result<Product?>> GetItemById(int id);
    Task<Result> AddItem(Product item);
    Task<Result> UpdateItem(Product item);
    Task<Result> DeleteItem(int id);
}