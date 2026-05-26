using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;

namespace Sprint3.Services.Interfaces;

public interface IDisciplinaService
{
    Task<IEnumerable<DisciplinaResponse>> ListarTodas();
    Task<DisciplinaResponse?> BuscarPorId(int id);
    Task<DisciplinaResponse> Criar(DisciplinaRequest request);
    Task<bool> Atualizar(int id, DisciplinaRequest request);
    Task<bool> Deletar(int id);
}