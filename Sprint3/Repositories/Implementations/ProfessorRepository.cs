using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;

namespace Sprint3.Repositories;

public class ProfessorRepository : IProfessorRepository
{
    private readonly AppDbContext _context;

    public ProfessorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Professor>> ListarTodos()
    {
        return await _context.Professores
            .Include(p => p.Disciplinas)
            .ToListAsync();
    }

    public async Task<Professor?> BuscarPorId(int id)
    {
        return await _context.Professores
            .Include(p => p.Disciplinas)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Professor?> BuscarPorNome(string nome)
    {
        return await _context.Professores
            .Include(p => p.Disciplinas)
            .FirstOrDefaultAsync(p => p.Nome.Contains(nome));
    }

    public async Task Adicionar(Professor professor)
    {
        await _context.Professores.AddAsync(professor);
    }

    public void Atualizar(Professor professor)
    {
        _context.Professores.Update(professor);
    }

    public void Deletar(Professor professor)
    {
        _context.Professores.Remove(professor);
    }

    public async Task<bool> SalvarAlteracoes()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}