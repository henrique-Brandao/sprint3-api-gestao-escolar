using Sprint3.Models;

namespace Sprint3.Repositories;

public interface IProfessorRepository
{
    Task<IEnumerable<Professor>> ListarTodos();
    Task<Professor?> BuscarPorId(int id);
    Task<Professor?> BuscarPorNome(string nome);
    Task Adicionar(Professor professor);
    void Atualizar(Professor professor);
    void Deletar(Professor professor);
    Task<bool> SalvarAlteracoes();
}