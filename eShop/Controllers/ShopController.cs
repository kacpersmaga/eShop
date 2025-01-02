using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers;

public class ShopController : Controller
{
    private readonly ItemService _itemService;

    public ShopController(ItemService itemService)
    {
        _itemService = itemService;
    }
    
    public IActionResult Index()
    {
        var items = _itemService.GetAllItems();
        return View(items);
    }
}