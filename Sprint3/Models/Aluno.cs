using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

[Table("Aluno")]
public class Aluno
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Nome { get; set; } = string.Empty;
    
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public List<Nota> Notas { get; set; } = new();

}