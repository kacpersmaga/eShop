using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Shared.Abstractions.Primitives;

namespace eShop.Modules.Catalog.Application.Services;

public interface IProductService
{
    Task<Result<List<ProductDto>>> GetAllProductsAsync();
    Task<Result<ProductDto>> GetProductByIdAsync(int id);
    Task<Result<List<ProductDto>>> GetProductsByCategoryAsync(string category);
    
    Task<Result<int>> CreateProductAsync(Product product);
    Task<Result<bool>> UpdateProductAsync(Product product);
    Task<Result<bool>> RemoveProductAsync(int productId);
}