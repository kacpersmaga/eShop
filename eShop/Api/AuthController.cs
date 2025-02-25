using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using eShop.Models.Domain;
using eShop.Models.Dtos;
using eShop.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            )});
        }

        var user = await userManager.FindByNameAsync(model.UserName);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return Unauthorized(new { errors = new Dictionary<string, string[]> {
                { "auth", new[] { "Invalid login attempt." } }
            }});
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid login attempt.");
            return Unauthorized(new { errors = new Dictionary<string, string[]> {
                { "auth", new[] { "Invalid login attempt." } }
            }});
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = tokenDescriptor.Expires,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        Response.Cookies.Append("authToken", jwtToken, cookieOptions);

        return Ok(new { message = "Login successful", token = jwtToken });
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
}