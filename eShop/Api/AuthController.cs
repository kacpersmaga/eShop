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
        
        var response = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
        };

        return Ok(response);
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
        
        var accessToken = await CreateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);
        
        var response = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes)
        };
        
        return Ok(response);
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name;
        if (username != null)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user != null)
            {
                user.RefreshToken = null;
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
    
    #region Helper Methods
    
    private async Task<string> CreateAccessToken(ApplicationUser user)
    {
        var userRoles = await userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
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
    
    #endregion
}