using eShop.Shared.Abstractions.Primitives;

namespace eShop.Shared.Abstractions.Interfaces.Storage;

public interface IImageService
{
    Result<string> GetImageUri(string? imagePath);
}