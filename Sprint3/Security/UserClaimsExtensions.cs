using System.Security.Claims;

namespace Sprint3.Security;

public static class UserClaimsExtensions
{
    public static int? GetIntClaim(this ClaimsPrincipal user, string type)
    {
        var value = user.FindFirstValue(type);
        return int.TryParse(value, out var id) ? id : null;
    }

    public static string? Role(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Role);

    public static int? UsuarioId(this ClaimsPrincipal user) =>
        user.GetIntClaim("usuarioId");

    public static int? AlunoId(this ClaimsPrincipal user) =>
        user.GetIntClaim("alunoId");

    public static int? ProfessorId(this ClaimsPrincipal user) =>
        user.GetIntClaim("professorId");

    public static int? DiretorId(this ClaimsPrincipal user) =>
        user.GetIntClaim("diretorId");
}
