using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs.Request;
using Sprint3.Services.Interfaces;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfessorController : ControllerBase
{
    private readonly IProfessorService _professorService;

    public ProfessorController(IProfessorService professorService)
    {
        _professorService = professorService;
    }

    /// <summary>
    /// Lista todos os professores cadastrados.
    /// </summary>
    /// <returns>Retorna todos os professores, incluindo suas disciplinas vinculadas.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Professor,Aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get()
    {
        var professores = await _professorService.ListarTodos();
        return Ok(professores);
    }

    /// <summary>
    /// Busca um professor pelo ID.
    /// </summary>
    /// <param name="id">ID do professor.</param>
    /// <returns>Retorna os dados do professor encontrado.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Professor,Aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(int id)
    {
        var professor = await _professorService.BuscarPorId(id);

        if (professor == null)
        {
            return NotFound(new { mensagem = "Professor não encontrado." });
        }

        return Ok(professor);
    }

    /// <summary>
    /// Busca um professor pelo nome.
    /// </summary>
    /// <param name="nome">Nome ou parte do nome do professor.</param>
    /// <returns>Retorna o professor correspondente ao nome informado.</returns>
    [HttpGet("nome/{nome}")]
    [Authorize(Roles = "Admin,Professor,Aluno")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByName(string nome)
    {
        var professor = await _professorService.BuscarPorNome(nome);

        if (professor == null)
        {
            return NotFound(new { mensagem = "Professor não encontrado." });
        }

        return Ok(professor);
    }

    /// <summary>
    /// Cadastra um novo professor.
    /// </summary>
    /// <param name="request">Dados do professor a ser cadastrado.</param>
    /// <returns>Retorna o professor criado.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Post([FromBody] ProfessorRequest request)
    {
        var novoProfessor = await _professorService.Criar(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = novoProfessor.Id },
            novoProfessor
        );
    }

    /// <summary>
    /// Atualiza os dados de um professor.
    /// </summary>
    /// <param name="id">ID do professor que será atualizado.</param>
    /// <param name="request">Novos dados do professor.</param>
    /// <returns>Retorna o professor atualizado.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put(int id, [FromBody] ProfessorRequest request)
    {
        var atualizado = await _professorService.Atualizar(id, request);

        if (!atualizado)
        {
            return NotFound(new { mensagem = "Professor não encontrado." });
        }

        var professorAtualizado = await _professorService.BuscarPorId(id);

        return Ok(professorAtualizado);
    }

    /// <summary>
    /// Remove um professor pelo ID.
    /// </summary>
    /// <param name="id">ID do professor que será removido.</param>
    /// <returns>Retorna 204 quando o professor é removido com sucesso.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        var deletado = await _professorService.Deletar(id);

        if (!deletado)
        {
            return NotFound(new { mensagem = "Professor não encontrado." });
        }

        return NoContent();
    }
}