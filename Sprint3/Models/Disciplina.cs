using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

[Table("Disciplinas")]
public class Disciplina
{
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Informe o nome da disciplina")]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
        
        public int ProfessorId { get; set; }
        public Professor? Professor { get; set; }
        
        public List<Nota> Notas { get; set; } = new();
        public List<Matricula> Matriculas { get; set; } = new();
}
