using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.GetById;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Result<PagedProductsDto>>
{
    private readonly IProductService _productService;
    private readonly ILogger<GetItemByIdQueryHandler> _logger;

    public GetItemByIdQueryHandler(
        IProductService productService,
        ILogger<GetItemByIdQueryHandler> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<PagedProductsDto>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetItemByIdQuery for ID {ItemId}...", request.ItemId);
            
            var result = await _productService.GetProductByIdAsync(request.ItemId);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Product with ID {ItemId} not found or error occurred: {Errors}", 
                    request.ItemId, string.Join(", ", result.Errors));
                return Result<PagedProductsDto>.Failure(result.Errors);
            }

            var product = result.Data;
            if (product != null)
            {
                var paged = new PagedProductsDto
                {
                    Items = new List<ProductDto> { product },
                    PageNumber = 1,
                    PageSize = 1,
                    TotalItems = 1,
                    TotalPages = 1,
                    HasPreviousPage = false,
                    HasNextPage = false
                };
    
                _logger.LogInformation("Successfully retrieved product with ID {ItemId}", request.ItemId);
                return Result<PagedProductsDto>.Success(paged);
            }
            else
            {
                _logger.LogWarning("Product with ID {ItemId} data is null", request.ItemId);
                return Result<PagedProductsDto>.Failure("Product data is null");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product with ID {ItemId}", request.ItemId);
            return Result<PagedProductsDto>.Failure($"Failed to retrieve product: {ex.Message}");
        }
    }
}
