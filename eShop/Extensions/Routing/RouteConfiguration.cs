namespace eShop.Extensions;

public static class RouteConfiguration
{
    public static void ConfigureRoutes(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
}
