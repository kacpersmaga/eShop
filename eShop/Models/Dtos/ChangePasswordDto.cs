using System.ComponentModel.DataAnnotations;

namespace eShop.Models.Dtos;

public class ChangePasswordDto
{
    [Required]
    public required string CurrentPassword { get; set; }

    [Required]
    public required string NewPassword { get; set; }

    [Required]
    public required string ConfirmPassword { get; set; }
}