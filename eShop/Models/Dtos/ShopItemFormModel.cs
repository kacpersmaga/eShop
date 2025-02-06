using System.ComponentModel.DataAnnotations;

namespace eShop.Models.Dtos;

public record ShopItemFormModel
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, 100000, ErrorMessage = "Price must be between $0.01 and $100,000.")]
    public decimal Price { get; init; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; init; }

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
    public required string Category { get; init; }

    public IFormFile? Image { get; init; }
}
