using eShop.Mappers;
using eShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers;

public class AdminController : Controller
{
    private readonly ItemService _itemService;

    public AdminController(ItemService itemService)
    {
        _itemService = itemService;
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
    public IActionResult AddItem(ShopItemFormModel item, IFormFile Image)
    {
        if (ModelState.IsValid)
        {
            string? imagePath = null;
            if (Image != null && Image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                imagePath = "/images/" + fileName;
            }
            
            var shopItem = ShopItemMapper.MapToShopItem(item, imagePath);
            
            _itemService.AddItem(shopItem);

            TempData["SuccessMessage"] = $"Item '{item.Name}' added successfully!";
            return RedirectToAction("AddItem");
        }

        return View(item);
    }
    
}