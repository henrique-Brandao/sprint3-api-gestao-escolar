using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Realiza login e retorna um token JWT.
    /// </summary>
    /// <param name="request">Email e senha do usuário.</param>
    /// <returns>Token JWT para acessar os endpoints protegidos.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        string role;

        if (request.Email == "admin@sprint3.com" && request.Senha == "123456")
        {
            role = "Admin";
        }
        else if (request.Email == "professor@sprint3.com" && request.Senha == "123456")
        {
            role = "Professor";
        }
        else if (request.Email == "aluno@sprint3.com" && request.Senha == "123456")
        {
            role = "Aluno";
        }
        else
        {
            return Unauthorized(new { mensagem = "Email ou senha inválidos." });
        }

        var tokenResponse = GerarToken(request.Email, role);

        return Ok(tokenResponse);
    }

    private TokenResponse GerarToken(string email, string role)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationInMinutes = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var expiration = DateTime.UtcNow.AddMinutes(expirationInMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResponse(tokenString, expiration);
    }
}