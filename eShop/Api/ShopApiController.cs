using AutoMapper;
using eShop.Models.Dtos;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Api;

[ApiController]
[Route("api/shop")]
public class ShopApiController(
    IItemService itemService,
    IMapper mapper)
    : ControllerBase
{
    private readonly IItemService _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    [HttpGet("items")]
    public async Task<IActionResult> GetItems()
    {
        var items = await _itemService.GetAllItems();
        var itemDtos = _mapper.Map<List<ShopItemViewModel>>(items);
        return Ok(itemDtos);
    }
}