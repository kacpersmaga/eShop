using Microsoft.AspNetCore.Identity;

namespace eShop.Models.Domain;
public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}