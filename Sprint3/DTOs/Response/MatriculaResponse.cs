namespace Sprint3.DTOs.Response;

public record MatriculaResponse(
    int Id,
    int AlunoId,
    string AlunoNome,
    int DisciplinaId,
    string DisciplinaNome,
    int ProfessorId,
    string ProfessorNome,
    DateTime DataMatricula,
    string Status,
    double? Nota
);
