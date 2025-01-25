using eShop.Models;
using Microsoft.EntityFrameworkCore;

namespace eShop.Services;

public interface IApplicationDbContext
{
    DbSet<ShopItem> ShopItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}