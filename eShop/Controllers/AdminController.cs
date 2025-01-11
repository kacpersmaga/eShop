using eShop.Mappers;
using eShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers;

public class AdminController : Controller
{
    private readonly ItemService _itemService;
    private readonly BlobStorageService _blobStorageService;

    public AdminController(ItemService itemService, BlobStorageService blobStorageService)
    {
        _itemService = itemService;
        _blobStorageService = blobStorageService;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult AddItem()
    {
        return View(new ShopItemFormModel());
    }
    
    [HttpPost]
    public async Task<IActionResult> AddItem(ShopItemFormModel item, IFormFile Image)
    {
        if (ModelState.IsValid)
        {
            string? imagePath = null;
            if (Image != null && Image.Length > 0)
            {
                imagePath = await _blobStorageService.UploadFileAsync(Image);
            }
            var shopItem = ShopItemMapper.MapToShopItem(item, imagePath);
            
            _itemService.AddItem(shopItem);

            TempData["SuccessMessage"] = $"Item '{item.Name}' added successfully!";
            return RedirectToAction("AddItem");
        }

        return View(item);
    }
    
}