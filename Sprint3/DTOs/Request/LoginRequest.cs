namespace Sprint3.DTOs.Request;

public record LoginRequest(
    string Email,
    string Senha
);