namespace Sprint3.DTOs.Response;

public record TokenResponse(
    string Token,
    DateTime Expiracao
);