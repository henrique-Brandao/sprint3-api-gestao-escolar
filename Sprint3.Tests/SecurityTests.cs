using Sprint3.Security;
using Xunit;

namespace Sprint3.Tests;

public class SecurityTests
{
    [Fact]
    public void PasswordService_VerificaSenhaCorreta_ERejeitaSenhaErrada()
    {
        var service = new PasswordService();

        var hash = service.Hash("123456");

        Assert.True(service.Verify("123456", hash));
        Assert.False(service.Verify("senha-errada", hash));
        Assert.DoesNotContain("123456", hash);
    }

    [Fact]
    public void RegrasDePerfil_DiretorSoPodeCriarAlunoEProfessor()
    {
        Assert.Contains(AppRoles.Aluno, AppRoles.CriaveisPorDiretor);
        Assert.Contains(AppRoles.Professor, AppRoles.CriaveisPorDiretor);
        Assert.DoesNotContain(AppRoles.Diretor, AppRoles.CriaveisPorDiretor);
        Assert.DoesNotContain(AppRoles.Admin, AppRoles.CriaveisPorDiretor);
    }

    [Fact]
    public void RegrasDePerfil_AdminProfessorDiretorAlunoExistem()
    {
        Assert.Contains(AppRoles.Admin, AppRoles.Todos);
        Assert.Contains(AppRoles.Diretor, AppRoles.Todos);
        Assert.Contains(AppRoles.Professor, AppRoles.Todos);
        Assert.Contains(AppRoles.Aluno, AppRoles.Todos);
    }
}
