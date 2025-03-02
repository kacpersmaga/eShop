using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using eShop.Models.Domain;
using eShop.Models.Dtos;
using eShop.Services.Interfaces;

namespace eShop.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController(
        UserManager<ApplicationUser> userManager,
        IPhoneService phoneService)
        : ControllerBase
    {
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.EmailConfirmed,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.TwoFactorType
            });
        }

        [HttpPost("update-phone")]
        public async Task<IActionResult> UpdatePhoneNumber([FromBody] UpdatePhoneDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            
            var changePhoneResult = await userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            if (!changePhoneResult.Succeeded)
            {
                foreach (var error in changePhoneResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }
            
            var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await phoneService.SendSmsAsync(model.PhoneNumber, $"Your phone verification code is: {token}");

            return Ok(new { message = "Phone number updated. A verification code has been sent." });
        }

        [HttpPost("verify-phone")]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, user.PhoneNumber!);
            if (!result)
            {
                return BadRequest(new { message = "Invalid verification code" });
            }

            user.PhoneNumberConfirmed = true;
            await userManager.UpdateAsync(user);

            return Ok(new { message = "Phone number verified successfully" });
        }

        [HttpPost("send-phone-verification")]
        public async Task<IActionResult> SendPhoneVerification()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                return BadRequest(new { message = "No phone number associated with this account" });
            }

            var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            await phoneService.SendSmsAsync(user.PhoneNumber, $"Your phone verification code is: {token}");

            return Ok(new { message = "Verification code sent successfully" });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(new { message = "Failed to change password", errors = ModelState });
            }

            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPost("disable-2fa")]
        public async Task<IActionResult> DisableTwoFactorAuthentication()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to disable two-factor authentication" });
            }
            
            user.TwoFactorType = null;
            await userManager.UpdateAsync(user);

            return Ok(new { message = "Two-factor authentication has been disabled" });
        }
    }
}