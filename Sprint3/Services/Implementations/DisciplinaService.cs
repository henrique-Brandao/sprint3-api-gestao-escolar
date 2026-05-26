using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;
using Sprint3.Models;
using Sprint3.Exceptions;
using Sprint3.Repositories;
using Sprint3.Repositories.Interfaces;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services.Implementations;

public class DisciplinaService : IDisciplinaService
{
    private readonly IDisciplinaRepository _disciplinaRepository;
    private readonly IProfessorRepository _professorRepository;

    public DisciplinaService(IDisciplinaRepository disciplinaRepository, IProfessorRepository professorRepository)
    {
        _disciplinaRepository = disciplinaRepository;
        _professorRepository = professorRepository;
    }

    public async Task<IEnumerable<DisciplinaResponse>> ListarTodas()
    {
        var disciplinas = await _disciplinaRepository.ListarTodas();
        return disciplinas.Select(d => new DisciplinaResponse(
            d.Id, 
            d.Nome, 
            d.ProfessorId, 
            d.Professor?.Nome ?? "Sem Professor"
        ));
    }

    public async Task<DisciplinaResponse?> BuscarPorId(int id)
    {
        var d = await _disciplinaRepository.BuscarPorId(id);
        if (d == null) return null;

        return new DisciplinaResponse(
            d.Id, 
            d.Nome, 
            d.ProfessorId, 
            d.Professor?.Nome ?? "Sem Professor"
        );
    }

    public async Task<DisciplinaResponse> Criar(DisciplinaRequest request)
    {
        var professor = await _professorRepository.BuscarPorId(request.ProfessorId);
        
        if (professor == null) throw new BadRequestException("Professor não encontrado!");

        var disciplina = new Disciplina 
        { 
            Nome = request.Nome, 
            ProfessorId = professor.Id 
        };

        await _disciplinaRepository.Adicionar(disciplina);
        await _disciplinaRepository.SalvarAlteracoes();

        return new DisciplinaResponse(disciplina.Id, disciplina.Nome, professor.Id, professor.Nome);
    }

    public async Task<bool> Atualizar(int id, DisciplinaRequest request)
    {
        var disciplinaExistente = await _disciplinaRepository.BuscarPorId(id);
        if (disciplinaExistente == null) return false;

        var professor = await _professorRepository.BuscarPorId(request.ProfessorId);
        if (professor == null) throw new BadRequestException("Professor não encontrado!");

        disciplinaExistente.Nome = request.Nome;
        disciplinaExistente.ProfessorId = professor.Id;

        _disciplinaRepository.Atualizar(disciplinaExistente);
        return await _disciplinaRepository.SalvarAlteracoes();
    }

    public async Task<bool> Deletar(int id)
    {
        _disciplinaRepository.Deletar(id);
        return await _disciplinaRepository.SalvarAlteracoes();
    }
}
