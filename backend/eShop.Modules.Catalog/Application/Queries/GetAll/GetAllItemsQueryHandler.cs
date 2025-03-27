using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.GetAll;

public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, Result<List<ProductDto>>>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllItemsQueryHandler> _logger;

    public GetAllItemsQueryHandler(
        IProductService productService,
        IMapper mapper,
        ILogger<GetAllItemsQueryHandler> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<ProductDto>>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetAllItemsQuery in Catalog module...");
            
            var result = await _productService.GetAllProductsAsync();
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to fetch products: {Errors}", string.Join(", ", result.Errors));
                return Result<List<ProductDto>>.Failure(result.Errors);
            }
            
            _logger.LogInformation("Successfully fetched {Count} products.", result.Data?.Count ?? 0);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching products");
            return Result<List<ProductDto>>.Failure($"Failed to retrieve products: {ex.Message}");
        }
    }
}