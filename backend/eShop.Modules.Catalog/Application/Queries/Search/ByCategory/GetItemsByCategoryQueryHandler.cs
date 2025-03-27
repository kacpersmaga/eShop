using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Search.ByCategory;

public class GetItemsByCategoryQueryHandler : IRequestHandler<GetItemsByCategoryQuery, Result<List<ProductDto>>>
{
    private readonly IProductService _productService;
    private readonly ILogger<GetItemsByCategoryQueryHandler> _logger;

    public GetItemsByCategoryQueryHandler(
        IProductService productService,
        ILogger<GetItemsByCategoryQueryHandler> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<ProductDto>>> Handle(GetItemsByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetItemsByCategoryQuery for category: {Category}", request.Category);
            
            var result = await _productService.GetProductsByCategoryAsync(request.Category);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to fetch products by category: {Errors}", string.Join(", ", result.Errors));
                return Result<List<ProductDto>>.Failure(result.Errors);
            }
            
            _logger.LogInformation("Successfully fetched {Count} products in category {Category}.", 
                result.Data?.Count ?? 0, request.Category);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products in category {Category}", request.Category);
            return Result<List<ProductDto>>.Failure($"Failed to retrieve products by category: {ex.Message}");
        }
    }
}