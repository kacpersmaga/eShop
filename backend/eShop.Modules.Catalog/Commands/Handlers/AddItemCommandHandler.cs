using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Commands;
using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Modules.Catalog.Infrastructure;
using eShop.Shared.Abstractions.Interfaces.Storage;
using eShop.Shared.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Commands.Handlers;

public class AddItemCommandHandler : IRequestHandler<AddItemCommand, Result<string>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobService;
    private readonly ILogger<AddItemCommandHandler> _logger;

    public AddItemCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobService,
        ILogger<AddItemCommandHandler> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<string>> Handle(AddItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            string? uploadedPath = null;
            if (request.Image is { Length: > 0 })
            {
                var uploadResult = await _blobService.UploadFileAsync(request.Image);
                if (!uploadResult.Succeeded)
                {
                    return Result<string>.Failure(uploadResult.Errors);
                }
                uploadedPath = uploadResult.Data;
            }
            
            var product = Product.Create(
                name: request.Model.Name,
                price: request.Model.Price,
                category: request.Model.Category,
                description: request.Model.Description,
                imagePath: uploadedPath
            );
            
            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("Product '{Name}' added successfully with ID {Id}", 
                product.Name.Value, product.Id);

            return Result<string>.Success($"Product '{request.Model.Name}' added successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add product {Name}", request.Model?.Name);
            return Result<string>.Failure($"Failed to add product: {ex.Message}");
        }
    }
}