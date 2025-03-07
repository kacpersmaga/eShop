namespace eShop.Models.Settings;

public class JwtSettings
{
    public string Secret { get; set; } = "YourSuperSecretKeyHere";
    public string Issuer { get; set; } = "eShopIssuer";
    public string Audience { get; set; } = "eShopAudience";
    public int ExpirationInMinutes { get; set; } = 15;
}