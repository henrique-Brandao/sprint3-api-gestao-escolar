using Sprint3.Models;

namespace Sprint3.Repositories.Interfaces;

public interface INotaRepository
{
    Task<IEnumerable<Nota>> ListarTodas();
    Task<Nota?> BuscarPorId(int id);
    Task<IEnumerable<Nota>> ListarPorAluno(int alunoId);
    Task Adicionar(Nota nota);
    void Atualizar(Nota nota);
    void Deletar(int id);
    Task<bool> SalvarAlteracoes();
}