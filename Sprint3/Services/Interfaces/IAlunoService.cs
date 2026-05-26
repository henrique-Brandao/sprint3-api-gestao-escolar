using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;

namespace Sprint3.Services.Interfaces;

public interface IAlunoService
{
    Task<IEnumerable<AlunoResponse>> ListarTodos();
    Task<AlunoResponse?> BuscarPorId(int id);
    Task<IEnumerable<AlunoResponse>> BuscarPorNome(string nome);
    Task<AlunoResponse> Criar(AlunoRequest request);
    Task<bool> Atualizar(int id, AlunoRequest request);
    Task<bool> Deletar(int id);
}