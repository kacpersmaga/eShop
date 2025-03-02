namespace eShop.Models.Dtos;

public class ResetPasswordDto
{
    public required string UserId { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}