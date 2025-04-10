using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace eShop.Modules.Catalog.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products", "catalog");
            
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.OwnsOne(e => e.Name, name =>
            {
                name.Property(p => p.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(100)
                    .IsRequired();
            });
            
            entity.OwnsOne(e => e.Description, desc =>
            {
                desc.Property(p => p.Value)
                    .HasColumnName("Description")
                    .HasMaxLength(500);
            });
            
            entity.OwnsOne<Money>(e => e.Price, price =>
            {
                price.Property(p => p.Value)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(p => p.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired()
                    .HasDefaultValue("USD");
            });
            
            entity.OwnsOne(e => e.Category, cat =>
            {
                cat.Property(p => p.Value)
                    .HasColumnName("Category")
                    .HasMaxLength(50)
                    .IsRequired();
            });
            
            entity.OwnsOne(e => e.ImagePath, img =>
            {
                img.Property(p => p.Value)
                    .HasColumnName("ImagePath")
                    .HasMaxLength(1000);
            });
            
            entity.Property(e => e.IsAvailable)
                .HasDefaultValue(true)
                .IsRequired();
                
            entity.Property(e => e.CreatedAt)
                .IsRequired();
                
            entity.Property(e => e.UpdatedAt);
            
            entity.Ignore(e => e.DomainEvents);
        });
    }
}