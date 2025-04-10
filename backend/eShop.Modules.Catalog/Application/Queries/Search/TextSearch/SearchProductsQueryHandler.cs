using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Search.TextSearch;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, Result<PagedProductsDto>>
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

    public async Task<Result<PagedProductsDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling SearchProductsQuery with term: {SearchTerm}", request.SearchTerm);
            
            var result = await _productService.GetProductsBySearchAsync(request.SearchTerm);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to search products: {Errors}", string.Join(", ", result.Errors));
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
            
            _logger.LogInformation("Successfully found {Count} products matching search term.", products.Count);
            
            return Result<PagedProductsDto>.Success(paged);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term: {SearchTerm}", request.SearchTerm);
            return Result<PagedProductsDto>.Failure($"Failed to search products: {ex.Message}");
        }
    }
}
