using AutoMapper;
using eShop.Models.Domain;
using eShop.Models.Dtos;

namespace eShop.Mappers.Profiles;

public class ShopItemMappingProfile : Profile
{
    public ShopItemMappingProfile()
    {
        CreateMap<ShopItemFormModel, ShopItem>();
        
        CreateMap<ShopItem, ShopItemFormModel>();
    }
}