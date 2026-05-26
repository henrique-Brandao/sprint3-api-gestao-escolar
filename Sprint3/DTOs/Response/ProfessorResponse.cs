namespace Sprint3.DTOs.Response;

public record ProfessorResponse(
    int Id,
    string Nome,
    string Email,
    List<string> Disciplinas
);