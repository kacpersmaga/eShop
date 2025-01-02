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
        return View();
    }
    
    [HttpPost]
    public IActionResult AddItem(ShopItem item, IFormFile Image)
    {
        if (ModelState.IsValid)
        {
            if (Image != null && Image.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", Image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Image.CopyTo(stream);
                }
                item.ImagePath = "/images/" + Image.FileName;
            }
            
            _itemService.AddItem(item);
            TempData["SuccessMessage"] = $"Item '{item.Name}' added successfully!";
            return RedirectToAction("AddItem");
        }

        return View(item);
    }
    
}