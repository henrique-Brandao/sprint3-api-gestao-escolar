using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Request;

public record AprovarSolicitacaoRequest(
    [param: MinLength(6)]
    string? SenhaInicial
);
