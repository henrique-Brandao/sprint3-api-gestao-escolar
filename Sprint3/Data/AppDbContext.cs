using Microsoft.EntityFrameworkCore;
using Sprint3.Models;

namespace Sprint3.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Aluno> Alunos { get; set; }
    public DbSet<Professor> Professores { get; set; }
    public DbSet<Nota> Notas { get; set; }
    public DbSet<Disciplina> Disciplinas { get; set; }
    public DbSet<Diretor> Diretores { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<SolicitacaoAcesso> SolicitacoesAcesso { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Aluno>()
            .HasIndex(a => a.Email)
            .IsUnique();

        modelBuilder.Entity<Professor>()
            .HasIndex(p => p.Email)
            .IsUnique();

        modelBuilder.Entity<Diretor>()
            .HasIndex(d => d.Email)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Aluno)
            .WithOne()
            .HasForeignKey<Usuario>(u => u.AlunoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Professor)
            .WithOne()
            .HasForeignKey<Usuario>(u => u.ProfessorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Diretor)
            .WithOne()
            .HasForeignKey<Usuario>(u => u.DiretorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
