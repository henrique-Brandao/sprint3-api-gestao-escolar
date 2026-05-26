using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;

namespace Sprint3.Services.Interfaces;

public interface IMatriculaService
{
    Task<IEnumerable<MatriculaResponse>> ListarTodas();
    Task<IEnumerable<MatriculaResponse>> ListarPorAluno(int alunoId);
    Task<IEnumerable<MatriculaResponse>> ListarPorDisciplina(int disciplinaId);
    Task<IEnumerable<MatriculaResponse>> ListarPorProfessor(int professorId);
    Task<MatriculaResponse?> BuscarPorId(int id);
    Task<MatriculaResponse> Criar(MatriculaRequest request);
    Task<bool> Atualizar(int id, MatriculaRequest request);
    Task<bool> Deletar(int id);
}
