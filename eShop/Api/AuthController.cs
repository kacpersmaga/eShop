using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using eShop.Models.Dtos;
using eShop.Models.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using eShop.Models.Domain;
using eShop.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eShop.Api;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<JwtSettings> jwtSettings)
    : ControllerBase
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var emailExists = await userManager.FindByEmailAsync(model.Email);
        if (emailExists != null)
        {
            ModelState.AddModelError("email", "Email is already in use.");
        }

        var usernameExists = await userManager.FindByNameAsync(model.UserName);
        if (usernameExists != null)
        {
            ModelState.AddModelError("userName", "Username is already in use.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                if (error.Code.StartsWith("Password", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("password", error.Description);
                }
                else
                {
                    ModelState.AddModelError("general", error.Description);
                }
            }
            return ValidationProblem();
        }

        return Ok(new { message = "Registration successful." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            )});
        }

        var user = await userManager.FindByNameAsync(model.UserName);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return Unauthorized(new { errors = new Dictionary<string, string[]> {
                { "auth", ["Invalid login attempt."] }
            }});
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return Unauthorized(new { errors = new Dictionary<string, string[]> {
                { "auth", ["Invalid login attempt."] }
            }});
        }
        
        if (await userManager.GetTwoFactorEnabledAsync(user))
        {
            return Ok(new { requiresTwoFactor = true, userName = user.UserName });
        }

        var tokens = await GenerateTokensForUser(user);
        return Ok(tokens);
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> VerifyTwoFactor([FromBody] Verify2faDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await userManager.FindByNameAsync(model.UserName);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid login attempt." });
        }
        
        var isValid = await userManager.VerifyTwoFactorTokenAsync(
            user, TokenOptions.DefaultAuthenticatorProvider, model.Code);

        if (!isValid)
        {
            return Unauthorized(new { message = "Invalid two-factor authentication code." });
        }

        var tokens = await GenerateTokensForUser(user);
        return Ok(tokens);
    }
    
    [HttpPost("enable-2fa/confirm")]
    [Authorize]
    public async Task<IActionResult> ConfirmEnableTwoFactor([FromBody] Confirm2faDto model)
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
            return Unauthorized();
        }
        
        var isValid = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, model.Code);
        if (!isValid)
        {
            return BadRequest(new { message = "Invalid authentication code." });
        }
        
        user.TwoFactorEnabled = true;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return StatusCode(500, new { message = "Failed to enable 2FA." });
        }
        
        return Ok(new { message = "Two-Factor Authentication enabled successfully." });
    }
    
    [HttpGet("enable-2fa")]
    [Authorize]
    public async Task<IActionResult> EnableTwoFactor()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized();
        }

        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            key = await userManager.GetAuthenticatorKeyAsync(user);
        }

        var qrCodeUri = $"otpauth://totp/eShop:{Uri.EscapeDataString(user.Email!)}?secret={key}&issuer=eShop";
        return Ok(new { key, qrCodeUri });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto tokenDto)
    {
        if (string.IsNullOrEmpty(tokenDto.RefreshToken))
        {
            return BadRequest("Refresh token is required");
        }

        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == tokenDto.RefreshToken);

        if (user == null)
        {
            return Unauthorized("Invalid refresh token");
        }

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Unauthorized("Refresh token expired");
        }

        var tokens = await GenerateTokensForUser(user);
        return Ok(tokens);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromServices] ITokenRevocationService tokenRevocationService)
    {
        var jti = User.FindFirstValue(JwtRegisteredClaimNames.Jti);
        if (!string.IsNullOrEmpty(jti))
        {
            await tokenRevocationService.RevokeTokenByJtiAsync(jti);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userId))
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddYears(-1);
                await userManager.UpdateAsync(user);
            }
        }

        Response.Cookies.Delete("authToken");
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("check-email")]
    public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        return Ok(new { exists = user != null });
    }

    [HttpGet("check-username")]
    public async Task<IActionResult> CheckUsernameExists([FromQuery] string username)
    {
        var user = await userManager.FindByNameAsync(username);
        return Ok(new { exists = user != null });
    }
    
    private async Task<AuthResponseDto> GenerateTokensForUser(ApplicationUser user)
    {
        var accessToken = await CreateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        Response.Cookies.Append("authToken", accessToken, cookieOptions);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
        };
    }

    private async Task<string> CreateAccessToken(ApplicationUser user)
    {
        var userRoles = await userManager.GetRolesAsync(user);
        var jti = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, jti)
        };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

