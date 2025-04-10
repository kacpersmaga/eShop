using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Search.ByCategory;

public class GetItemsByCategoryQueryHandler : IRequestHandler<GetItemsByCategoryQuery, Result<PagedProductsDto>>
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

    public async Task<Result<PagedProductsDto>> Handle(GetItemsByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetItemsByCategoryQuery for category: {Category}", request.Category);
            
            var result = await _productService.GetProductsByCategoryAsync(request.Category);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to fetch products by category: {Errors}", string.Join(", ", result.Errors));
                return Result<PagedProductsDto>.Failure(result.Errors);
            }
            
            var products = result.Data ?? new List<ProductDto>();
            
            var paged = new PagedProductsDto
            {
                Items = products,
                PageNumber = 1,
                PageSize = products.Count,
                TotalItems = products.Count,
                TotalPages = 1,
                HasPreviousPage = false,
                HasNextPage = false
            };
            
            _logger.LogInformation("Successfully fetched {Count} products in category {Category}.", 
                products.Count, request.Category);
            
            return Result<PagedProductsDto>.Success(paged);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products in category {Category}", request.Category);
            return Result<PagedProductsDto>.Failure($"Failed to retrieve products by category: {ex.Message}");
        }
    }
}