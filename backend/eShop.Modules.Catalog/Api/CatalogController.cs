using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Application.Queries;
using eShop.Modules.Catalog.Commands;
using eShop.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
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
    public async Task<IActionResult> GetProducts()
    {
        try
        {
            _logger.LogInformation("Executing GetAllItemsQuery query...");
            var result = await _mediator.Send(new GetAllItemsQuery());
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching products.");
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
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching product with ID {ProductId}.", id);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    [HttpPost("products")]
    public async Task<IActionResult> AddProduct([FromForm] CreateProductDto model, [FromForm] IFormFile? image)
    {
        try
        {
            _logger.LogInformation("Executing AddItemCommand command...");
            var result = await _mediator.Send(new AddItemCommand(model, image));
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding product {Name}", model.Name);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromForm] UpdateProductDto model, [FromForm] IFormFile? image)
    {
        try
        {
            _logger.LogInformation("Executing UpdateItemCommand command for ID {ProductId}...", id);
            var result = await _mediator.Send(new UpdateItemCommand(id, model, image));
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return result.Errors.Any(e => e.Contains("not found")) 
                ? NotFound(result) 
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product with ID {ProductId}.", id);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            _logger.LogInformation("Executing DeleteItemCommand command for ID {ProductId}...", id);
            var result = await _mediator.Send(new DeleteItemCommand(id));
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return result.Errors.Any(e => e.Contains("not found")) 
                ? NotFound(result) 
                : BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product with ID {ProductId}.", id);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }
}