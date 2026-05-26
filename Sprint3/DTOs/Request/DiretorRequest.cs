using System.ComponentModel.DataAnnotations;

namespace Sprint3.DTOs.Request;

public record DiretorRequest(
    [param: Required, StringLength(80)]
    string Nome,
    [param: Required, EmailAddress, StringLength(120)]
    string Email
);
