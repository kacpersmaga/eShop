using System.ComponentModel.DataAnnotations;

namespace eShop.Models.Dtos;

public class ChangePasswordDto
{
    public required string CurrentPassword { get; set; }

    public required string NewPassword { get; set; }
    
    public required string ConfirmPassword { get; set; }
}