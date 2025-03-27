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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eShop.Modules.Catalog.Api;

[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(
        IMediator mediator,
        ILogger<CatalogController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("products")]
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

    [HttpGet("products/search")]
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

    [HttpGet("products/{id}")]
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

    [HttpGet("products/price-range")]
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

    [HttpPost("products")]
    public async Task<IActionResult> AddProduct([FromForm] CreateProductDto form)
    {
        var result = await _mediator.Send(new AddItemCommand(form, form.Image));
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto form)
    {
        var result = await _mediator.Send(new UpdateItemCommand(id, form, form.Image));
        return result.Succeeded ? Ok(result) :
            result.Errors.Any(e => e.Contains("not found")) ? NotFound(result) : BadRequest(result);
    }

    [HttpDelete("products/{id}")]
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