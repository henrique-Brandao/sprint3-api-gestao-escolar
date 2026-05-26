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

    public NotaService(
        INotaRepository notaRepository,
        IAlunoRepository alunoRepository,
        IDisciplinaRepository disciplinaRepository)
    {
        _notaRepository = notaRepository;
        _alunoRepository = alunoRepository;
        _disciplinaRepository = disciplinaRepository;
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

        var alunos = await _alunoRepository.BuscarPorNome(request.AlunoNome);
        var aluno = alunos.FirstOrDefault();

        if (aluno == null)
        {
            throw new BadRequestException("Aluno não encontrado!");
        }

        var disciplina = await _disciplinaRepository.BuscarPorNome(request.DisciplinaNome);

        if (disciplina == null)
        {
            throw new BadRequestException("Disciplina não encontrada!");
        }

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

        var alunos = await _alunoRepository.BuscarPorNome(request.AlunoNome);
        var aluno = alunos.FirstOrDefault();

        if (aluno == null)
        {
            throw new BadRequestException("Aluno não encontrado!");
        }

        var disciplina = await _disciplinaRepository.BuscarPorNome(request.DisciplinaNome);

        if (disciplina == null)
        {
            throw new BadRequestException("Disciplina não encontrada!");
        }

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
}
