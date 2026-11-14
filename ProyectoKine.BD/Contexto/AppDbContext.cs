using Microsoft.EntityFrameworkCore;
using ProyectoKine.BD.Entidades;

namespace ProyectoKine.BD.Contexto;

/// <summary>
/// Contexto de Entity Framework Core. Es el punto de acceso a la base de datos
/// SQL Server y el encargado de aplicar las configuraciones de las entidades.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Turno> Turnos => Set<Turno>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas las clases que implementan IEntityTypeConfiguration
        // dentro de este ensamblado (carpeta Configuraciones).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
