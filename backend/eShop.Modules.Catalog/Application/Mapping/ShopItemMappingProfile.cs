using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Domain.Entities;

namespace eShop.Modules.Catalog.Application.Mapping;

public class ShopItemMappingProfile : Profile
{
    public ShopItemMappingProfile()
    {
        CreateMap<ShopItemFormModel, ShopItem>();
        CreateMap<ShopItem, ShopItemFormModel>();
        
        CreateMap<ShopItem, ShopItemViewModel>()
            .ForMember(dest => dest.ImageUri, opt => opt.MapFrom<ImageUriResolver>());
    }
}

