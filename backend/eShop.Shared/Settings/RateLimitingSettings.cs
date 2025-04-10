namespace eShop.Shared.Settings;

public class RateLimitingSettings
{
    public int MaxRequestsPerMinute { get; set; } = 30;
}