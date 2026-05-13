using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;

namespace Sprint3.Services.Interfaces;

public interface INotaService
{
    Task<IEnumerable<NotaResponse>> ListarTodas();
    Task<NotaResponse?> BuscarPorId(int id);
    Task<IEnumerable<NotaResponse>> ListarPorAluno(int alunoId);
    Task<NotaResponse?> Criar(NotaRequest request);
    Task<bool> Atualizar(int id, NotaRequest request);
    Task<bool> Deletar(int id);
}