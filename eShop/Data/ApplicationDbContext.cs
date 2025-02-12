using eShop.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace eShop.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ShopItem> ShopItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ShopItem>(entity =>
        {
            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            entity.Property(x => x.Price)
                .HasColumnType("decimal(18, 2)")
                .HasPrecision(18, 2);
            
            entity.Property(x => x.Description)
                .HasMaxLength(500);
            
            entity.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(50);
            
            entity.Property(x => x.ImagePath)
                .HasMaxLength(200);
        });
    }
}