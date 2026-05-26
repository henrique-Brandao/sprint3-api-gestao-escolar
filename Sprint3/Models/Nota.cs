using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

[Table("Notas")]
public class Nota
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Range(0, 10)]
    public double Valor { get; set; }

    public int AlunoId { get; set; }

    [ForeignKey("AlunoId")]
    public Aluno? Aluno { get; set; }

    public int DisciplinaId { get; set; }

    [ForeignKey("DisciplinaId")]
    public Disciplina? Disciplina { get; set; }
}
