using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;
using Sprint3.Models;

namespace Sprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DiretorController : ControllerBase
{
    private readonly AppDbContext _context;

    public DiretorController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var diretores = await _context.Diretores
            .AsNoTracking()
            .Select(d => new DiretorResponse(d.Id, d.Nome, d.Email))
            .ToListAsync();

        return Ok(diretores);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var diretor = await _context.Diretores.AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DiretorResponse(d.Id, d.Nome, d.Email))
            .FirstOrDefaultAsync();

        return diretor == null
            ? NotFound(new { mensagem = "Diretor não encontrado." })
            : Ok(diretor);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] DiretorRequest request)
    {
        if (await _context.Diretores.AnyAsync(d => d.Email == request.Email))
        {
            return BadRequest(new { mensagem = "Já existe diretor com este email." });
        }

        var diretor = new Diretor { Nome = request.Nome, Email = request.Email };
        _context.Diretores.Add(diretor);
        await _context.SaveChangesAsync();

        var response = new DiretorResponse(diretor.Id, diretor.Nome, diretor.Email);
        return CreatedAtAction(nameof(GetById), new { id = diretor.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, [FromBody] DiretorRequest request)
    {
        var diretor = await _context.Diretores.FindAsync(id);
        if (diretor == null)
        {
            return NotFound(new { mensagem = "Diretor não encontrado." });
        }

        if (await _context.Diretores.AnyAsync(d => d.Id != id && d.Email == request.Email))
        {
            return BadRequest(new { mensagem = "Já existe diretor com este email." });
        }

        diretor.Nome = request.Nome;
        diretor.Email = request.Email;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var diretor = await _context.Diretores.FindAsync(id);
        if (diretor == null)
        {
            return NotFound(new { mensagem = "Diretor não encontrado." });
        }

        _context.Diretores.Remove(diretor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
