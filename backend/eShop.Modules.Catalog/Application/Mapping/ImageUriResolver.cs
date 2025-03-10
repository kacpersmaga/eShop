using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Modules.Catalog.Domain.Entities;

namespace eShop.Modules.Catalog.Application.Mapping;

public class ImageUriResolver(IImageService imageService) : IValueResolver<ShopItem, ShopItemViewModel, string?>
{
    public string? Resolve(ShopItem source, ShopItemViewModel destination, string? destMember, ResolutionContext context)
    {
        return imageService.GetImageUri(source.ImagePath ?? "default.jpg");
    }
}