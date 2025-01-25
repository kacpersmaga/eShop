using eShop.Models;
using eShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers;

public class ShopController : Controller
{
    private readonly IItemService _itemService;
    private readonly IImageService _imageService; // New abstraction for image-related logic
    private readonly ILogger<ShopController> _logger;

    public ShopController(IItemService itemService, IImageService imageService, ILogger<ShopController> logger)
    {
        _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var items = await _itemService.GetAllItems();

            var viewModel = items.Select(item => new ShopItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description,
                Category = item.Category,
                ImageUri = !string.IsNullOrEmpty(item.ImagePath) 
                    ? _imageService.GetImageUri(item.ImagePath) 
                    : null
            });

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching items for the shop.");
            return View("Error");
        }
    }
}