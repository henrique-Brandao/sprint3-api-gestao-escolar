namespace Sprint3.DTOs.Request;

using System.ComponentModel.DataAnnotations;

public record NotaRequest(
    [param: Range(0, 10)]
    double Valor,
    int? AlunoId,
    int? DisciplinaId,
    [param: StringLength(80)]
    string? AlunoNome,
    [param: StringLength(50)]
    string? DisciplinaNome
);
