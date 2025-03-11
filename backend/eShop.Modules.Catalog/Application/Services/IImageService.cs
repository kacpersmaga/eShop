using eShop.Shared.Common;

namespace eShop.Modules.Catalog.Application.Services;

public interface IImageService
{
    Result<string> GetImageUri(string? imagePath);
}