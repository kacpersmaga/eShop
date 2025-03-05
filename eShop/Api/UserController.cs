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
        IPhoneService phoneService,
        IEmailService emailService)
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

        [HttpPost("initiate-password-change")]
        public async Task<IActionResult> InitiatePasswordChange([FromBody] InitiatePasswordChangeDto model)
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
            
            var checkPasswordResult = await userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!checkPasswordResult)
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }
            
            if (user.TwoFactorEnabled)
            {
                if (user.TwoFactorType == "Email")
                {
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    await emailService.SendEmailAsync(user.Email!, "Password Change Verification", 
                        $"Your verification code to authorize password change is: {token}");
                    
                    return Ok(new { message = "Verification code sent to your email", requiresTwoFactor = true });
                }
                else if (user.TwoFactorType == "Phone")
                {
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, "Phone");
                    await phoneService.SendSmsAsync(user.PhoneNumber!, $"Your verification code to authorize password change is: {token}");
                    
                    return Ok(new { message = "Verification code sent to your phone", requiresTwoFactor = true });
                }
                else
                {
                    return Ok(new { message = "Please enter the code from your authenticator app to authorize password change", requiresTwoFactor = true });
                }
            }
            
            return Ok(new { message = "Identity verified. Please enter your new password.", verificationComplete = true });
        }

        [HttpPost("complete-password-change")]
        public async Task<IActionResult> CompletePasswordChange([FromBody] CompletePasswordChangeDto model)
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
            
            if (user.TwoFactorEnabled)
            {
                bool isValid;
                if (user.TwoFactorType == "Email")
                {
                    isValid = await userManager.VerifyTwoFactorTokenAsync(user, "Email", model.VerificationCode);
                }
                else if (user.TwoFactorType == "Phone")
                {
                    isValid = await userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.VerificationCode);
                }
                else
                {
                    isValid = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, model.VerificationCode);
                }

                if (!isValid)
                {
                    return BadRequest(new { message = "Invalid verification code" });
                }
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

        [HttpPost("initiate-email-change")]
        public async Task<IActionResult> InitiateEmailChange()
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
            
            if (user.TwoFactorEnabled)
            {
                if (user.TwoFactorType == "Email")
                {
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    await emailService.SendEmailAsync(user.Email!, "Email Change Verification", 
                        $"Your verification code to authorize email change is: {token}");
                    
                    return Ok(new { message = "Verification code sent to your current email", requiresTwoFactor = true });
                }
                else if (user.TwoFactorType == "Phone")
                {
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, "Phone");
                    await phoneService.SendSmsAsync(user.PhoneNumber!, $"Your verification code to authorize email change is: {token}");
                    
                    return Ok(new { message = "Verification code sent to your phone", requiresTwoFactor = true });
                }
                else
                {
                    return Ok(new { message = "Please enter the code from your authenticator app to authorize email change", requiresTwoFactor = true });
                }
            }
            
            return Ok(new { message = "Please enter your new email address.", verificationComplete = true });
        }

        [HttpPost("complete-email-change")]
        public async Task<IActionResult> CompleteEmailChange([FromBody] CompleteEmailChangeDto model)
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
            
            var existingUser = await userManager.FindByEmailAsync(model.NewEmail);
            if (existingUser != null && existingUser.Id != userId)
            {
                return BadRequest(new { message = "Email is already in use" });
            }
            
            if (user.TwoFactorEnabled)
            {
                bool isValid;
                if (user.TwoFactorType == "Email")
                {
                    isValid = await userManager.VerifyTwoFactorTokenAsync(user, "Email", model.VerificationCode);
                }
                else if (user.TwoFactorType == "Phone")
                {
                    isValid = await userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.VerificationCode);
                }
                else
                {
                    isValid = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, model.VerificationCode);
                }

                if (!isValid)
                {
                    return BadRequest(new { message = "Invalid verification code" });
                }
            }
            
            user.Email = model.NewEmail;
            user.EmailConfirmed = false;
            var updateResult = await userManager.UpdateAsync(user);
            
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }
            
            var emailConfirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(emailConfirmToken);
            var callbackUrl = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email?userId={user.Id}&token={encodedToken}";
            
            await emailService.SendEmailAsync(model.NewEmail, "Confirm your new email", 
                $"Please confirm your new email by clicking here: <a href='{callbackUrl}'>Confirm Email</a>");
            
            return Ok(new { message = "Email updated. Please check your new email for confirmation." });
        }

        [HttpPost("request-account-deletion")]
        public async Task<IActionResult> RequestAccountDeletion()
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
            
            if (user.TwoFactorEnabled)
            {
                if (user.TwoFactorType == "Email")
                {
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    await emailService.SendEmailAsync(user.Email!, "Account Deletion Verification", 
                        $"Your verification code to delete your account is: {token}");
                    
                    return Ok(new { message = "Verification code sent to your email", requiresTwoFactor = true });
                }
                else if (user.TwoFactorType == "Phone")
                {
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, "Phone");
                    await phoneService.SendSmsAsync(user.PhoneNumber!, $"Your verification code to delete your account is: {token}");
                    
                    return Ok(new { message = "Verification code sent to your phone", requiresTwoFactor = true });
                }
                else
                {
                    return Ok(new { message = "Please enter the code from your authenticator app", requiresTwoFactor = true });
                }
            }
            
            return BadRequest(new { message = "Two-factor authentication must be enabled to delete your account" });
        }

        [HttpPost("confirm-account-deletion")]
        public async Task<IActionResult> ConfirmAccountDeletion([FromBody] TwoFactorVerificationDto model)
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
            
            bool isValid;
            if (user.TwoFactorType == "Email")
            {
                isValid = await userManager.VerifyTwoFactorTokenAsync(user, "Email", model.Code);
            }
            else if (user.TwoFactorType == "Phone")
            {
                isValid = await userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.Code);
            }
            else
            {
                isValid = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, model.Code);
            }

            if (!isValid)
            {
                return BadRequest(new { message = "Invalid verification code" });
            }
            
            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                foreach (var error in deleteResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(new { message = "Failed to delete account", errors = ModelState });
            }

            return Ok(new { message = "Account deleted successfully" });
        }

        [HttpPost("disable-2fa")]
        public async Task<IActionResult> RequestDisableTwoFactorAuthentication()
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

            if (!user.TwoFactorEnabled)
            {
                return BadRequest(new { message = "Two-factor authentication is not enabled" });
            }
            
            if (user.TwoFactorType == "Email")
            {
                var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");
                await emailService.SendEmailAsync(user.Email!, "Disable 2FA Verification", 
                    $"Your verification code to disable 2FA is: {token}");
                
                return Ok(new { message = "Verification code sent to your email", requiresVerification = true });
            }
            else if (user.TwoFactorType == "Phone")
            {
                var token = await userManager.GenerateTwoFactorTokenAsync(user, "Phone");
                await phoneService.SendSmsAsync(user.PhoneNumber!, $"Your verification code to disable 2FA is: {token}");
                
                return Ok(new { message = "Verification code sent to your phone", requiresVerification = true });
            }
            else
            {
                return Ok(new { message = "Please enter the code from your authenticator app", requiresVerification = true });
            }
        }

        [HttpPost("confirm-disable-2fa")]
        public async Task<IActionResult> ConfirmDisableTwoFactorAuthentication([FromBody] TwoFactorVerificationDto model)
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
            
            bool isValid;
            if (user.TwoFactorType == "Email")
            {
                isValid = await userManager.VerifyTwoFactorTokenAsync(user, "Email", model.Code);
            }
            else if (user.TwoFactorType == "Phone")
            {
                isValid = await userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.Code);
            }
            else
            {
                isValid = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, model.Code);
            }

            if (!isValid)
            {
                return BadRequest(new { message = "Invalid verification code" });
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