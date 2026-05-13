using System.Linq;
using Sprint3.DTOs.Request;
using Sprint3.DTOs.Response;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services.Implementations;

public class AlunoService : IAlunoService
{
    private readonly IAlunoRepository _alunoRepository;

    public AlunoService(IAlunoRepository alunoRepository)
    {
        _alunoRepository = alunoRepository;
    }

    public async Task<IEnumerable<AlunoResponse>> ListarTodos()
    {
        var alunos = await _alunoRepository.ListarTodos();

        return alunos.Select(aluno => MapearParaResponse(aluno));
    }

    public async Task<AlunoResponse?> BuscarPorId(int id)
    {
        var aluno = await _alunoRepository.BuscarPorId(id);

        if (aluno == null)
        {
            return null;
        }

        return MapearParaResponse(aluno);
    }

    public async Task<IEnumerable<AlunoResponse>> BuscarPorNome(string nome)
    {
        var alunos = await _alunoRepository.BuscarPorNome(nome);

        return alunos.Select(aluno => MapearParaResponse(aluno));
    }

    public async Task<AlunoResponse> Criar(AlunoRequest request)
    {
        var aluno = new Aluno
        {
            Nome = request.Nome,
            Email = request.Email
        };

        await _alunoRepository.Adicionar(aluno);
        await _alunoRepository.SalvarAlteracoes();

        return MapearParaResponse(aluno);
    }

    public async Task<bool> Atualizar(int id, AlunoRequest request)
    {
        var alunoExistente = await _alunoRepository.BuscarPorId(id);

        if (alunoExistente == null)
        {
            return false;
        }

        alunoExistente.Nome = request.Nome;
        alunoExistente.Email = request.Email;

        _alunoRepository.Atualizar(alunoExistente);

        return await _alunoRepository.SalvarAlteracoes();
    }

    public async Task<bool> Deletar(int id)
    {
        _alunoRepository.Deletar(id);

        return await _alunoRepository.SalvarAlteracoes();
    }

    private AlunoResponse MapearParaResponse(Aluno aluno)
    {
        var listaDeNotas = aluno.Notas?.Select(n => n.Valor).ToList() ?? new List<double>();

        double media = listaDeNotas.Any()
            ? Math.Round(listaDeNotas.Average(), 2)
            : 0;

        string situacao = media >= 7.0
            ? "Aprovado"
            : listaDeNotas.Any()
                ? "Reprovado"
                : "Sem Notas";

        return new AlunoResponse(
            aluno.Id,
            aluno.Nome,
            aluno.Email,
            listaDeNotas,
            media,
            situacao
        );
    }
}