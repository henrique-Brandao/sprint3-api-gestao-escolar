using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Request;

public record SolicitacaoAcessoRequest(
    [param: Required, StringLength(80)]
    string Nome,
    [param: Required, EmailAddress, StringLength(120)]
    string Email,
    [param: Required]
    string TipoSolicitado,
    [param: StringLength(500)]
    string? Mensagem
);
