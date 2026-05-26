namespace Sprint3.DTOs.Request;

using System.ComponentModel.DataAnnotations;

public record NotaRequest(
    [param: Range(0, 10)]
    double Valor,
    [param: Required, StringLength(80)]
    string AlunoNome,
    [param: Required, StringLength(50)]
    string DisciplinaNome
);
