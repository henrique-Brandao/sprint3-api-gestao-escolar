using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

[Table("Professores")]
public class Professor
{
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(120)]
        public string Email { get; set; } = string.Empty;

        public List<Disciplina> Disciplinas { get; set; } = new();
}
