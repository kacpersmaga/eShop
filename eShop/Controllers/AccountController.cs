using Microsoft.AspNetCore.Mvc;

namespace eShop.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Enable2fa()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Verify2fa()
        {
            return View();
        }
    }
}