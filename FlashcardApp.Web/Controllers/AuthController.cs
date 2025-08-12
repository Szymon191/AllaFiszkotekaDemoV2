using FlashcardApp.Core.Auth.DTO;
using FlashcardApp.Core.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FlashcardApp.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly List<string> _revokedTokens = new List<string>();// Prosty rejestr unieważnionych tokenów (w prawdziwej aplikacji użyj bazy)

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var token = await _authService.RegisterAsync(registerDto);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var token = await _authService.LoginAsync(loginDto);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("refresh")]
    [Authorize] // Wymaga ważnego tokenu
    public async Task<IActionResult> RefreshToken()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            return Unauthorized("Token is required.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]); // Klucz z appsettings.json
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = false // Pomijamy walidację czasu, bo token może być nieaktualny
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            // Generowanie nowego tokenu
            var newToken = GenerateJwtToken(userId);
            return Ok(new { token = newToken });
        }
        catch
        {
            return Unauthorized("Invalid token.");
        }
    }

    private string GenerateJwtToken(string userId)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId),
                    // Dodaj inne claimy, jeśli potrzebne
                }),
            Expires = DateTime.UtcNow.AddDays(1), // Nowy token ważny 1 godzinę
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateToken([FromHeader] string Authorization)
    {
        var token = Authorization?.Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            return Unauthorized("Token is required.");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Brak tolerancji na różnice czasowe
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            if (_revokedTokens.Contains(token)) // Sprawdzenie rezygnacji
                return Unauthorized("Token has been revoked.");

            var userId = jwtToken.Claims.First(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return Ok(new { valid = true, userId });
        }
        catch
        {
            return Unauthorized("Invalid token.");
        }
    }

    // Metoda wylogowania do unieważnienia tokenu (przykład)
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token))
            return Unauthorized("Token is required.");

        _revokedTokens.Add(token); // Dodaj token do listy unieważnionych
        return Ok("Logged out successfully.");
    }
}