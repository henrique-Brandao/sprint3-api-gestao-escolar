namespace Sprint3.DTOs.Response;

public record NotaResponse(
    int Id,
    double Valor,
    int AlunoId,
    string AlunoNome,
    int DisciplinaId,
    string DisciplinaNome
);