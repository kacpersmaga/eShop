namespace eShop.Modules.Catalog.Domain.ValueObjects;

public sealed record ProductDescription
{
    public string Value { get; }

    private ProductDescription(string value)
    {
        Value = value;
    }

    public static ProductDescription Create(string? description)
    {
        if (description == null)
            return new ProductDescription(string.Empty);
            
        if (description.Length > 500)
            throw new ArgumentException("Description cannot exceed 500 characters", nameof(description));
        
        return new ProductDescription(description);
    }

    public static implicit operator string(ProductDescription description) => description.Value;
    public override string ToString() => Value;
}