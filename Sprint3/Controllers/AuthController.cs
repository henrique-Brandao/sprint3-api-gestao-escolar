using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Microsoft.IdentityModel.Tokens;
using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;
using Sprint3.Security;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;

    public AuthController(IConfiguration configuration, AppDbContext context, IPasswordService passwordService)
    {
        _configuration = configuration;
        _context = context;
        _passwordService = passwordService;
    }

    /// <summary>
    /// Realiza login e retorna um token JWT.
    /// </summary>
    /// <param name="request">Email e senha do usuário.</param>
    /// <returns>Token JWT para acessar os endpoints protegidos.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var usuario = await _context.Usuarios.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (usuario == null || !_passwordService.Verify(request.Senha, usuario.SenhaHash))
        {
            return Unauthorized(new { mensagem = "Email ou senha inválidos." });
        }

        var tokenResponse = GerarToken(usuario);

        return Ok(tokenResponse);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var usuarioId = User.UsuarioId();
        if (usuarioId == null)
        {
            return Unauthorized(new { mensagem = "Token inválido." });
        }

        var usuario = await _context.Usuarios.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == usuarioId);

        if (usuario == null)
        {
            return Unauthorized(new { mensagem = "Usuário não encontrado." });
        }

        return Ok(MapUsuario(usuario));
    }

    private TokenResponse GerarToken(Models.Usuario usuario)
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
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim("usuarioId", usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        if (usuario.AlunoId.HasValue) claims.Add(new Claim("alunoId", usuario.AlunoId.Value.ToString()));
        if (usuario.ProfessorId.HasValue) claims.Add(new Claim("professorId", usuario.ProfessorId.Value.ToString()));
        if (usuario.DiretorId.HasValue) claims.Add(new Claim("diretorId", usuario.DiretorId.Value.ToString()));

        var expiration = DateTime.UtcNow.AddMinutes(expirationInMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResponse(
            tokenString,
            expiration,
            usuario.Role,
            usuario.Id,
            usuario.AlunoId,
            usuario.ProfessorId,
            usuario.DiretorId,
            usuario.Nome
        );
    }

    private static UsuarioResponse MapUsuario(Models.Usuario usuario) =>
        new(usuario.Id, usuario.Nome, usuario.Email, usuario.Role, usuario.AlunoId, usuario.ProfessorId, usuario.DiretorId);
}
