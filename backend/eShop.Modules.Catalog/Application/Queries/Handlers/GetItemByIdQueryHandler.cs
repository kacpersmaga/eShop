using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Handlers;

public class GetItemByIdQueryHandler : IRequestHandler<GetItemByIdQuery, Result<ShopItemViewModel>>
{
    private readonly IItemService _itemService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetItemByIdQueryHandler> _logger;

    public GetItemByIdQueryHandler(
        IItemService itemService,
        IMapper mapper,
        ILogger<GetItemByIdQueryHandler> logger)
    {
        _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<ShopItemViewModel>> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Handling GetItemByIdQuery for ID {ItemId}...", request.ItemId);
            
            var item = await _itemService.GetItemById(request.ItemId);
            if (item == null)
            {
                return Result<ShopItemViewModel>.Failure($"Item with ID {request.ItemId} not found.");
            }
            
            var itemDto = _mapper.Map<ShopItemViewModel>(item);
            
            return Result<ShopItemViewModel>.Success(itemDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching item with ID {ItemId}", request.ItemId);
            return Result<ShopItemViewModel>.Failure($"Failed to retrieve item: {ex.Message}");
        }
    }
}