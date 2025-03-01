using AutoMapper;
using eShop.Models.Dtos;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Api;

[ApiController]
[Route("api/shop")]
public class ShopApiController(
    IItemService itemService,
    IMapper mapper,
    ILogger<ShopApiController> logger)
    : ControllerBase
{
    private readonly IItemService _itemService = itemService 
                                                 ?? throw new ArgumentNullException(nameof(itemService));
    private readonly IMapper _mapper = mapper 
                                       ?? throw new ArgumentNullException(nameof(mapper));
    private readonly ILogger<ShopApiController> _logger = logger 
                                                          ?? throw new ArgumentNullException(nameof(logger));

    [HttpGet("items")]
    public async Task<IActionResult> GetItems()
    {
        try
        {
            _logger.LogInformation("Fetching items in ShopApiController...");

            var items = await _itemService.GetAllItems();
            var itemDtos = _mapper.Map<List<ShopItemViewModel>>(items);

            _logger.LogInformation("Successfully fetched {Count} items.", itemDtos.Count);

            return Ok(itemDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching items in ShopApiController.");
            throw;
        }
    }
}