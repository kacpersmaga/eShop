namespace eShop.Shared.Settings;

public class CsrfSettings
{
    public string HeaderName { get; set; } = "X-CSRF-TOKEN";
    public string CookieName { get; set; } = "XSRF-TOKEN";
}