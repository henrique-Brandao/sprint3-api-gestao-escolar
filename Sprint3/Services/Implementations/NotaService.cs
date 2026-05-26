using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;
using Sprint3.Exceptions;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services.Implementations;

public class NotaService : INotaService
{
    private readonly INotaRepository _notaRepository;
    private readonly IAlunoRepository _alunoRepository;
    private readonly IDisciplinaRepository _disciplinaRepository;
    private readonly IMatriculaRepository _matriculaRepository;

    public NotaService(
        INotaRepository notaRepository,
        IAlunoRepository alunoRepository,
        IDisciplinaRepository disciplinaRepository,
        IMatriculaRepository matriculaRepository)
    {
        _notaRepository = notaRepository;
        _alunoRepository = alunoRepository;
        _disciplinaRepository = disciplinaRepository;
        _matriculaRepository = matriculaRepository;
    }

    public async Task<IEnumerable<NotaResponse>> ListarTodas()
    {
        var notas = await _notaRepository.ListarTodas();

        return notas.Select(nota => MapearParaResponse(nota));
    }

    public async Task<NotaResponse?> BuscarPorId(int id)
    {
        var nota = await _notaRepository.BuscarPorId(id);

        if (nota == null)
        {
            return null;
        }

        return MapearParaResponse(nota);
    }

    public async Task<IEnumerable<NotaResponse>> ListarPorAluno(int alunoId)
    {
        var notas = await _notaRepository.ListarPorAluno(alunoId);

        return notas.Select(nota => MapearParaResponse(nota));
    }

    public async Task<NotaResponse?> Criar(NotaRequest request)
    {
        if (request.Valor is < 0 or > 10)
        {
            throw new BadRequestException("A nota deve estar entre 0 e 10.");
        }

        var aluno = await ObterAluno(request);
        var disciplina = await ObterDisciplina(request);

        await ValidarMatricula(aluno.Id, disciplina.Id);

        var nota = new Nota
        {
            Valor = request.Valor,
            AlunoId = aluno.Id,
            DisciplinaId = disciplina.Id
        };

        await _notaRepository.Adicionar(nota);
        await _notaRepository.SalvarAlteracoes();

        nota.Aluno = aluno;
        nota.Disciplina = disciplina;

        return MapearParaResponse(nota);
    }

    public async Task<bool> Atualizar(int id, NotaRequest request)
    {
        if (request.Valor is < 0 or > 10)
        {
            throw new BadRequestException("A nota deve estar entre 0 e 10.");
        }

        var notaExistente = await _notaRepository.BuscarPorId(id);

        if (notaExistente == null)
        {
            return false;
        }

        var aluno = await ObterAluno(request);
        var disciplina = await ObterDisciplina(request);

        await ValidarMatricula(aluno.Id, disciplina.Id);

        notaExistente.Valor = request.Valor;
        notaExistente.AlunoId = aluno.Id;
        notaExistente.DisciplinaId = disciplina.Id;

        _notaRepository.Atualizar(notaExistente);

        return await _notaRepository.SalvarAlteracoes();
    }

    public async Task<bool> Deletar(int id)
    {
        var nota = await _notaRepository.BuscarPorId(id);

        if (nota == null)
        {
            return false;
        }

        _notaRepository.Deletar(id);

        return await _notaRepository.SalvarAlteracoes();
    }

    private NotaResponse MapearParaResponse(Nota nota)
    {
        return new NotaResponse(
            nota.Id,
            nota.Valor,
            nota.AlunoId,
            nota.Aluno?.Nome ?? "Aluno não encontrado",
            nota.DisciplinaId,
            nota.Disciplina?.Nome ?? "Disciplina não encontrada"
        );
    }

    private async Task ValidarMatricula(int alunoId, int disciplinaId)
    {
        if (!await _matriculaRepository.Existe(alunoId, disciplinaId))
        {
            throw new BadRequestException("Não é possível lançar nota: o aluno não está matriculado nessa disciplina.");
        }
    }

    private async Task<Aluno> ObterAluno(NotaRequest request)
    {
        if (request.AlunoId.HasValue)
        {
            var alunoPorId = await _alunoRepository.BuscarPorId(request.AlunoId.Value);
            if (alunoPorId != null) return alunoPorId;
        }

        if (!string.IsNullOrWhiteSpace(request.AlunoNome))
        {
            var alunos = await _alunoRepository.BuscarPorNome(request.AlunoNome);
            var alunoPorNome = alunos.FirstOrDefault();
            if (alunoPorNome != null) return alunoPorNome;
        }

        throw new BadRequestException("Aluno não encontrado!");
    }

    private async Task<Disciplina> ObterDisciplina(NotaRequest request)
    {
        if (request.DisciplinaId.HasValue)
        {
            var disciplinaPorId = await _disciplinaRepository.BuscarPorId(request.DisciplinaId.Value);
            if (disciplinaPorId != null) return disciplinaPorId;
        }

        if (!string.IsNullOrWhiteSpace(request.DisciplinaNome))
        {
            var disciplinaPorNome = await _disciplinaRepository.BuscarPorNome(request.DisciplinaNome);
            if (disciplinaPorNome != null) return disciplinaPorNome;
        }

        throw new BadRequestException("Disciplina não encontrada!");
    }
}
