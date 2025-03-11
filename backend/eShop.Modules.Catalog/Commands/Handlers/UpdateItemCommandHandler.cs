using AutoMapper;
using eShop.Modules.Catalog.Application.Services;
using eShop.Shared.Common;
using eShop.Shared.Interfaces.Storage;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Commands.Handlers;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Result<string>>
{
    private readonly IItemService _itemService;
    private readonly IBlobStorageService _blobService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateItemCommandHandler> _logger;

    public UpdateItemCommandHandler(
        IItemService itemService,
        IBlobStorageService blobService,
        IMapper mapper,
        ILogger<UpdateItemCommandHandler> logger)
    {
        _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<string>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var itemResult = await _itemService.GetItemById(request.ItemId);
            if (!itemResult.Succeeded)
            {
                return Result<string>.Failure(itemResult.Errors);
            }

            var existingItem = itemResult.Data;
            if (existingItem == null)
            {
                return Result<string>.Failure($"Item with ID {request.ItemId} not found.");
            }
            
            _mapper.Map(request.Model, existingItem);
            
            if (request.Image is { Length: > 0 })
            {
                if (!string.IsNullOrEmpty(existingItem.ImagePath) && !existingItem.ImagePath.Contains("default"))
                {
                    var deleteResult = await _blobService.DeleteFileAsync(existingItem.ImagePath);
                    if (!deleteResult.Succeeded)
                    {
                        _logger.LogWarning("Failed to delete old image: {Errors}", string.Join(", ", deleteResult.Errors));
                    }
                }
                
                var uploadResult = await _blobService.UploadFileAsync(request.Image);
                if (!uploadResult.Succeeded)
                {
                    return Result<string>.Failure(uploadResult.Errors);
                }
                
                existingItem.ImagePath = uploadResult.Data;
            }

            var updateResult = await _itemService.UpdateItem(existingItem);
            if (!updateResult.Succeeded)
            {
                return Result<string>.Failure(updateResult.Errors);
            }
            
            _logger.LogInformation("Item '{Name}' with ID {Id} updated successfully.", existingItem.Name, request.ItemId);

            return Result<string>.Success($"Item '{existingItem.Name}' updated successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update item with ID {ItemId}", request.ItemId);
            return Result<string>.Failure($"Failed to update item: {ex.Message}");
        }
    }
}