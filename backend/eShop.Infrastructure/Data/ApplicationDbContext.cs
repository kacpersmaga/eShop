using eShop.Models.Domain;
using eShop.Modules.Catalog.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eShop.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ShopItem> ShopItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<ShopItem>(entity =>
        {
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasPrecision(18, 2);
                
            entity.Property(e => e.Description)
                .HasMaxLength(500);
                
            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.ImagePath)
                .HasMaxLength(200);
        });
    }
}