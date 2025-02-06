using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace eShop.Models.Domain;
public class ShopItem
{
    [Key]
    public int Id { get; init; }

    [Required]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public required string Name { get; init; } 

    [Required]
    [Precision(10, 2)]
    [Range(0.01, 100000, ErrorMessage = "Price must be between $0.01 and $100,000.")]
    public decimal Price { get; init; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; init; }

    [Required]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
    public required string Category { get; init; }

    [StringLength(200, ErrorMessage = "Image path cannot exceed 200 characters.")]
    public string? ImagePath { get; set; }
}