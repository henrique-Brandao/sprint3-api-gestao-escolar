using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Request;

public record UsuarioRequest(
    [param: Required, StringLength(80)]
    string Nome,
    [param: Required, EmailAddress, StringLength(120)]
    string Email,
    [param: Required, MinLength(6)]
    string Senha,
    [param: Required]
    string Role,
    int? AlunoId,
    int? ProfessorId,
    int? DiretorId
);
