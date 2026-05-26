using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs.Request;
using Sprint3.Exceptions;
using Sprint3.Security;
using Sprint3.Services.Interfaces;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatriculaController : ControllerBase
{
    private readonly IMatriculaService _matriculaService;

    public MatriculaController(IMatriculaService matriculaService)
    {
        _matriculaService = matriculaService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Diretor,Professor,Aluno")]
    public async Task<IActionResult> Get()
    {
        if (User.Role() == AppRoles.Aluno)
        {
            var alunoId = User.AlunoId();
            if (alunoId == null) return Forbid();

            return Ok(await _matriculaService.ListarPorAluno(alunoId.Value));
        }

        if (User.Role() == AppRoles.Professor)
        {
            var professorId = User.ProfessorId();
            if (professorId == null) return Forbid();

            return Ok(await _matriculaService.ListarPorProfessor(professorId.Value));
        }

        return Ok(await _matriculaService.ListarTodas());
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Diretor,Professor,Aluno")]
    public async Task<IActionResult> GetById(int id)
    {
        var matricula = await _matriculaService.BuscarPorId(id);

        if (matricula == null)
        {
            return NotFound(new { mensagem = "Matrícula não encontrada." });
        }

        if (User.Role() == AppRoles.Aluno && User.AlunoId() != matricula.AlunoId)
        {
            return Forbid();
        }

        if (User.Role() == AppRoles.Professor && User.ProfessorId() != matricula.ProfessorId)
        {
            return Forbid();
        }

        return Ok(matricula);
    }

    [HttpGet("aluno/{alunoId:int}")]
    [Authorize(Roles = "Admin,Diretor,Professor,Aluno")]
    public async Task<IActionResult> GetByAluno(int alunoId)
    {
        if (User.Role() == AppRoles.Aluno && User.AlunoId() != alunoId)
        {
            return Forbid();
        }

        var matriculas = await _matriculaService.ListarPorAluno(alunoId);

        if (User.Role() == AppRoles.Professor)
        {
            var professorId = User.ProfessorId();
            if (professorId == null) return Forbid();

            matriculas = matriculas.Where(m => m.ProfessorId == professorId);
        }

        return Ok(matriculas);
    }

    [HttpGet("disciplina/{disciplinaId:int}")]
    [Authorize(Roles = "Admin,Diretor,Professor")]
    public async Task<IActionResult> GetByDisciplina(int disciplinaId)
    {
        var matriculas = await _matriculaService.ListarPorDisciplina(disciplinaId);

        if (User.Role() == AppRoles.Professor)
        {
            var professorId = User.ProfessorId();
            if (professorId == null || matriculas.Any(m => m.ProfessorId != professorId)) return Forbid();
        }

        return Ok(matriculas);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Diretor")]
    public async Task<IActionResult> Post([FromBody] MatriculaRequest request)
    {
        try
        {
            var matricula = await _matriculaService.Criar(request);
            return CreatedAtAction(nameof(GetById), new { id = matricula.Id }, matricula);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Diretor")]
    public async Task<IActionResult> Put(int id, [FromBody] MatriculaRequest request)
    {
        try
        {
            var atualizado = await _matriculaService.Atualizar(id, request);
            if (!atualizado) return NotFound(new { mensagem = "Matrícula não encontrada." });

            return Ok(await _matriculaService.BuscarPorId(id));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Diretor")]
    public async Task<IActionResult> Delete(int id)
    {
        var deletado = await _matriculaService.Deletar(id);
        if (!deletado) return NotFound(new { mensagem = "Matrícula não encontrada." });

        return NoContent();
    }
}
