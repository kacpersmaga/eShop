using System.ComponentModel.DataAnnotations;

namespace eShop.Models.Dtos;

public class CompleteEmailChangeDto
{
    [Required]
    [EmailAddress]
    public required string NewEmail { get; set; }

    // Optional verification code (required if 2FA is enabled)
    public string? VerificationCode { get; set; }
}