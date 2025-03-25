using Microsoft.AspNetCore.Identity;

namespace eShop.Shared.Infrastructure.Identity;
public class ApplicationUser : IdentityUser
{
    public string? TwoFactorType { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}