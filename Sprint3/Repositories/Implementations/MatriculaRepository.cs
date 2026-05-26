using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;

namespace Sprint3.Repositories.Implementations;

public class MatriculaRepository : IMatriculaRepository
{
    private readonly AppDbContext _context;

    public MatriculaRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Matricula>> ListarTodas()
    {
        return await QueryCompleta()
            .OrderBy(m => m.Aluno!.Nome)
            .ThenBy(m => m.Disciplina!.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Matricula>> ListarPorAluno(int alunoId)
    {
        return await QueryCompleta()
            .Where(m => m.AlunoId == alunoId)
            .OrderBy(m => m.Disciplina!.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Matricula>> ListarPorDisciplina(int disciplinaId)
    {
        return await QueryCompleta()
            .Where(m => m.DisciplinaId == disciplinaId)
            .OrderBy(m => m.Aluno!.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Matricula>> ListarPorProfessor(int professorId)
    {
        return await QueryCompleta()
            .Where(m => m.Disciplina!.ProfessorId == professorId)
            .OrderBy(m => m.Disciplina!.Nome)
            .ThenBy(m => m.Aluno!.Nome)
            .ToListAsync();
    }

    public async Task<Matricula?> BuscarPorId(int id)
    {
        return await QueryCompleta().FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<bool> Existe(int alunoId, int disciplinaId)
    {
        return await _context.Matriculas.AnyAsync(m => m.AlunoId == alunoId && m.DisciplinaId == disciplinaId);
    }

    public async Task Adicionar(Matricula matricula)
    {
        await _context.Matriculas.AddAsync(matricula);
    }

    public void Atualizar(Matricula matricula)
    {
        _context.Matriculas.Update(matricula);
    }

    public void Deletar(Matricula matricula)
    {
        _context.Matriculas.Remove(matricula);
    }

    public async Task<bool> SalvarAlteracoes()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    private IQueryable<Matricula> QueryCompleta()
    {
        return _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Disciplina)
                .ThenInclude(d => d!.Professor)
            .Include(m => m.Aluno!.Notas);
    }
}
