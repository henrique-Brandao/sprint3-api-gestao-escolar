using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;
using Sprint3.Exceptions;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services.Implementations;

public class MatriculaService : IMatriculaService
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IAlunoRepository _alunoRepository;
    private readonly IDisciplinaRepository _disciplinaRepository;

    public MatriculaService(
        IMatriculaRepository matriculaRepository,
        IAlunoRepository alunoRepository,
        IDisciplinaRepository disciplinaRepository)
    {
        _matriculaRepository = matriculaRepository;
        _alunoRepository = alunoRepository;
        _disciplinaRepository = disciplinaRepository;
    }

    public async Task<IEnumerable<MatriculaResponse>> ListarTodas()
    {
        var matriculas = await _matriculaRepository.ListarTodas();
        return matriculas.Select(MapearParaResponse);
    }

    public async Task<IEnumerable<MatriculaResponse>> ListarPorAluno(int alunoId)
    {
        var matriculas = await _matriculaRepository.ListarPorAluno(alunoId);
        return matriculas.Select(MapearParaResponse);
    }

    public async Task<IEnumerable<MatriculaResponse>> ListarPorDisciplina(int disciplinaId)
    {
        var matriculas = await _matriculaRepository.ListarPorDisciplina(disciplinaId);
        return matriculas.Select(MapearParaResponse);
    }

    public async Task<IEnumerable<MatriculaResponse>> ListarPorProfessor(int professorId)
    {
        var matriculas = await _matriculaRepository.ListarPorProfessor(professorId);
        return matriculas.Select(MapearParaResponse);
    }

    public async Task<MatriculaResponse?> BuscarPorId(int id)
    {
        var matricula = await _matriculaRepository.BuscarPorId(id);
        return matricula == null ? null : MapearParaResponse(matricula);
    }

    public async Task<MatriculaResponse> Criar(MatriculaRequest request)
    {
        await ValidarAlunoDisciplina(request.AlunoId, request.DisciplinaId);

        if (await _matriculaRepository.Existe(request.AlunoId, request.DisciplinaId))
        {
            throw new BadRequestException("Este aluno já está matriculado nessa disciplina.");
        }

        var matricula = new Matricula
        {
            AlunoId = request.AlunoId,
            DisciplinaId = request.DisciplinaId,
            DataMatricula = DateTime.UtcNow,
            Status = NormalizarStatus(request.Status)
        };

        await _matriculaRepository.Adicionar(matricula);
        await _matriculaRepository.SalvarAlteracoes();

        var matriculaCompleta = await _matriculaRepository.BuscarPorId(matricula.Id);
        return MapearParaResponse(matriculaCompleta!);
    }

    public async Task<bool> Atualizar(int id, MatriculaRequest request)
    {
        var matricula = await _matriculaRepository.BuscarPorId(id);
        if (matricula == null) return false;

        await ValidarAlunoDisciplina(request.AlunoId, request.DisciplinaId);

        if ((matricula.AlunoId != request.AlunoId || matricula.DisciplinaId != request.DisciplinaId)
            && await _matriculaRepository.Existe(request.AlunoId, request.DisciplinaId))
        {
            throw new BadRequestException("Este aluno já está matriculado nessa disciplina.");
        }

        matricula.AlunoId = request.AlunoId;
        matricula.DisciplinaId = request.DisciplinaId;
        matricula.Status = NormalizarStatus(request.Status);

        _matriculaRepository.Atualizar(matricula);
        return await _matriculaRepository.SalvarAlteracoes();
    }

    public async Task<bool> Deletar(int id)
    {
        var matricula = await _matriculaRepository.BuscarPorId(id);
        if (matricula == null) return false;

        _matriculaRepository.Deletar(matricula);
        return await _matriculaRepository.SalvarAlteracoes();
    }

    private async Task ValidarAlunoDisciplina(int alunoId, int disciplinaId)
    {
        if (await _alunoRepository.BuscarPorId(alunoId) == null)
        {
            throw new BadRequestException("Aluno não encontrado.");
        }

        if (await _disciplinaRepository.BuscarPorId(disciplinaId) == null)
        {
            throw new BadRequestException("Disciplina não encontrada.");
        }
    }

    private static string NormalizarStatus(string? status)
    {
        return string.IsNullOrWhiteSpace(status) ? "Ativa" : status.Trim();
    }

    private static MatriculaResponse MapearParaResponse(Matricula matricula)
    {
        var nota = matricula.Aluno?.Notas
            .Where(n => n.DisciplinaId == matricula.DisciplinaId)
            .OrderByDescending(n => n.Id)
            .Select(n => (double?)n.Valor)
            .FirstOrDefault();

        return new MatriculaResponse(
            matricula.Id,
            matricula.AlunoId,
            matricula.Aluno?.Nome ?? "Aluno não encontrado",
            matricula.DisciplinaId,
            matricula.Disciplina?.Nome ?? "Disciplina não encontrada",
            matricula.Disciplina?.ProfessorId ?? 0,
            matricula.Disciplina?.Professor?.Nome ?? "Sem Professor",
            matricula.DataMatricula,
            matricula.Status,
            nota
        );
    }
}
