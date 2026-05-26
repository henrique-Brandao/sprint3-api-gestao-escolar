using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Request;

public record MatriculaRequest(
    [param: Required]
    int AlunoId,
    [param: Required]
    int DisciplinaId,
    string? Status
);
