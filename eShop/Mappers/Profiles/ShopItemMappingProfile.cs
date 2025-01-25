using AutoMapper;
using eShop.Models;

namespace eShop.Mappers;

public class ShopItemMappingProfile : Profile
{
    public ShopItemMappingProfile()
    {
        CreateMap<ShopItemFormModel, ShopItem>();
        
        CreateMap<ShopItem, ShopItemFormModel>();
    }
}