namespace eShop.Modules.Catalog.Domain.Entities;
public class ShopItem
{
    public int Id { get; init; }
    public required string Name { get; init; } 
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public required string Category { get; init;}
    public string? ImagePath { get; set; }
}