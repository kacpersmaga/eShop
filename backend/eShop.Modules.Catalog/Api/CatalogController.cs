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

    [HttpGet("items")]
    public async Task<IActionResult> GetItems()
    {
        try
        {
            _logger.LogInformation("Executing GetItems query...");
            var result = await _mediator.Send(new GetAllItemsQuery());
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching items.");
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    [HttpGet("items/{id}")]
    public async Task<IActionResult> GetItemById(int id)
    {
        try
        {
            _logger.LogInformation("Executing GetItemById query for ID {ItemId}...", id);
            var result = await _mediator.Send(new GetItemByIdQuery(id));
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching item with ID {ItemId}.", id);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromForm] ShopItemFormModel model, [FromForm] IFormFile? image)
    {
        try
        {
            _logger.LogInformation("Executing AddItem command...");
            var result = await _mediator.Send(new AddItemCommand(model, image));
            
            if (result.Succeeded)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding item {Name}", model.Name);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromForm] ShopItemFormModel model, [FromForm] IFormFile? image)
    {
        try
        {
            _logger.LogInformation("Executing UpdateItem command for ID {ItemId}...", id);
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
            _logger.LogError(ex, "Error occurred while updating item with ID {ItemId}.", id);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        try
        {
            _logger.LogInformation("Executing DeleteItem command for ID {ItemId}...", id);
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
            _logger.LogError(ex, "Error occurred while deleting item with ID {ItemId}.", id);
            return StatusCode(500, Result.Failure("An error occurred while processing your request."));
        }
    }
}