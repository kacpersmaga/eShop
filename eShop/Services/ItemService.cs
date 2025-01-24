using eShop.Data;
using eShop.Models;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services;

public class ItemService : IItemService
{
    private readonly ApplicationDbContext _context;

    public ItemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ShopItem>> GetAllItems()
    {
        return await _context.ShopItems.AsNoTracking().ToListAsync();
    }

    public async Task AddItem(ShopItem item)
    {
        await _context.ShopItems.AddAsync(item);
        await _context.SaveChangesAsync();
    }
}