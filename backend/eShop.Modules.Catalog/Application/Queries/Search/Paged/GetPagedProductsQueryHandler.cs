using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Search.Paged;

public class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, Result<PagedProductsDto>>
{
    private readonly IProductService _productService;
    private readonly ILogger<GetPagedProductsQueryHandler> _logger;

    public GetPagedProductsQueryHandler(
        IProductService productService,
        ILogger<GetPagedProductsQueryHandler> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<PagedProductsDto>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetPagedProductsQuery (Page: {PageNumber}, Size: {PageSize}, Category: {Category}, SortBy: {SortBy}, Ascending: {Ascending})",
                request.PageNumber, request.PageSize, request.Category ?? "All", request.SortBy ?? "Default", request.Ascending);
            
            var result = await _productService.GetPagedProductsAsync(
                request.PageNumber,
                request.PageSize,
                request.Category,
                request.SortBy,
                request.Ascending);
                
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to retrieve paged products: {Errors}", string.Join(", ", result.Errors));
                return Result<PagedProductsDto>.Failure(result.Errors);
            }
            
            _logger.LogInformation("Successfully retrieved {Count} products (Page {PageNumber} of {TotalPages}).", 
                result.Data?.Items.Count ?? 0, 
                result.Data?.PageNumber ?? 0,
                result.Data?.TotalPages ?? 0);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged products");
            return Result<PagedProductsDto>.Failure($"Failed to retrieve paged products: {ex.Message}");
        }
    }
}