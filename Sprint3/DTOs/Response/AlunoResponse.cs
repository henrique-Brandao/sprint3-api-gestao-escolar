using Sprint3.Models;

namespace Sprint3.DTOs.Response;

public record AlunoResponse(
    int Id,
    String Nome,
    String Email,
    List<double> Notas,
    double Media,
    string Situacao
    );