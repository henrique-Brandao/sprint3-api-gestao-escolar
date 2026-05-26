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
public class SolicitacaoAcessoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;

    public SolicitacaoAcessoController(AppDbContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Post([FromBody] SolicitacaoAcessoRequest request)
    {
        if (!AppRoles.CriaveisPorDiretor.Contains(request.TipoSolicitado))
        {
            return BadRequest(new { mensagem = "Tipo de solicitação inválido. Use Aluno ou Professor." });
        }

        var solicitacao = new SolicitacaoAcesso
        {
            Nome = request.Nome,
            Email = request.Email,
            TipoSolicitado = request.TipoSolicitado,
            Mensagem = request.Mensagem,
            Status = StatusSolicitacao.Pendente,
            CriadoEm = DateTime.UtcNow
        };

        _context.SolicitacoesAcesso.Add(solicitacao);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = solicitacao.Id }, Map(solicitacao));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Diretor")]
    public async Task<IActionResult> Get([FromQuery] string? status = null)
    {
        var query = _context.SolicitacoesAcesso.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(s => s.Status == status);
        }

        var solicitacoes = (await query
            .OrderByDescending(s => s.CriadoEm)
            .ToListAsync())
            .Select(Map);

        return Ok(solicitacoes);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Diretor")]
    public async Task<IActionResult> GetById(int id)
    {
        var solicitacao = await _context.SolicitacoesAcesso.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        return solicitacao == null
            ? NotFound(new { mensagem = "Solicitação não encontrada." })
            : Ok(Map(solicitacao));
    }

    [HttpPost("{id:int}/aprovar")]
    [Authorize(Roles = "Admin,Diretor")]
    public async Task<IActionResult> Aprovar(int id, [FromBody] AprovarSolicitacaoRequest request)
    {
        var solicitacao = await _context.SolicitacoesAcesso.FindAsync(id);
        if (solicitacao == null)
        {
            return NotFound(new { mensagem = "Solicitação não encontrada." });
        }

        if (solicitacao.Status != StatusSolicitacao.Pendente)
        {
            return BadRequest(new { mensagem = "A solicitação já foi analisada." });
        }

        if (User.Role() == AppRoles.Diretor && !AppRoles.CriaveisPorDiretor.Contains(solicitacao.TipoSolicitado))
        {
            return Forbid();
        }

        if (await _context.Usuarios.AnyAsync(u => u.Email == solicitacao.Email))
        {
            return BadRequest(new { mensagem = "Já existe usuário com este email." });
        }

        var senha = string.IsNullOrWhiteSpace(request.SenhaInicial)
            ? "Temp@123"
            : request.SenhaInicial;

        var usuario = new Usuario
        {
            Nome = solicitacao.Nome,
            Email = solicitacao.Email,
            Role = solicitacao.TipoSolicitado,
            SenhaHash = _passwordService.Hash(senha)
        };

        if (solicitacao.TipoSolicitado == AppRoles.Aluno)
        {
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.Email == solicitacao.Email)
                ?? new Aluno { Nome = solicitacao.Nome, Email = solicitacao.Email };
            if (aluno.Id == 0) _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();
            usuario.AlunoId = aluno.Id;
        }
        else if (solicitacao.TipoSolicitado == AppRoles.Professor)
        {
            var professor = await _context.Professores.FirstOrDefaultAsync(p => p.Email == solicitacao.Email)
                ?? new Professor { Nome = solicitacao.Nome, Email = solicitacao.Email };
            if (professor.Id == 0) _context.Professores.Add(professor);
            await _context.SaveChangesAsync();
            usuario.ProfessorId = professor.Id;
        }
        else
        {
            return BadRequest(new { mensagem = "Tipo de solicitação inválido." });
        }

        solicitacao.Status = StatusSolicitacao.Aprovada;
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok(new AprovacaoSolicitacaoResponse(
            Map(solicitacao),
            new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.Role, usuario.AlunoId, usuario.ProfessorId, usuario.DiretorId),
            senha
        ));
    }

    [HttpPost("{id:int}/recusar")]
    [Authorize(Roles = "Admin,Diretor")]
    public async Task<IActionResult> Recusar(int id)
    {
        var solicitacao = await _context.SolicitacoesAcesso.FindAsync(id);
        if (solicitacao == null)
        {
            return NotFound(new { mensagem = "Solicitação não encontrada." });
        }

        if (solicitacao.Status != StatusSolicitacao.Pendente)
        {
            return BadRequest(new { mensagem = "A solicitação já foi analisada." });
        }

        solicitacao.Status = StatusSolicitacao.Recusada;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private static SolicitacaoAcessoResponse Map(SolicitacaoAcesso s) =>
        new(s.Id, s.Nome, s.Email, s.TipoSolicitado, s.Mensagem, s.Status, s.CriadoEm);
}
