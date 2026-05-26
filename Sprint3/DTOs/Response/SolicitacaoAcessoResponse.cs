namespace Sprint3.DTOs.Response;

public record SolicitacaoAcessoResponse(
    int Id,
    string Nome,
    string Email,
    string TipoSolicitado,
    string? Mensagem,
    string Status,
    DateTime CriadoEm
);
