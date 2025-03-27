using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Search.TextSearch;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, Result<List<ProductDto>>>
{
    private readonly IProductService _productService;
    private readonly ILogger<SearchProductsQueryHandler> _logger;

    public SearchProductsQueryHandler(
        IProductService productService,
        ILogger<SearchProductsQueryHandler> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<ProductDto>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling SearchProductsQuery with term: {SearchTerm}", request.SearchTerm);
            
            var result = await _productService.GetProductsBySearchAsync(request.SearchTerm);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to search products: {Errors}", string.Join(", ", result.Errors));
                return Result<List<ProductDto>>.Failure(result.Errors);
            }
            
            _logger.LogInformation("Successfully found {Count} products matching search term.", result.Data?.Count ?? 0);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term: {SearchTerm}", request.SearchTerm);
            return Result<List<ProductDto>>.Failure($"Failed to search products: {ex.Message}");
        }
    }
}