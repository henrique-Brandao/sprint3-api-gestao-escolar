using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs.Request;
using Sprint3.Services.Interfaces;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DisciplinaController : ControllerBase
{
    private readonly IDisciplinaService _disciplinaService;

    public DisciplinaController(IDisciplinaService disciplinaService)
    {
        _disciplinaService = disciplinaService;
    }

    /// <summary>
    /// Lista todas as disciplinas cadastradas.
    /// </summary>
    /// <returns>Retorna todas as disciplinas, incluindo o professor vinculado.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Professor,Aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get()
    {
        var disciplinas = await _disciplinaService.ListarTodas();
        return Ok(disciplinas);
    }

    /// <summary>
    /// Busca uma disciplina pelo ID.
    /// </summary>
    /// <param name="id">ID da disciplina.</param>
    /// <returns>Retorna os dados da disciplina encontrada.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Professor,Aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(int id)
    {
        var disciplina = await _disciplinaService.BuscarPorId(id);

        if (disciplina == null)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada." });
        }

        return Ok(disciplina);
    }

    /// <summary>
    /// Cadastra uma nova disciplina.
    /// </summary>
    /// <param name="request">Dados da disciplina, incluindo o nome do professor responsável.</param>
    /// <returns>Retorna a disciplina criada.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Professor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post([FromBody] DisciplinaRequest request)
    {
        try
        {
            var novaDisciplina = await _disciplinaService.Criar(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = novaDisciplina.Id },
                novaDisciplina
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza os dados de uma disciplina.
    /// </summary>
    /// <param name="id">ID da disciplina que será atualizada.</param>
    /// <param name="request">Novos dados da disciplina, incluindo o nome do professor responsável.</param>
    /// <returns>Retorna a disciplina atualizada.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Professor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put(int id, [FromBody] DisciplinaRequest request)
    {
        try
        {
            var atualizado = await _disciplinaService.Atualizar(id, request);

            if (!atualizado)
            {
                return NotFound(new { mensagem = "Disciplina não encontrada." });
            }

            var disciplinaAtualizada = await _disciplinaService.BuscarPorId(id);

            return Ok(disciplinaAtualizada);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Remove uma disciplina pelo ID.
    /// </summary>
    /// <param name="id">ID da disciplina que será removida.</param>
    /// <returns>Retorna 204 quando a disciplina é removida com sucesso.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Professor")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var deletado = await _disciplinaService.Deletar(id);

        if (!deletado)
        {
            return NotFound(new { mensagem = "Disciplina não encontrada." });
        }

        return NoContent();
    }
}