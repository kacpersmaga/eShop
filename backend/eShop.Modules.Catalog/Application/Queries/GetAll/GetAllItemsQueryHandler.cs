using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.GetAll;

public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, Result<PagedProductsDto>>
{
    private readonly IProductService _productService;
    private readonly ILogger<GetAllItemsQueryHandler> _logger;

    public GetAllItemsQueryHandler(
        IProductService productService,
        ILogger<GetAllItemsQueryHandler> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<PagedProductsDto>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetAllItemsQuery in Catalog module...");
            
            var listResult = await _productService.GetAllProductsAsync();
            if (!listResult.Succeeded)
            {
                _logger.LogError("Failed to fetch products: {Errors}", string.Join(", ", listResult.Errors));
                return Result<PagedProductsDto>.Failure(listResult.Errors);
            }

            var products = listResult.Data ?? new List<ProductDto>();
            
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

            _logger.LogInformation("Successfully fetched {Count} products.", products.Count);

            return Result<PagedProductsDto>.Success(paged);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
            return Result<PagedProductsDto>.Failure($"Failed to retrieve products: {ex.Message}");
        }
    }
}