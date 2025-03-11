namespace eShop.Modules.Catalog.Domain.Entities;

public class ShopItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? ImagePath { get; set; }
    public string Category { get; set; } = "Uncategorized";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}