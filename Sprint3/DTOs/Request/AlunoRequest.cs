namespace Sprint3.DTOs.Request;

using System.ComponentModel.DataAnnotations;

public record AlunoRequest(
        [param: Required, StringLength(80)]
        string Nome,
        [param: Required, EmailAddress, StringLength(120)]
        string Email
    );
