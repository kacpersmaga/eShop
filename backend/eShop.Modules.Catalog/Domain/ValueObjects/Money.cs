namespace eShop.Modules.Catalog.Domain.ValueObjects;

public sealed record Money
{
    public decimal Value { get; }
    public string Currency { get; }

    private Money(decimal value, string currency)
    {
        if (value < 0)
            throw new ArgumentException("Price cannot be negative", nameof(value));
        
        Value = value;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public static Money Create(decimal value, string currency = "USD")
    {
        return new Money(value, currency);
    }

    public static Money FromDecimal(decimal value)
    {
        return new Money(value, "USD");
    }

    public static implicit operator decimal(Money money) => money.Value;

    public override string ToString() => $"{Value:F2} {Currency}";
}