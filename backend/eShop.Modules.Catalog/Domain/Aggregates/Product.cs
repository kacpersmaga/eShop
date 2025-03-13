using eShop.Modules.Catalog.Domain.ValueObjects;

namespace eShop.Modules.Catalog.Domain.Aggregates;

public class Product
{
    public int Id { get; private set; }
    public ProductName Name { get; private set; }
    public ProductDescription Description { get; private set; }
    public Money Price { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public ImagePath ImagePath { get; private set; }
    public ProductCategory Category { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

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
    }

    public void UpdatePrice(decimal price)
    {
        Price = Money.FromDecimal(price);
        UpdateModifiedDate();
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
    }

    public void Disable()
    {
        if (!IsAvailable)
            return;

        IsAvailable = false;
        UpdateModifiedDate();
    }

    private void UpdateModifiedDate()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}