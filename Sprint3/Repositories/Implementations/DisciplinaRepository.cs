using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;

namespace Sprint3.Repositories.Implementations;

public class DisciplinaRepository : IDisciplinaRepository
{
    private readonly AppDbContext _context;

    public DisciplinaRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Disciplina>> ListarTodas()
    {
        return await _context.Disciplinas.Include(d => d.Professor).ToListAsync();
    }

    public async Task<Disciplina?> BuscarPorId(int id)
    {
        return await _context.Disciplinas
            .Include(d => d.Professor)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
    public async Task Adicionar(Disciplina disciplina) => await _context.Disciplinas.AddAsync(disciplina);

    public void Atualizar(Disciplina disciplina) => _context.Disciplinas.Update(disciplina);

    public void Deletar(int id)
    {
        var disciplina = _context.Disciplinas.Find(id);
        if (disciplina != null) _context.Disciplinas.Remove(disciplina);
    }
    
    public async Task<Disciplina?> BuscarPorNome(string nome)
    {
        return await _context.Disciplinas
            .Include(d => d.Professor)
            .FirstOrDefaultAsync(d => d.Nome.Contains(nome));
    }

    public async Task<bool> SalvarAlteracoes() => (await _context.SaveChangesAsync()) > 0;
}