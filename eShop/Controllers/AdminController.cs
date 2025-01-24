using eShop.Mappers;
using eShop.Models;
using eShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly IItemService _itemService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IItemService itemService, 
            IBlobStorageService blobStorageService, 
            ILogger<AdminController> logger)
        {
            _itemService = itemService;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddItem()
        {
            // Return a blank form model; Name and Category start as "", Description is null.
            return View(new ShopItemFormModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(ShopItemFormModel item, IFormFile Image)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Invalid model state while adding item '{ItemName}' by user {User}.", 
                    item.Name, 
                    User?.Identity?.Name ?? "Anonymous"
                );
                
                return View(item);
            }

            try
            {
                string? imagePath = null;

                if (Image != null && Image.Length > 0)
                {
                    imagePath = await _blobStorageService.UploadFileAsync(Image);
                }

                var shopItem = ShopItemMapper.MapToShopItem(item, imagePath);
                await _itemService.AddItem(shopItem);

                TempData["SuccessMessage"] = $"Item '{item.Name}' added successfully!";
                _logger.LogInformation(
                    "Item '{ItemName}' added successfully by user {User}.", 
                    item.Name, 
                    User?.Identity?.Name ?? "Anonymous"
                );

                return RedirectToAction("AddItem");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while adding item '{ItemName}' by user {User}.", 
                    item.Name, 
                    User?.Identity?.Name ?? "Anonymous"
                );

                ModelState.AddModelError("", "An error occurred while processing your request. Please try again later.");
                return View(item);
            }
        }
    }
}
