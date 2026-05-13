using Sprint3.Models;

namespace Sprint3.Repositories.Interfaces;

public interface IDisciplinaRepository
{
    Task<IEnumerable<Disciplina>> ListarTodas();
    Task<Disciplina?> BuscarPorId(int id);
    Task Adicionar(Disciplina disciplina);
    void Atualizar(Disciplina disciplina);
    void Deletar(int id);
    Task<bool> SalvarAlteracoes();
    Task<Disciplina?> BuscarPorNome(string nome);
}