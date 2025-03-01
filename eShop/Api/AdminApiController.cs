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
        try
        {
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while adding item {Name}", model?.Name);
            throw;
        }
    }
}