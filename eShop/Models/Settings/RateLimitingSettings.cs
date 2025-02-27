namespace eShop.Models.Settings;

public class RateLimitingSettings
{
    public int MaxRequestsPerMinute { get; set; } = 10;
}