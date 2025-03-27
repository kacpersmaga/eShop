using eShop.Shared.Abstractions.Primitives;

namespace eShop.Modules.Catalog.Application.Services;

public interface IImageService
{
    Result<string> GetImageUri(string? imagePath);
}