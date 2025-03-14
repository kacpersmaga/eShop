using eShop.Modules.Catalog.Application.Dtos;
using eShop.Shared.Common;

namespace eShop.Modules.Catalog.Application.Services;

public interface IProductService
{
    Task<Result<List<ProductDto>>> GetAllProductsAsync();
    Task<Result<ProductDto>> GetProductByIdAsync(int id);
    Task<Result<List<ProductDto>>> GetProductsByCategoryAsync(string category);
}