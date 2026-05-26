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

        if (!await context.Alunos.AnyAsync(a => a.Email == "aluno@sprint3.com"))
        {
            context.Alunos.Add(new Aluno { Nome = "Aluno Teste", Email = "aluno@sprint3.com" });
        }

        if (!await context.Professores.AnyAsync(p => p.Email == "professor@sprint3.com"))
        {
            context.Professores.Add(new Professor { Nome = "Professor Teste", Email = "professor@sprint3.com" });
        }

        if (!await context.Diretores.AnyAsync(d => d.Email == "diretor@sprint3.com"))
        {
            context.Diretores.Add(new Diretor { Nome = "Diretor Teste", Email = "diretor@sprint3.com" });
        }

        await context.SaveChangesAsync();

        var aluno = await context.Alunos.FirstAsync(a => a.Email == "aluno@sprint3.com");
        var professor = await context.Professores.FirstAsync(p => p.Email == "professor@sprint3.com");
        var diretor = await context.Diretores.FirstAsync(d => d.Email == "diretor@sprint3.com");

        await EnsureUsuario(context, passwordService, "Admin Teste", "admin@sprint3.com", AppRoles.Admin, null, null, null);
        await EnsureUsuario(context, passwordService, aluno.Nome, aluno.Email, AppRoles.Aluno, aluno.Id, null, null);
        await EnsureUsuario(context, passwordService, professor.Nome, professor.Email, AppRoles.Professor, null, professor.Id, null);
        await EnsureUsuario(context, passwordService, diretor.Nome, diretor.Email, AppRoles.Diretor, null, null, diretor.Id);
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
        if (await context.Usuarios.AnyAsync(u => u.Email == email))
        {
            return;
        }

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

        await context.SaveChangesAsync();
    }
}
