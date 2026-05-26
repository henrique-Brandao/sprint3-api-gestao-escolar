namespace Sprint3.DTOs.Response;

public record UsuarioResponse(
    int Id,
    string Nome,
    string Email,
    string Role,
    int? AlunoId,
    int? ProfessorId,
    int? DiretorId
);
