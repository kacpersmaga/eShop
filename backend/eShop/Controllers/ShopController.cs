using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers;

public class ShopController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet("index")]
    public IActionResult RedirectToIndex()
    {
        return RedirectToAction("Index");
    }
}
