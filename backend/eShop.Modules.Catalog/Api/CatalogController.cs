using eShop.Modules.Catalog.Application.Commands;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Queries;
using eShop.Modules.Catalog.Application.Queries.GetAll;
using eShop.Modules.Catalog.Application.Queries.GetById;
using eShop.Modules.Catalog.Application.Queries.Search.ByCategory;
using eShop.Modules.Catalog.Application.Queries.Search.ByPriceRnage;
using eShop.Modules.Catalog.Application.Queries.Search.Paged;
using eShop.Modules.Catalog.Application.Queries.Search.TextSearch;
using eShop.Shared.Abstractions.Primitives;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Api;

/// <summary>
/// API controller for managing product catalog operations including browsing, searching, and product management
/// </summary>
[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CatalogController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CatalogController"/> class
    /// </summary>
    /// <param name="mediator">Mediator service for handling commands and queries</param>
    /// <param name="logger">Logger for controller operations</param>
    /// <exception cref="ArgumentNullException">Thrown if required dependencies are null</exception>
    public CatalogController(
        IMediator mediator,
        ILogger<CatalogController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves products with optional filtering, paging, and sorting
    /// </summary>
    /// <param name="page">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="category">Filter by product category</param>
    /// <param name="sortBy">Property name to sort by</param>
    /// <param name="ascending">Sort direction (true for ascending, false for descending)</param>
    /// <returns>List of products based on provided criteria</returns>
    /// <response code="200">Returns the matching products</response>
    /// <response code="400">If the parameters are invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("products")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProducts([FromQuery] int? page, [FromQuery] int? pageSize, 
        [FromQuery] string? category, [FromQuery] string? sortBy, [FromQuery] bool? ascending)
    {
        try
        {
            if (page.HasValue && pageSize.HasValue)
            {
                _logger.LogInformation("Executing GetPagedProductsQuery...");
                var pagedResult = await _mediator.Send(new GetPagedProductsQuery(
                    page.Value, pageSize.Value, category, sortBy, ascending ?? true));
                
                return pagedResult.Succeeded ? Ok(pagedResult) : BadRequest(pagedResult);
            }
            
            if (!string.IsNullOrEmpty(category))
            {
                _logger.LogInformation("Executing GetItemsByCategoryQuery for category {Category}...", category);
                var categoryResult = await _mediator.Send(new GetItemsByCategoryQuery(category));
                
                return categoryResult.Succeeded ? Ok(categoryResult) : BadRequest(categoryResult);
            }
            
            _logger.LogInformation("Executing GetAllItemsQuery query...");
            var result = await _mediator.Send(new GetAllItemsQuery());
            
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products.");
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    /// <summary>
    /// Searches for products matching the specified search term
    /// </summary>
    /// <param name="term">Search term to match against product name and description</param>
    /// <returns>List of products matching the search criteria</returns>
    /// <response code="200">Returns the matching products</response>
    /// <response code="400">If the search term is empty or invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("products/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchProducts([FromQuery] string term)
    {
        try
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest(Result.Failure("Search term cannot be empty."));
            }
            
            _logger.LogInformation("Executing SearchProductsQuery with term {SearchTerm}...", term);
            var result = await _mediator.Send(new SearchProductsQuery(term));
            
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while searching products.");
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    /// <summary>
    /// Retrieves a specific product by its identifier
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <returns>Product details</returns>
    /// <response code="200">Returns the requested product</response>
    /// <response code="404">If the product is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("products/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductById(int id)
    {
        try
        {
            _logger.LogInformation("Executing GetItemByIdQuery query for ID {ProductId}...", id);
            var result = await _mediator.Send(new GetItemByIdQuery(id));
            
            return result.Succeeded ? Ok(result) : NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product with ID {ProductId}.", id);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    /// <summary>
    /// Retrieves products within the specified price range
    /// </summary>
    /// <param name="minPrice">Minimum price (inclusive)</param>
    /// <param name="maxPrice">Maximum price (inclusive)</param>
    /// <returns>List of products within the specified price range</returns>
    /// <response code="200">Returns products within the price range</response>
    /// <response code="400">If the price range is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("products/price-range")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        try
        {
            if (minPrice < 0 || maxPrice < minPrice)
            {
                return BadRequest(Result.Failure("Invalid price range."));
            }
            
            _logger.LogInformation("Executing GetProductsByPriceRangeQuery with range {MinPrice} to {MaxPrice}...", minPrice, maxPrice);
            var result = await _mediator.Send(new GetProductsByPriceRangeQuery(minPrice, maxPrice));
            
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products by price range.");
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    /// <summary>
    /// Creates a new product
    /// </summary>
    /// <param name="form">Product details and image</param>
    /// <returns>Result indicating success or failure with the created product details</returns>
    /// <response code="200">Returns the newly created product</response>
    /// <response code="400">If the product data is invalid</response>
    [HttpPost("products")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddProduct([FromForm] CreateProductDto form)
    {
        try
        {
            _logger.LogInformation("Executing AddItemCommand...");
            var result = await _mediator.Send(new AddItemCommand(form, form.Image));
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating a new product.");
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    /// <summary>
    /// Updates an existing product
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <param name="form">Updated product details and optional new image</param>
    /// <returns>Result indicating success or failure with the updated product details</returns>
    /// <response code="200">Returns the updated product</response>
    /// <response code="400">If the product data is invalid</response>
    /// <response code="404">If the product is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpPut("products/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto form)
    {
        try
        {
            _logger.LogInformation("Executing UpdateItemCommand for ID {ProductId}...", id);
            var result = await _mediator.Send(new UpdateItemCommand(id, form, form.Image));
            return result.Succeeded ? Ok(result) :
                result.Errors.Any(e => e.Contains("not found")) ? NotFound(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product with ID {ProductId}.", id);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">Product identifier</param>
    /// <returns>Result indicating success or failure</returns>
    /// <response code="200">If the product was successfully deleted</response>
    /// <response code="404">If the product is not found</response>
    /// <response code="400">If the delete operation failed due to business rules</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpDelete("products/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            _logger.LogInformation("Executing DeleteItemCommand command for ID {ProductId}...", id);
            var result = await _mediator.Send(new DeleteItemCommand(id));
            
            return result.Succeeded ? Ok(result) :
                result.Errors.Any(e => e.Contains("not found")) ? NotFound(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product with ID {ProductId}.", id);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }
}