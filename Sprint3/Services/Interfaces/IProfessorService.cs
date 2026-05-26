using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;

namespace Sprint3.Services.Interfaces;

public interface IProfessorService
{
    Task<IEnumerable<ProfessorResponse>> ListarTodos();
    Task<ProfessorResponse?> BuscarPorId(int id);
    Task<ProfessorResponse?> BuscarPorNome(string nome);
    Task<ProfessorResponse> Criar(ProfessorRequest request);
    Task<bool> Atualizar(int id, ProfessorRequest request);
    Task<bool> Deletar(int id);
}