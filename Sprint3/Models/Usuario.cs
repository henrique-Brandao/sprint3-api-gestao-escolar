using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

[Table("Usuarios")]
public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string SenhaHash { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = string.Empty;

    public int? AlunoId { get; set; }
    public Aluno? Aluno { get; set; }

    public int? ProfessorId { get; set; }
    public Professor? Professor { get; set; }

    public int? DiretorId { get; set; }
    public Diretor? Diretor { get; set; }
}
