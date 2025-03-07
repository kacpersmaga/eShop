using System.ComponentModel.DataAnnotations;

namespace eShop.Models.Dtos;

public class CompletePasswordChangeDto
{
    [Required]
    public required string CurrentPassword { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public required  string ConfirmNewPassword { get; set; } = string.Empty;
    
    public string? VerificationCode { get; set; }
}