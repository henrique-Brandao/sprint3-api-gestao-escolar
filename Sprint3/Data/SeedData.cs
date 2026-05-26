using Microsoft.EntityFrameworkCore;
using Sprint3.Models;
using Sprint3.Security;

namespace Sprint3.Data;

public static class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

        var diretor = await EnsureDiretor(context, "Helena Martins", "diretor@sprint3.com");

        var professores = new[]
        {
            await EnsureProfessor(context, "Mariana Costa", "professor@sprint3.com"),
            await EnsureProfessor(context, "Roberto Lima", "roberto.lima@sprint3.com"),
            await EnsureProfessor(context, "Camila Fernandes", "camila.fernandes@sprint3.com"),
        };

        var alunos = new[]
        {
            await EnsureAluno(context, "Joao Silva", "aluno@sprint3.com"),
            await EnsureAluno(context, "Lucas Almeida", "lucas.almeida@sprint3.com"),
            await EnsureAluno(context, "Maria Oliveira", "maria.oliveira@sprint3.com"),
            await EnsureAluno(context, "Ana Souza", "ana.souza@sprint3.com"),
            await EnsureAluno(context, "Pedro Santos", "pedro.santos@sprint3.com"),
        };

        var matematica = await EnsureDisciplina(context, "Matematica", professores[0].Id);
        var fisica = await EnsureDisciplina(context, "Fisica", professores[1].Id);
        var historia = await EnsureDisciplina(context, "Historia", professores[2].Id);
        var biologia = await EnsureDisciplina(context, "Biologia", professores[2].Id);

        await EnsureMatricula(context, alunos[0].Id, matematica.Id);
        await EnsureMatricula(context, alunos[0].Id, fisica.Id);
        await EnsureMatricula(context, alunos[1].Id, matematica.Id);
        await EnsureMatricula(context, alunos[1].Id, historia.Id);
        await EnsureMatricula(context, alunos[2].Id, fisica.Id);
        await EnsureMatricula(context, alunos[2].Id, biologia.Id);
        await EnsureMatricula(context, alunos[3].Id, historia.Id);
        await EnsureMatricula(context, alunos[3].Id, biologia.Id);
        await EnsureMatricula(context, alunos[4].Id, matematica.Id);
        await EnsureMatricula(context, alunos[4].Id, fisica.Id);

        await EnsureNota(context, alunos[0].Id, matematica.Id, 8.5);
        await EnsureNota(context, alunos[0].Id, fisica.Id, 7.2);
        await EnsureNota(context, alunos[1].Id, matematica.Id, 6.4);
        await EnsureNota(context, alunos[1].Id, historia.Id, 8.0);
        await EnsureNota(context, alunos[2].Id, fisica.Id, 9.1);
        await EnsureNota(context, alunos[3].Id, historia.Id, 5.8);
        await EnsureNota(context, alunos[4].Id, matematica.Id, 7.7);

        await EnsureUsuario(context, passwordService, diretor.Nome, diretor.Email, AppRoles.Diretor, null, null, diretor.Id);

        foreach (var professor in professores)
        {
            await EnsureUsuario(context, passwordService, professor.Nome, professor.Email, AppRoles.Professor, null, professor.Id, null);
        }

        foreach (var aluno in alunos)
        {
            await EnsureUsuario(context, passwordService, aluno.Nome, aluno.Email, AppRoles.Aluno, aluno.Id, null, null);
        }
    }

    private static async Task<Aluno> EnsureAluno(AppDbContext context, string nome, string email)
    {
        var aluno = await context.Alunos.FirstOrDefaultAsync(a => a.Email == email);
        if (aluno == null)
        {
            aluno = new Aluno { Nome = nome, Email = email };
            context.Alunos.Add(aluno);
        }
        else
        {
            aluno.Nome = nome;
        }

        await context.SaveChangesAsync();
        return aluno;
    }

    private static async Task<Professor> EnsureProfessor(AppDbContext context, string nome, string email)
    {
        var professor = await context.Professores.FirstOrDefaultAsync(p => p.Email == email);
        if (professor == null)
        {
            professor = new Professor { Nome = nome, Email = email };
            context.Professores.Add(professor);
        }
        else
        {
            professor.Nome = nome;
        }

        await context.SaveChangesAsync();
        return professor;
    }

    private static async Task<Diretor> EnsureDiretor(AppDbContext context, string nome, string email)
    {
        var diretor = await context.Diretores.FirstOrDefaultAsync(d => d.Email == email);
        if (diretor == null)
        {
            diretor = new Diretor { Nome = nome, Email = email };
            context.Diretores.Add(diretor);
        }
        else
        {
            diretor.Nome = nome;
        }

        await context.SaveChangesAsync();
        return diretor;
    }

    private static async Task<Disciplina> EnsureDisciplina(AppDbContext context, string nome, int professorId)
    {
        var disciplina = await context.Disciplinas.FirstOrDefaultAsync(d => d.Nome == nome);
        if (disciplina == null)
        {
            disciplina = new Disciplina { Nome = nome, ProfessorId = professorId };
            context.Disciplinas.Add(disciplina);
        }
        else
        {
            disciplina.ProfessorId = professorId;
        }

        await context.SaveChangesAsync();
        return disciplina;
    }

    private static async Task EnsureMatricula(AppDbContext context, int alunoId, int disciplinaId)
    {
        if (await context.Matriculas.AnyAsync(m => m.AlunoId == alunoId && m.DisciplinaId == disciplinaId))
        {
            return;
        }

        context.Matriculas.Add(new Matricula
        {
            AlunoId = alunoId,
            DisciplinaId = disciplinaId,
            DataMatricula = DateTime.UtcNow,
            Status = "Ativa"
        });

        await context.SaveChangesAsync();
    }

    private static async Task EnsureNota(AppDbContext context, int alunoId, int disciplinaId, double valor)
    {
        if (await context.Notas.AnyAsync(n => n.AlunoId == alunoId && n.DisciplinaId == disciplinaId))
        {
            return;
        }

        context.Notas.Add(new Nota
        {
            AlunoId = alunoId,
            DisciplinaId = disciplinaId,
            Valor = valor
        });

        await context.SaveChangesAsync();
    }

    private static async Task EnsureUsuario(
        AppDbContext context,
        IPasswordService passwordService,
        string nome,
        string email,
        string role,
        int? alunoId,
        int? professorId,
        int? diretorId)
    {
        var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        if (usuario == null)
        {
            context.Usuarios.Add(new Usuario
            {
                Nome = nome,
                Email = email,
                Role = role,
                AlunoId = alunoId,
                ProfessorId = professorId,
                DiretorId = diretorId,
                SenhaHash = passwordService.Hash("123456")
            });
        }
        else
        {
            usuario.Nome = nome;
            usuario.Role = role;
            usuario.AlunoId = alunoId;
            usuario.ProfessorId = professorId;
            usuario.DiretorId = diretorId;
        }

        await context.SaveChangesAsync();
    }
}
