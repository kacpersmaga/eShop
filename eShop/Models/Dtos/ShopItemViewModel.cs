namespace eShop.Models;

public class ShopItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? ImageUri { get; set; } // Contains the SAS URI
}