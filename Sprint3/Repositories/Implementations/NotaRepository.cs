using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;

namespace Sprint3.Repositories.Implementations;

public class NotaRepository : INotaRepository
{
    private readonly AppDbContext _context;

    public NotaRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Nota>> ListarTodas()
    {
        return await _context.Notas
            .Include(n => n.Aluno)
            .Include(n => n.Disciplina)
            .ToListAsync();
    }

    public async Task<Nota?> BuscarPorId(int id)
    {
        return await _context.Notas
            .Include(n => n.Aluno)
            .Include(n => n.Disciplina)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<Nota>> ListarPorAluno(int alunoId)
    {
        return await _context.Notas
            .Include(n => n.Aluno)
            .Include(n => n.Disciplina)
            .Where(n => n.AlunoId == alunoId)
            .ToListAsync();
    }

    public async Task Adicionar(Nota nota)
    {
        await _context.Notas.AddAsync(nota);
    }

    public void Atualizar(Nota nota)
    {
        _context.Notas.Update(nota);
    }

    public void Deletar(int id)
    {
        var nota = _context.Notas.Find(id);

        if (nota != null)
        {
            _context.Notas.Remove(nota);
        }
    }

    public async Task<bool> SalvarAlteracoes()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}