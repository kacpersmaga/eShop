using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Application.Mapping;

public class ImageUriResolver : IValueResolver<Product, ShopItemViewModel, string?>
{
    private readonly IImageService _imageService;

    public ImageUriResolver(IImageService imageService)
    {
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    }

    public string? Resolve(Product source, ShopItemViewModel destination, string? destMember, ResolutionContext context)
    {
        var result = _imageService.GetImageUri(source.ImagePath ?? "default.jpg");
        return result.Succeeded ? result.Data : "/images/default.jpg";
    }
}