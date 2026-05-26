using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;
using Sprint3.Models;
using Sprint3.Security;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Diretor")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;

    public UsuarioController(AppDbContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Get()
    {
        var usuarios = (await _context.Usuarios.AsNoTracking().ToListAsync()).Select(Map);
        return Ok(usuarios);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return usuario == null
            ? NotFound(new { mensagem = "Usuário não encontrado." })
            : Ok(Map(usuario));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UsuarioRequest request)
    {
        if (!PodeCriarRole(request.Role))
        {
            return Forbid();
        }

        var resultado = await CriarUsuario(request);
        if (!resultado.Ok)
        {
            return BadRequest(new { mensagem = resultado.Erro });
        }

        return CreatedAtAction(nameof(GetById), new { id = resultado.Usuario!.Id }, Map(resultado.Usuario));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Put(int id, [FromBody] UsuarioRequest request)
    {
        _ = id;
        _ = request;
        return BadRequest(new { mensagem = "Usuários não podem ser editados. Exclua e crie uma nova conta para alterar vínculos ou identificadores." });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound(new { mensagem = "Usuário não encontrado." });
        }

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    internal async Task<(bool Ok, string? Erro, Usuario? Usuario)> CriarUsuario(UsuarioRequest request)
    {
        if (!AppRoles.Todos.Contains(request.Role))
        {
            return (false, "Perfil inválido.", null);
        }

        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Email))
        {
            return (false, "Já existe usuário com este email.", null);
        }

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            Role = request.Role,
            SenhaHash = _passwordService.Hash(request.Senha)
        };

        if (request.Role == AppRoles.Aluno)
        {
            usuario.AlunoId = request.AlunoId;
            if (usuario.AlunoId == null || !await _context.Alunos.AnyAsync(a => a.Id == usuario.AlunoId))
                return (false, "Aluno informado não existe.", null);
        }
        else if (request.Role == AppRoles.Professor)
        {
            usuario.ProfessorId = request.ProfessorId;
            if (usuario.ProfessorId == null || !await _context.Professores.AnyAsync(p => p.Id == usuario.ProfessorId))
                return (false, "Professor informado não existe.", null);
        }
        else if (request.Role == AppRoles.Diretor)
        {
            usuario.DiretorId = request.DiretorId;
            if (usuario.DiretorId == null || !await _context.Diretores.AnyAsync(d => d.Id == usuario.DiretorId))
                return (false, "Diretor informado não existe.", null);
        }

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return (true, null, usuario);
    }

    private bool PodeCriarRole(string role)
    {
        if (User.Role() == AppRoles.Admin) return true;
        return User.Role() == AppRoles.Diretor && AppRoles.CriaveisPorDiretor.Contains(role);
    }

    private static UsuarioResponse Map(Usuario u) =>
        new(u.Id, u.Nome, u.Email, u.Role, u.AlunoId, u.ProfessorId, u.DiretorId);
}
