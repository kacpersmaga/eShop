using System.ComponentModel.DataAnnotations;

namespace eShop.Models;

public class ShopItem
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    public string? Description { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    public string? ImagePath { get; set; }
}