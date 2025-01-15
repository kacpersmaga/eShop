using eShop.Data;
using eShop.Models;

namespace eShop.Services;

public class ItemService : IItemService
{
    private readonly ApplicationDbContext _context;

    public ItemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ShopItem> GetAllItems()
    {
        return _context.ShopItems.ToList();
    }

    public void AddItem(ShopItem item)
    {
        _context.ShopItems.Add(item);
        _context.SaveChanges();
    }
}