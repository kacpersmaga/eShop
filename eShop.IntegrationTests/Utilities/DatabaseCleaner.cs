using eShop.Data;

namespace IntegrationTests.Utilities;

public static class DatabaseCleaner
{
    public static void Clean(ApplicationDbContext context)
    {
        context.ShopItems.RemoveRange(context.ShopItems);
        
        context.SaveChanges();
    }
}