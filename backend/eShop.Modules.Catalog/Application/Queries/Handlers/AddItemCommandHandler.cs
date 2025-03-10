using AutoMapper;
using eShop.Modules.Catalog.Application.Services;
using eShop.Modules.Catalog.Commands;
using eShop.Modules.Catalog.Domain.Entities;
using eShop.Shared.Common;
using eShop.Shared.Interfaces.Storage;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Queries.Handlers;

public class AddItemCommandHandler : IRequestHandler<AddItemCommand, Result<string>>
{
    private readonly IItemService _itemService;
    private readonly IBlobStorageService _blobService;
    private readonly IMapper _mapper;
    private readonly ILogger<AddItemCommandHandler> _logger;

    public AddItemCommandHandler(
        IItemService itemService,
        IBlobStorageService blobService,
        IMapper mapper,
        ILogger<AddItemCommandHandler> logger)
    {
        _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<string>> Handle(AddItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            string? uploadedPath = null;
            if (request.Image is { Length: > 0 })
            {
                uploadedPath = await _blobService.UploadFileAsync(request.Image);
            }

            var shopItem = _mapper.Map<ShopItem>(request.Model);
            shopItem.ImagePath = uploadedPath;

            await _itemService.AddItem(shopItem);
            _logger.LogInformation("Item '{Name}' added successfully.", request.Model.Name);

            return Result<string>.Success($"Item '{request.Model.Name}' added successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add item {Name}", request.Model.Name);
            return Result<string>.Failure($"Failed to add item: {ex.Message}");
        }
    }
}