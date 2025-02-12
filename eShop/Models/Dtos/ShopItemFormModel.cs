
namespace eShop.Models.Dtos;

public class ShopItemFormModel
{
    public required string Name { get; init; }
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public required string Category { get; init; }
    public IFormFile? Image { get; init; }
}
