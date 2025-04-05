using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Search.ByPriceRnage;

public class GetProductsByPriceRangeQueryHandler : IRequestHandler<GetProductsByPriceRangeQuery, Result<PagedProductsDto>>
{
    private readonly IProductService _productService;
    private readonly ILogger<GetProductsByPriceRangeQueryHandler> _logger;

    public GetProductsByPriceRangeQueryHandler(
        IProductService productService,
        ILogger<GetProductsByPriceRangeQueryHandler> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<PagedProductsDto>> Handle(GetProductsByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetProductsByPriceRangeQuery for range: {MinPrice} to {MaxPrice}", 
                request.MinPrice, request.MaxPrice);
            
            var result = await _productService.GetProductsByPriceRangeAsync(request.MinPrice, request.MaxPrice);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to fetch products by price range: {Errors}", string.Join(", ", result.Errors));
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
            
            _logger.LogInformation("Successfully fetched {Count} products in price range {MinPrice} to {MaxPrice}.", 
                products.Count, request.MinPrice, request.MaxPrice);
            
            return Result<PagedProductsDto>.Success(paged);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products in price range {MinPrice} to {MaxPrice}", 
                request.MinPrice, request.MaxPrice);
            return Result<PagedProductsDto>.Failure($"Failed to retrieve products by price range: {ex.Message}");
        }
    }
}