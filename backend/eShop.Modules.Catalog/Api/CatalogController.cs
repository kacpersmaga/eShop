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
}