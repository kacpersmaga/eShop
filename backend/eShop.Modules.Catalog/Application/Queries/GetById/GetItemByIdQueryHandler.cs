using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.GetById;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Result<ProductDto>>
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetItemByIdQueryHandler> _logger;

    public GetItemByIdQueryHandler(
        IProductService productService,
        IMapper mapper,
        ILogger<GetItemByIdQueryHandler> logger)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<ProductDto>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetItemByIdQuery for ID {ItemId}...", request.ItemId);
            
            var result = await _productService.GetProductByIdAsync(request.ItemId);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Product with ID {ItemId} not found or error occurred: {Errors}", 
                    request.ItemId, string.Join(", ", result.Errors));
                return Result<ProductDto>.Failure(result.Errors);
            }
            
            _logger.LogInformation("Successfully retrieved product with ID {ItemId}", request.ItemId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product with ID {ItemId}", request.ItemId);
            return Result<ProductDto>.Failure($"Failed to retrieve product: {ex.Message}");
        }
    }
}