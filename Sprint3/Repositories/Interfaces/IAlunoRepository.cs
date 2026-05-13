using Sprint3.Models;

namespace Sprint3.Repositories.Interfaces;

public interface IAlunoRepository
{
    Task<IEnumerable<Aluno>> ListarTodos();
    Task<Aluno?> BuscarPorId(int id);
    Task<IEnumerable<Aluno>> BuscarPorNome(string nome);
    Task Adicionar(Aluno aluno);
    void Atualizar(Aluno aluno);
    void Deletar(int id);
    Task<bool> SalvarAlteracoes();
}