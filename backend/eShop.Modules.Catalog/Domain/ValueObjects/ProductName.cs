namespace eShop.Modules.Catalog.Domain.ValueObjects;

public sealed record ProductName
{
    public string Value { get; }

    private ProductName(string value)
    {
        Value = value;
    }

    public static ProductName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));
        
        if (name.Length > 100)
            throw new ArgumentException("Product name cannot exceed 100 characters", nameof(name));
        
        return new ProductName(name);
    }

    public static implicit operator string(ProductName name) => name.Value;
    public override string ToString() => Value;
}