using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;

namespace Sprint3.Repositories.Implementations;

public class AlunoRepository : IAlunoRepository
{
    private readonly AppDbContext _context;

    public AlunoRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Aluno>> ListarTodos()
    {
        return await _context.Alunos
            .Include(a => a.Notas)
            .ToListAsync();
    }

    public async Task<Aluno?> BuscarPorId(int id)
    {
        return await _context.Alunos
            .Include(a => a.Notas)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Aluno>> BuscarPorNome(string nome)
    {
        return await _context.Alunos
            .Include(a => a.Notas)
            .Where(a => a.Nome.Contains(nome))
            .ToListAsync();
    }

    public async Task Adicionar(Aluno aluno)
    {
        await _context.Alunos.AddAsync(aluno);
    }

    public void Atualizar(Aluno aluno)
    {
        _context.Alunos.Update(aluno);
    }

    public void Deletar(int id)
    {
        var aluno = _context.Alunos.Find(id);

        if (aluno != null)
        {
            _context.Alunos.Remove(aluno);
        }
    }

    public async Task<bool> SalvarAlteracoes()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}