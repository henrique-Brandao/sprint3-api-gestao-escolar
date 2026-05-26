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
public class AlunoController : ControllerBase
{
    private readonly IAlunoService _alunoService;
    private readonly AppDbContext _context;

    public AlunoController(IAlunoService alunoService, AppDbContext context)
    {
        _alunoService = alunoService;
        _context = context;
    }

    /// <summary>
    /// Lista todos os alunos cadastrados.
    /// </summary>
    /// <returns>Retorna todos os alunos, incluindo notas, média e situação.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Professor,Diretor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get()
    {
        if (User.Role() == AppRoles.Professor)
        {
            var professorId = User.ProfessorId();
            if (professorId == null) return Forbid();

            var alunosProfessor = await _context.Alunos
                .Where(a => a.Matriculas.Any(m => m.Disciplina!.ProfessorId == professorId))
                .Select(a => new
                {
                    a.Id,
                    a.Nome,
                    a.Email,
                    Notas = a.Notas
                        .Where(n => n.Disciplina!.ProfessorId == professorId)
                        .Select(n => n.Valor)
                        .ToList()
                })
                .ToListAsync();

            return Ok(alunosProfessor.Select(a => MapearAlunoProfessor(a.Id, a.Nome, a.Email, a.Notas)));
        }

        var alunos = await _alunoService.ListarTodos();
        return Ok(alunos);
    }

    /// <summary>
    /// Busca um aluno pelo ID.
    /// </summary>
    /// <param name="id">ID do aluno.</param>
    /// <returns>Retorna os dados do aluno encontrado.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Professor,Diretor,Aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(int id)
    {
        if (User.Role() == AppRoles.Aluno && User.AlunoId() != id)
        {
            return Forbid();
        }

        if (User.Role() == AppRoles.Professor)
        {
            var professorId = User.ProfessorId();
            var alunoProfessor = professorId == null
                ? null
                : await _context.Alunos
                    .Where(a => a.Id == id && a.Matriculas.Any(m => m.Disciplina!.ProfessorId == professorId))
                    .Select(a => new
                    {
                        a.Id,
                        a.Nome,
                        a.Email,
                        Notas = a.Notas
                            .Where(n => n.Disciplina!.ProfessorId == professorId)
                            .Select(n => n.Valor)
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

            if (alunoProfessor == null) return Forbid();
            return Ok(MapearAlunoProfessor(alunoProfessor.Id, alunoProfessor.Nome, alunoProfessor.Email, alunoProfessor.Notas));
        }

        var aluno = await _alunoService.BuscarPorId(id);

        if (aluno == null)
        {
            return NotFound(new { mensagem = "Aluno não encontrado." });
        }

        return Ok(aluno);
    }

    /// <summary>
    /// Busca alunos pelo nome.
    /// </summary>
    /// <param name="nome">Nome ou parte do nome do aluno.</param>
    /// <returns>Retorna uma lista de alunos que correspondem ao nome informado.</returns>
    [HttpGet("nome/{nome}")]
    [Authorize(Roles = "Admin,Professor,Diretor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByName(string nome)
    {
        if (User.Role() == AppRoles.Professor)
        {
            var professorId = User.ProfessorId();
            if (professorId == null) return Forbid();

            var alunosProfessor = await _context.Alunos
                .Where(a => a.Nome.Contains(nome) && a.Matriculas.Any(m => m.Disciplina!.ProfessorId == professorId))
                .Select(a => new
                {
                    a.Id,
                    a.Nome,
                    a.Email,
                    Notas = a.Notas
                        .Where(n => n.Disciplina!.ProfessorId == professorId)
                        .Select(n => n.Valor)
                        .ToList()
                })
                .ToListAsync();

            if (!alunosProfessor.Any())
            {
                return NotFound(new { mensagem = "Nenhum aluno encontrado com esse nome." });
            }

            return Ok(alunosProfessor.Select(a => MapearAlunoProfessor(a.Id, a.Nome, a.Email, a.Notas)));
        }

        var alunos = await _alunoService.BuscarPorNome(nome);

        if (!alunos.Any())
        {
            return NotFound(new { mensagem = "Nenhum aluno encontrado com esse nome." });
        }

        return Ok(alunos);
    }

    /// <summary>
    /// Cadastra um novo aluno.
    /// </summary>
    /// <param name="request">Dados do aluno a ser cadastrado.</param>
    /// <returns>Retorna o aluno criado.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Diretor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post([FromBody] AlunoRequest request)
    {
        if (await _context.Alunos.AnyAsync(a => a.Email == request.Email))
        {
            return BadRequest(new { mensagem = "Já existe aluno com este email." });
        }

        var novoAluno = await _alunoService.Criar(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = novoAluno.Id },
            novoAluno
        );
    }

    /// <summary>
    /// Atualiza os dados de um aluno.
    /// </summary>
    /// <param name="id">ID do aluno que será atualizado.</param>
    /// <param name="request">Novos dados do aluno.</param>
    /// <returns>Retorna o aluno atualizado.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Diretor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put(int id, [FromBody] AlunoRequest request)
    {
        if (await _context.Alunos.AnyAsync(a => a.Id != id && a.Email == request.Email))
        {
            return BadRequest(new { mensagem = "Já existe aluno com este email." });
        }

        var atualizado = await _alunoService.Atualizar(id, request);

        if (!atualizado)
        {
            return NotFound(new { mensagem = "Aluno não encontrado." });
        }

        var alunoAtualizado = await _alunoService.BuscarPorId(id);

        return Ok(alunoAtualizado);
    }

    /// <summary>
    /// Remove um aluno pelo ID.
    /// </summary>
    /// <param name="id">ID do aluno que será removido.</param>
    /// <returns>Retorna 204 quando o aluno é removido com sucesso.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Diretor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var deletado = await _alunoService.Deletar(id);

        if (!deletado)
        {
            return NotFound(new { mensagem = "Aluno não encontrado." });
        }

        return NoContent();
    }

    private static AlunoResponse MapearAlunoProfessor(int id, string nome, string email, List<double> notas)
    {
        var media = notas.Any() ? Math.Round(notas.Average(), 2) : 0;
        var situacao = notas.Any()
            ? media >= 7.0 ? "Aprovado" : "Reprovado"
            : "Sem nota";

        return new AlunoResponse(id, nome, email, notas, media, situacao);
    }
}
