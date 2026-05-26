namespace Sprint3.DTOs.Request;

using System.ComponentModel.DataAnnotations;

public record DisciplinaRequest(
    [param: Required, StringLength(50)]
    string Nome,
    [param: Required, StringLength(80)]
    string ProfessorNome
    );
