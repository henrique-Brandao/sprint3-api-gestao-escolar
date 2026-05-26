namespace Sprint3.DTOs.Response;

public record AprovacaoSolicitacaoResponse(
    SolicitacaoAcessoResponse Solicitacao,
    UsuarioResponse Usuario,
    string SenhaTemporaria
);
