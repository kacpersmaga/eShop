namespace eShop.Modules.Catalog.Domain.ValueObjects;

public sealed record ImagePath
{
    public string? Value { get; }

    private ImagePath(string? value)
    {
        Value = value;
    }

    public static ImagePath Create(string? path)
    {
        return new ImagePath(path);
    }

    public static implicit operator string?(ImagePath path) => path.Value;
    public override string ToString() => Value ?? string.Empty;
}