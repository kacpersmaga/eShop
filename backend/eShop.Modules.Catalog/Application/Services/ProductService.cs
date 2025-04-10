using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Domain.Aggregates;
using eShop.Modules.Catalog.Domain.Exceptions;
using eShop.Modules.Catalog.Domain.Repositories;
using eShop.Modules.Catalog.Domain.Specifications;
using eShop.Modules.Catalog.Domain.Specifications.Core;
using eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Availability;
using eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Filtering;
using eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Price;
using eShop.Modules.Catalog.Domain.Specifications.ProductSpecs.Searching;
using eShop.Shared.Abstractions.Interfaces.Persistence;
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
            var spec = new AvailableProductsSpecification();
            var products = await _productRepository.ListAsync(spec);
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
            var spec = new ProductByIdSpecification(id);
            var product = await _productRepository.GetBySpecAsync(spec);
            
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
            var spec = new ProductByCategorySpecification(category, true);
            var products = await _productRepository.ListAsync(spec);
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

    public async Task<Result<List<ProductDto>>> GetProductsBySearchAsync(string searchTerm)
    {
        try
        {
            _logger.LogInformation("Searching products with term {SearchTerm}", searchTerm);
            var spec = new ProductBySearchTermSpecification(searchTerm);
            var products = await _productRepository.ListAsync(spec);
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            _logger.LogInformation("Found {Count} products matching search term {SearchTerm}", productDtos.Count, searchTerm);
            return Result<List<ProductDto>>.Success(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with term {SearchTerm}", searchTerm);
            return Result<List<ProductDto>>.Failure("Failed to search products.");
        }
    }

    public async Task<Result<List<ProductDto>>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        try
        {
            _logger.LogInformation("Retrieving products in price range {MinPrice} to {MaxPrice}", minPrice, maxPrice);
            var spec = new ProductsByPriceRangeSpecification(minPrice, maxPrice);
            var products = await _productRepository.ListAsync(spec);
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            _logger.LogInformation("Retrieved {Count} products in price range", productDtos.Count);
            return Result<List<ProductDto>>.Success(productDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products in price range {MinPrice} to {MaxPrice}", minPrice, maxPrice);
            return Result<List<ProductDto>>.Failure("Failed to retrieve products by price range.");
        }
    }

    public async Task<Result<PagedProductsDto>> GetPagedProductsAsync(int pageNumber, int pageSize, string? category = null, string? sortBy = null, bool ascending = true)
    {
        try
        {
            _logger.LogInformation("Retrieving paged products (Page {PageNumber}, Size {PageSize})", pageNumber, pageSize);
            
            var builder = new ProductSpecificationBuilder()
                .OnlyAvailable()
                .WithPaging(pageNumber, pageSize);
            
            if (!string.IsNullOrEmpty(category))
            {
                builder.ByCategory(category);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        builder.OrderByName(ascending);
                        break;
                    case "price":
                        builder.OrderByPrice(ascending);
                        break;
                    case "date":
                        builder.OrderByDate(ascending);
                        break;
                    default:
                        builder.OrderByName(ascending);
                        break;
                }
            }
            else
            {
                builder.OrderByName(ascending);
            }
            
            var spec = builder.Build();
            var products = await _productRepository.ListAsync(spec);
            
            var countSpec = new ProductSpecificationBuilder()
                .OnlyAvailable();
                
            if (!string.IsNullOrEmpty(category))
            {
                countSpec.ByCategory(category);
            }
            
            var totalItems = await _productRepository.CountAsync(countSpec.Build());
            
            var productDtos = _mapper.Map<List<ProductDto>>(products);
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var result = new PagedProductsDto
            {
                Items = productDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };

            _logger.LogInformation("Retrieved {Count} products (Page {PageNumber} of {TotalPages})", 
                productDtos.Count, pageNumber, totalPages);
                
            return Result<PagedProductsDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged products");
            return Result<PagedProductsDto>.Failure("Failed to retrieve paged products.");
        }
    }

    public async Task<Result<int>> CreateProductAsync(Product product)
    {
        try
        {
            _logger.LogInformation("Creating new product {ProductName}", product.Name.Value);

            var spec = new ProductByNameSpecification(product.Name.Value);
            var existing = await _productRepository.GetBySpecAsync(spec);
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

            var spec = new ProductByIdSpecification(productId);
            var product = await _productRepository.GetBySpecAsync(spec);
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