using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers;

[Route("admin")]
public class AdminController : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("add-item")]
    public IActionResult AddItem()
    {
        return View();
    }
}
