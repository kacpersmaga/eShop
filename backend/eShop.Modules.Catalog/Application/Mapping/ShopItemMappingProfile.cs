using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Application.Mapping;

public class ShopItemMappingProfile : Profile
{
    public ShopItemMappingProfile()
    {
        CreateMap<ShopItemFormModel, Product>();
        CreateMap<Product, ShopItemFormModel>();
        
        CreateMap<Product, ShopItemViewModel>()
            .ForMember(dest => dest.ImageUri, opt => opt.MapFrom<ImageUriResolver>());
    }
}

