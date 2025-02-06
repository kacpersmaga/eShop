using eShop.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace eShop.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ShopItem> ShopItems { get; set; }
}