using eShop.Models;

namespace eShop.Mappers;

public static class ShopItemMapper
{
    public static ShopItem MapToShopItem(ShopItemFormModel model, string? imagePath = null)
    {
        return new ShopItem
        {
            Name = model.Name,
            Price = model.Price,
            Description = model.Description,
            Category = model.Category,
            ImagePath = imagePath
        };
    }
}