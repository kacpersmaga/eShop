using System.Text.Json.Serialization;
using eShop.Modules.Catalog.Domain.Events;
using eShop.Modules.Catalog.Domain.ValueObjects;
using eShop.Shared.Abstractions.Domain;

namespace eShop.Modules.Catalog.Domain.Aggregates;

public class Product : AggregateRoot
{
    public override int Id { get; protected set; }
    public ProductName Name { get; private set; } = null!;
    public ProductDescription Description { get; private set; } = null!;
    public Money Price { get; private set; } = null!;
    public bool IsAvailable { get; private set; } = true;
    public ImagePath ImagePath { get; private set; } = null!;
    public ProductCategory Category { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    [JsonConstructor]
    private Product() { }


    private Product(
        ProductName name,
        Money price,
        ProductCategory category,
        ProductDescription description,
        ImagePath imagePath)
    {
        Name = name;
        Price = price;
        Category = category;
        Description = description;
        ImagePath = imagePath;
        CreatedAt = DateTime.UtcNow;
        
        AddDomainEvent(new ProductCreatedDomainEvent(this));
    }
    
    public static Product Create(
        string name,
        decimal price,
        string category,
        string? description = null,
        string? imagePath = null)
    {
        var product = new Product(
            ProductName.Create(name),
            Money.FromDecimal(price),
            ProductCategory.Create(category),
            ProductDescription.Create(description),
            ImagePath.Create(imagePath)
        );
        
        return product;
    }
    
    public void UpdateBasicDetails(string name, string? description, string category)
    {
        Name = ProductName.Create(name);
        Description = ProductDescription.Create(description);
        Category = ProductCategory.Create(category);
        UpdateModifiedDate();
        
        AddDomainEvent(new ProductUpdatedDomainEvent(this));
    }
    
    public void UpdatePrice(decimal price)
    {
        var oldPrice = Price;
        Price = Money.FromDecimal(price);
        UpdateModifiedDate();
        
        AddDomainEvent(new ProductPriceChangedDomainEvent(this, oldPrice, Price));
    }
    
    public void UpdateImage(string? imagePath)
    {
        ImagePath = ImagePath.Create(imagePath);
        UpdateModifiedDate();
    }
    
    public void Enable()
    {
        if (IsAvailable)
            return;
            
        IsAvailable = true;
        UpdateModifiedDate();
        
        AddDomainEvent(new ProductStatusChangedDomainEvent(this, true));
    }
    
    public void Disable()
    {
        if (!IsAvailable)
            return;
            
        IsAvailable = false;
        UpdateModifiedDate();
        
        AddDomainEvent(new ProductStatusChangedDomainEvent(this, false));
    }
    
    private void UpdateModifiedDate()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}