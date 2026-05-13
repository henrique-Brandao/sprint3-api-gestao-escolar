namespace Sprint3.DTOs.Response;

public record DisciplinaResponse(
    int Id, 
    string Nome,
    int ProfessorId,
    string ProfessorNome
    );