using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public IActionResult Enable2fa()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult Verify2fa()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }
        
        [HttpGet]
        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult ConfirmEmail2fa()
        {
            return View();
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult ConfirmPhone2fa()
        {
            return View();
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}