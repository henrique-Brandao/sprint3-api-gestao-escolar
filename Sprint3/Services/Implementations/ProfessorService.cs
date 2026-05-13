using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;
using Sprint3.Models;
using Sprint3.Repositories;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services.Implementations;

public class ProfessorService : IProfessorService
{
    private readonly IProfessorRepository _professorRepository;

    public ProfessorService(IProfessorRepository professorRepository)
    {
        _professorRepository = professorRepository;
    }

    public async Task<IEnumerable<ProfessorResponse>> ListarTodos()
    {
        var professores = await _professorRepository.ListarTodos();

        return professores.Select(p => MapearParaResponse(p));
    }

    public async Task<ProfessorResponse?> BuscarPorId(int id)
    {
        var professor = await _professorRepository.BuscarPorId(id);

        if (professor == null)
        {
            return null;
        }

        return MapearParaResponse(professor);
    }

    public async Task<ProfessorResponse?> BuscarPorNome(string nome)
    {
        var professor = await _professorRepository.BuscarPorNome(nome);

        if (professor == null)
        {
            return null;
        }

        return MapearParaResponse(professor);
    }

    public async Task<ProfessorResponse> Criar(ProfessorRequest request)
    {
        var professor = new Professor
        {
            Nome = request.Nome,
            Email = request.Email
        };

        await _professorRepository.Adicionar(professor);
        await _professorRepository.SalvarAlteracoes();

        return MapearParaResponse(professor);
    }

    public async Task<bool> Atualizar(int id, ProfessorRequest request)
    {
        var professor = await _professorRepository.BuscarPorId(id);

        if (professor == null)
        {
            return false;
        }

        professor.Nome = request.Nome;
        professor.Email = request.Email;

        _professorRepository.Atualizar(professor);

        return await _professorRepository.SalvarAlteracoes();
    }

    public async Task<bool> Deletar(int id)
    {
        var professor = await _professorRepository.BuscarPorId(id);

        if (professor == null)
        {
            return false;
        }

        _professorRepository.Deletar(professor);

        return await _professorRepository.SalvarAlteracoes();
    }

    private ProfessorResponse MapearParaResponse(Professor professor)
    {
        return new ProfessorResponse(
            professor.Id,
            professor.Nome,
            professor.Email,
            professor.Disciplinas?.Select(d => d.Nome).ToList() ?? new List<string>()
        );
    }
}