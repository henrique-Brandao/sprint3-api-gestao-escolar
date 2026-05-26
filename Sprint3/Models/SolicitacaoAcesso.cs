using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models;

[Table("SolicitacoesAcesso")]
public class SolicitacaoAcesso
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
    [StringLength(20)]
    public string TipoSolicitado { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Mensagem { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pendente";

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
