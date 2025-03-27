using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Shared.Abstractions.Interfaces.Persistence;
using eShop.Shared.Abstractions.Interfaces.Storage;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Commands.Handlers;

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, Result<string>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobService;
    private readonly ILogger<DeleteItemCommandHandler> _logger;

    public DeleteItemCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobService,
        ILogger<DeleteItemCommandHandler> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<string>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(request.ItemId);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for deletion", request.ItemId);
                return Result<string>.Failure($"Product with ID {request.ItemId} not found.");
            }
            
            if (product.ImagePath.Value != null && !product.ImagePath.Value.Contains("default"))
            {
                var deleteResult = await _blobService.DeleteFileAsync(product.ImagePath.Value);
                if (!deleteResult.Succeeded)
                {
                    _logger.LogWarning("Failed to delete image: {Errors}", string.Join(", ", deleteResult.Errors));
                }
            }
            
            await _productRepository.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Product with ID {Id} deleted successfully.", request.ItemId);
            return Result<string>.Success("Product deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete product with ID {ItemId}", request.ItemId);
            return Result<string>.Failure($"Failed to delete product: {ex.Message}");
        }
    }
}