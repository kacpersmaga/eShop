using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Modules.Catalog.Infrastructure;
using eShop.Shared.Abstractions.Interfaces.Storage;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Commands.Handlers;

public class UpdateItemCommandHandler : IRequestHandler<UpdateItemCommand, Result<string>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobService;
    private readonly ILogger<UpdateItemCommandHandler> _logger;

    public UpdateItemCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobService,
        ILogger<UpdateItemCommandHandler> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<string>> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ItemId);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", request.ItemId);
                return Result<string>.Failure($"Product with ID {request.ItemId} not found.");
            }
            
            if (request.Image is { Length: > 0 })
            {
                if (product.ImagePath.Value != null && !product.ImagePath.Value.Contains("default"))
                {
                    var deleteResult = await _blobService.DeleteFileAsync(product.ImagePath.Value);
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
                
                product.UpdateImage(uploadResult.Data);
            }
            
            product.UpdateBasicDetails(
                request.Model.Name,
                request.Model.Description,
                request.Model.Category
            );
            
            product.UpdatePrice(request.Model.Price);
            
            await _productRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Product with ID {Id} updated successfully", request.ItemId);
            return Result<string>.Success($"Product '{request.Model.Name}' updated successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update product with ID {ItemId}", request.ItemId);
            return Result<string>.Failure($"Failed to update product: {ex.Message}");
        }
    }
}