using eShop.Models;
using Microsoft.EntityFrameworkCore;

namespace eShop.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<ShopItem> ShopItems { get; set; }
}