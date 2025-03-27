using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Search.ByPriceRnage;

public class GetProductsByPriceRangeQueryHandler : IRequestHandler<GetProductsByPriceRangeQuery, Result<List<ProductDto>>>
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

    public async Task<Result<List<ProductDto>>> Handle(GetProductsByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetProductsByPriceRangeQuery for range: {MinPrice} to {MaxPrice}", 
                request.MinPrice, request.MaxPrice);
            
            var result = await _productService.GetProductsByPriceRangeAsync(request.MinPrice, request.MaxPrice);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to fetch products by price range: {Errors}", string.Join(", ", result.Errors));
                return Result<List<ProductDto>>.Failure(result.Errors);
            }
            
            _logger.LogInformation("Successfully fetched {Count} products in price range {MinPrice} to {MaxPrice}.", 
                result.Data?.Count ?? 0, request.MinPrice, request.MaxPrice);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products in price range {MinPrice} to {MaxPrice}", 
                request.MinPrice, request.MaxPrice);
            return Result<List<ProductDto>>.Failure($"Failed to retrieve products by price range: {ex.Message}");
        }
    }
}