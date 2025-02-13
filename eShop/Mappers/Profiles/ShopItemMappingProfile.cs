using AutoMapper;
using eShop.Models.Domain;
using eShop.Models.Dtos;
using eShop.Services.Interfaces;

namespace eShop.Mappers.Profiles;

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

public class ImageUriResolver(IImageService imageService) : IValueResolver<ShopItem, ShopItemViewModel, string?>
{
    public string? Resolve(ShopItem source, ShopItemViewModel destination, string? destMember, ResolutionContext context)
    {
        return imageService.GetImageUri(source.ImagePath ?? "default.jpg");
    }
}