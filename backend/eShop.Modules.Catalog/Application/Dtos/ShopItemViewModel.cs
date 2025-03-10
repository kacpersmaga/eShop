namespace eShop.Modules.Catalog.Application.Dtos;

public class ShopItemViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public required string Category { get; set; }
    public string? ImageUri { get; set; }
}