using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Handlers;

public class GetAllItemsQueryHandler : IRequestHandler<GetAllItemsQuery, Result<List<ShopItemViewModel>>>
{
    private readonly IItemService _itemService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllItemsQueryHandler> _logger;

    public GetAllItemsQueryHandler(
        IItemService itemService,
        IMapper mapper,
        ILogger<GetAllItemsQueryHandler> logger)
    {
        _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<ShopItemViewModel>>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetAllItemsQuery in Catalog module...");
            
            var items = await _itemService.GetAllItems();
            var itemDtos = _mapper.Map<List<ShopItemViewModel>>(items);
            
            _logger.LogInformation("Successfully fetched {Count} items.", itemDtos.Count);
            
            return Result<List<ShopItemViewModel>>.Success(itemDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching items");
            return Result<List<ShopItemViewModel>>.Failure($"Failed to retrieve items: {ex.Message}");
        }
    }
}