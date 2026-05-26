using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;
using Sprint3.Security;
using Sprint3.Services.Interfaces;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotaController : ControllerBase
{
    private readonly INotaService _notaService;
    private readonly AppDbContext _context;

    public NotaController(INotaService notaService, AppDbContext context)
    {
        _notaService = notaService;
        _context = context;
    }

    /// <summary>
    /// Lista todas as notas cadastradas.
    /// </summary>
    /// <returns>Retorna todas as notas, incluindo aluno e disciplina vinculados.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Professor,Diretor,Aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get()
    {
        if (User.Role() == AppRoles.Aluno)
        {
            var alunoId = User.AlunoId();
            if (alunoId == null) return Forbid();
            var notasAluno = await _notaService.ListarPorAluno(alunoId.Value);
            return Ok(notasAluno);
        }

        if (User.Role() == AppRoles.Professor)
        {
            var professorId = User.ProfessorId();
            if (professorId == null) return Forbid();

            var notasProfessor = await _context.Notas
                .Where(n => n.Disciplina!.ProfessorId == professorId)
                .Include(n => n.Aluno)
                .Include(n => n.Disciplina)
                .Select(n => new NotaResponse(
                    n.Id,
                    n.Valor,
                    n.AlunoId,
                    n.Aluno!.Nome,
                    n.DisciplinaId,
                    n.Disciplina!.Nome
                ))
                .ToListAsync();

            return Ok(notasProfessor);
        }

        var notas = await _notaService.ListarTodas();
        return Ok(notas);
    }

    /// <summary>
    /// Busca uma nota pelo ID.
    /// </summary>
    /// <param name="id">ID da nota.</param>
    /// <returns>Retorna os dados da nota encontrada.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Professor,Diretor,Aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(int id)
    {
        var nota = await _notaService.BuscarPorId(id);

        if (nota == null)
        {
            return NotFound(new { mensagem = "Nota não encontrada." });
        }

        if (User.Role() == AppRoles.Aluno && User.AlunoId() != nota.AlunoId)
        {
            return Forbid();
        }

        if (User.Role() == AppRoles.Professor)
        {
            var professorId = User.ProfessorId();
            var permitido = professorId != null && await _context.Notas.AnyAsync(n => n.Id == id && n.Disciplina!.ProfessorId == professorId);
            if (!permitido) return Forbid();
        }

        return Ok(nota);
    }

    /// <summary>
    /// Lista as notas de um aluno específico.
    /// </summary>
    /// <param name="alunoId">ID do aluno.</param>
    /// <returns>Retorna todas as notas vinculadas ao aluno informado.</returns>
    [HttpGet("aluno/{alunoId:int}")]
    [Authorize(Roles = "Admin,Professor,Diretor,Aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByAluno(int alunoId)
    {
        if (User.Role() == AppRoles.Aluno && User.AlunoId() != alunoId)
        {
            return Forbid();
        }

        if (User.Role() == AppRoles.Professor)
        {
            var professorId = User.ProfessorId();
            if (professorId == null) return Forbid();

            var notasProfessor = await _context.Notas
                .Where(n => n.AlunoId == alunoId && n.Disciplina!.ProfessorId == professorId)
                .Include(n => n.Aluno)
                .Include(n => n.Disciplina)
                .Select(n => new NotaResponse(
                    n.Id,
                    n.Valor,
                    n.AlunoId,
                    n.Aluno!.Nome,
                    n.DisciplinaId,
                    n.Disciplina!.Nome
                ))
                .ToListAsync();

            if (!notasProfessor.Any())
            {
                return NotFound(new { mensagem = "Nenhuma nota encontrada para esse aluno." });
            }

            return Ok(notasProfessor);
        }

        var notas = await _notaService.ListarPorAluno(alunoId);

        if (!notas.Any())
        {
            return NotFound(new { mensagem = "Nenhuma nota encontrada para esse aluno." });
        }

        return Ok(notas);
    }

    /// <summary>
    /// Cadastra uma nova nota.
    /// </summary>
    /// <param name="request">Dados da nota, incluindo valor, nome do aluno e nome da disciplina.</param>
    /// <returns>Retorna a nota criada.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Professor,Diretor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post([FromBody] NotaRequest request)
    {
        try
        {
            if (User.Role() == AppRoles.Professor && !await ProfessorPodeAcessarDisciplina(request.DisciplinaId))
            {
                return Forbid();
            }

            var novaNota = await _notaService.Criar(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = novaNota?.Id },
                novaNota
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza os dados de uma nota.
    /// </summary>
    /// <param name="id">ID da nota que será atualizada.</param>
    /// <param name="request">Novos dados da nota, incluindo valor, aluno e disciplina.</param>
    /// <returns>Retorna a nota atualizada.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Professor,Diretor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put(int id, [FromBody] NotaRequest request)
    {
        try
        {
            if (User.Role() == AppRoles.Professor)
            {
                var professorPodeEditarNota = await ProfessorPodeAcessarNota(id);
                var professorPodeUsarDisciplina = await ProfessorPodeAcessarDisciplina(request.DisciplinaId);
                if (!professorPodeEditarNota || !professorPodeUsarDisciplina) return Forbid();
            }

            var atualizado = await _notaService.Atualizar(id, request);

            if (!atualizado)
            {
                return NotFound(new { mensagem = "Nota não encontrada." });
            }

            var notaAtualizada = await _notaService.BuscarPorId(id);

            return Ok(notaAtualizada);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Remove uma nota pelo ID.
    /// </summary>
    /// <param name="id">ID da nota que será removida.</param>
    /// <returns>Retorna 204 quando a nota é removida com sucesso.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Professor,Diretor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        if (User.Role() == AppRoles.Professor && !await ProfessorPodeAcessarNota(id))
        {
            return Forbid();
        }

        var deletado = await _notaService.Deletar(id);

        if (!deletado)
        {
            return NotFound(new { mensagem = "Nota não encontrada." });
        }

        return NoContent();
    }

    private async Task<bool> ProfessorPodeAcessarDisciplina(int? disciplinaId)
    {
        var professorId = User.ProfessorId();
        return professorId != null
            && disciplinaId != null
            && await _context.Disciplinas.AnyAsync(d => d.Id == disciplinaId && d.ProfessorId == professorId);
    }

    private async Task<bool> ProfessorPodeAcessarNota(int notaId)
    {
        var professorId = User.ProfessorId();
        return professorId != null
            && await _context.Notas.AnyAsync(n => n.Id == notaId && n.Disciplina!.ProfessorId == professorId);
    }
}
