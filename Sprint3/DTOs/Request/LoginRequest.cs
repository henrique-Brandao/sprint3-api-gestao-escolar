namespace Sprint3.DTOs.Request;

using System.ComponentModel.DataAnnotations;

public record LoginRequest(
    [param: Required, EmailAddress]
    string Email,
    [param: Required]
    string Senha
);
