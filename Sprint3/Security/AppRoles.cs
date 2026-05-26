namespace Sprint3.Security;

public static class AppRoles
{
    public const string Aluno = "Aluno";
    public const string Professor = "Professor";
    public const string Diretor = "Diretor";
    public const string Admin = "Admin";

    public static readonly string[] Todos = [Aluno, Professor, Diretor, Admin];
    public static readonly string[] CriaveisPorDiretor = [Aluno, Professor];
}
