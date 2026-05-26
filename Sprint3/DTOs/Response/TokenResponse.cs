namespace Sprint3.DTOs.Response;

public record TokenResponse(
    string Token,
    DateTime Expiracao,
    string Role,
    int UsuarioId,
    int? AlunoId,
    int? ProfessorId,
    int? DiretorId,
    string Nome
);
