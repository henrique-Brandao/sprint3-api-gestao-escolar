using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

[Table("Diretores")]
public class Diretor
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
}
