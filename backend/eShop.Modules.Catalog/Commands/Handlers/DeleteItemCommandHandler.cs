using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Abstractions.Interfaces.Storage;
using eShop.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Commands.Handlers;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, Result<string>>
{
    private readonly IItemService _itemService;
    private readonly IBlobStorageService _blobService;
    private readonly ILogger<DeleteItemCommandHandler> _logger;

    public DeleteItemCommandHandler(
        IItemService itemService,
        IBlobStorageService blobService,
        ILogger<DeleteItemCommandHandler> logger)
    {
        _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<string>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var itemResult = await _itemService.GetItemById(request.ItemId);
            if (!itemResult.Succeeded)
            {
                return Result<string>.Failure(itemResult.Errors);
            }

            var item = itemResult.Data;
            if (item == null)
            {
                return Result<string>.Failure($"Item with ID {request.ItemId} not found.");
            }
            
            if (!string.IsNullOrEmpty(item.ImagePath) && !item.ImagePath.Contains("default"))
            {
                var deleteResult = await _blobService.DeleteFileAsync(item.ImagePath);
                if (!deleteResult.Succeeded)
                {
                    _logger.LogWarning("Failed to delete image: {Errors}", string.Join(", ", deleteResult.Errors));
                }
            }

            var result = await _itemService.DeleteItem(request.ItemId);
            if (!result.Succeeded)
            {
                return Result<string>.Failure(result.Errors);
            }
            
            _logger.LogInformation("Item with ID {Id} deleted successfully.", request.ItemId);

            return Result<string>.Success($"Item deleted successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete item with ID {ItemId}", request.ItemId);
            return Result<string>.Failure($"Failed to delete item: {ex.Message}");
        }
    }
}