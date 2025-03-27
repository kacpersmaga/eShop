using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Exceptions;
using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Modules.Catalog.Infrastructure.Persistence;
using eShop.Shared.Abstractions.Primitives;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<ProductDto>>> GetAllProductsAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all products");
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            _logger.LogInformation("Retrieved {Count} products", productDtos.Count);
            return Result<List<ProductDto>>.Success(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all products");
            return Result<List<ProductDto>>.Failure("Failed to retrieve products.");
        }
    }

    public async Task<Result<ProductDto>> GetProductByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Retrieving product with ID {Id}", id);
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
                throw new ProductNotFoundException(id);

            var productDto = _mapper.Map<ProductDto>(product);
            return Result<ProductDto>.Success(productDto);
        }
        catch (ProductDomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception retrieving product {Id}", id);
            return Result<ProductDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving product {Id}", id);
            return Result<ProductDto>.Failure("Unexpected error retrieving product.");
        }
    }

    public async Task<Result<List<ProductDto>>> GetProductsByCategoryAsync(string category)
    {
        try
        {
            _logger.LogInformation("Retrieving products in category {Category}", category);
            var products = await _productRepository.GetByCategoryAsync(category);
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            _logger.LogInformation("Retrieved {Count} products in category {Category}", productDtos.Count, category);
            return Result<List<ProductDto>>.Success(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products in category {Category}", category);
            return Result<List<ProductDto>>.Failure("Failed to retrieve products by category.");
        }
    }

    public async Task<Result<int>> CreateProductAsync(Product product)
    {
        try
        {
            _logger.LogInformation("Creating new product {ProductName}", product.Name.Value);

            var existing = await _productRepository.GetByNameAsync(product.Name.Value);
            if (existing is not null)
                throw new ProductAlreadyExistsException(product.Name.Value);

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product {ProductName} created with ID {ProductId}", product.Name.Value, product.Id);
            return Result<int>.Success(product.Id);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid product data");
            throw new InvalidProductDataException(ex.Message);
        }
        catch (ProductDomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while creating product");
            return Result<int>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating product {ProductName}", product.Name.Value);
            return Result<int>.Failure("Unexpected error occurred while creating the product.");
        }
    }

    public async Task<Result<bool>> UpdateProductAsync(Product product)
    {
        try
        {
            _logger.LogInformation("Updating product with ID {ProductId}", product.Id);

            await _productRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product with ID {ProductId} updated successfully", product.Id);
            return Result<bool>.Success(true);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid product data during update");
            throw new InvalidProductDataException(ex.Message);
        }
        catch (ProductDomainException ex)
        {
            _logger.LogWarning(ex, "Domain error while updating product");
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating product {ProductId}", product.Id);
            return Result<bool>.Failure("Unexpected error occurred while updating the product.");
        }
    }

    public async Task<Result<bool>> RemoveProductAsync(int productId)
    {
        try
        {
            _logger.LogInformation("Removing product with ID {ProductId}", productId);

            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null)
                throw new ProductNotFoundException(productId);

            await _productRepository.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Product with ID {ProductId} removed successfully", productId);
            return Result<bool>.Success(true);
        }
        catch (ProductDomainException ex)
        {
            _logger.LogWarning(ex, "Domain exception while removing product {ProductId}", productId);
            return Result<bool>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error removing product {ProductId}", productId);
            return Result<bool>.Failure("Unexpected error occurred while removing the product.");
        }
    }
}
