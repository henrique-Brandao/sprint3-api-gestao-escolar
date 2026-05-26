using Sprint3.Models;

namespace Sprint3.Repositories.Interfaces;

public interface IMatriculaRepository
{
    Task<IEnumerable<Matricula>> ListarTodas();
    Task<IEnumerable<Matricula>> ListarPorAluno(int alunoId);
    Task<IEnumerable<Matricula>> ListarPorDisciplina(int disciplinaId);
    Task<IEnumerable<Matricula>> ListarPorProfessor(int professorId);
    Task<Matricula?> BuscarPorId(int id);
    Task<bool> Existe(int alunoId, int disciplinaId);
    Task Adicionar(Matricula matricula);
    void Atualizar(Matricula matricula);
    void Deletar(Matricula matricula);
    Task<bool> SalvarAlteracoes();
}
