namespace eShop.Modules.Catalog.Domain.ValueObjects;

public sealed record ProductCategory
{
    public string Value { get; }

    private ProductCategory(string value)
    {
        Value = value;
    }

    public static ProductCategory Create(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty", nameof(category));
        
        if (category.Length > 50)
            throw new ArgumentException("Category name cannot exceed 50 characters", nameof(category));
        
        return new ProductCategory(category);
    }

    public static implicit operator string(ProductCategory category) => category.Value;
    public override string ToString() => Value;
}
