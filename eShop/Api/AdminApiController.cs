using AutoMapper;
using eShop.Models.Domain;
using eShop.Models.Dtos;
using eShop.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Api;

[ApiController]
[Route("api/admin")]
public class AdminApiController(
    IItemService itemService,
    IBlobStorageService blobService,
    IMapper mapper,
    ILogger<AdminApiController> logger)
    : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> AddItem([FromForm] ShopItemFormModel model, [FromForm] IFormFile? image)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Model state invalid for item '{Name}'. Returning 400.", model.Name);
            return BadRequest(ModelState);
        }

        string? uploadedPath = null;
        if (image is { Length: > 0 })
        {
            uploadedPath = await blobService.UploadFileAsync(image);
        }

        var shopItem = mapper.Map<ShopItem>(model);
        shopItem.ImagePath = uploadedPath;

        await itemService.AddItem(shopItem);

        logger.LogInformation("Item '{Name}' added successfully.", model.Name);

        var successResponse = new SuccessResponse
        {
            Message = $"Item '{model.Name}' added successfully!"
        };
        return Ok(successResponse);
    }
}