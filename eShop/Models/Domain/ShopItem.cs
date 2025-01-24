using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace eShop.Models;
public class ShopItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Precision(10, 2)] // Precision supports values up to 99,999,999.99
    [Range(0.01, 100000, ErrorMessage = "Price must be between $0.01 and $100,000.")]
    public decimal Price { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? Description { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
    public string Category { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Image path cannot exceed 200 characters.")]
    public string? ImagePath { get; set; }
}