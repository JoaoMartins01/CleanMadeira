using CleanMadeira.Domain.Entities;
using CleanMadeira.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext db,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration config)
    {
        _db = db;
        _signInManager = signInManager;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == request.Email);

        if (user == null)
            return Unauthorized(new { message = "Email inválido" });

        var result = await _signInManager.PasswordSignInAsync(
            request.Email,
            request.Password,
            false,
            lockoutOnFailure: false);

        if (!result.Succeeded)
            return Unauthorized(new { message = "Password inválida" });

        var token = GenerateJwtToken(user);

        return Ok(new { token });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ← CORRETO
        new Claim("userId", user.Id.ToString())
    };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

