using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace eShop.Models;

public class ShopItem
{
    [Key] 
    public int Id { get; set; } 

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Precision(18, 2)]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    public string? Description { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    public string? ImagePath { get; set; }
}