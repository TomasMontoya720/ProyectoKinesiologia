using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoKine.BD.Entidades;
using ProyectoKine.Shared.Constantes;

namespace ProyectoKine.BD.Configuraciones;

/// <summary>
/// Configuración de la tabla Turno y de la relación 1:N con Paciente.
/// </summary>
public class TurnoConfiguracion : IEntityTypeConfiguration<Turno>
{
    public void Configure(EntityTypeBuilder<Turno> builder)
    {
        builder.ToTable("Turno");

        builder.HasKey(t => t.IdTurno);
        builder.Property(t => t.IdTurno).HasColumnName("IdTurno").ValueGeneratedOnAdd();

        builder.Property(t => t.Fecha)
               .HasColumnName("Fecha")
               .HasColumnType("date")
               .IsRequired();

        builder.Property(t => t.Hora)
               .HasColumnName("Hora")
               .HasColumnType("time(0)")
               .IsRequired();

        builder.Property(t => t.Estado)
               .HasColumnName("Estado")
               .HasColumnType("varchar(20)")
               .HasDefaultValue(EstadosTurno.Pendiente)
               .IsRequired();

        builder.Property(t => t.IdPaciente)
               .HasColumnName("IdPaciente")
               .IsRequired();

        // Relación: un paciente tiene muchos turnos y un turno pertenece a un paciente.
        // Se restringe el borrado para no perder el historial de turnos.
        builder.HasOne(t => t.Paciente)
               .WithMany(p => p.Turnos)
               .HasForeignKey(t => t.IdPaciente)
               .HasConstraintName("FK_Turno_Paciente")
               .OnDelete(DeleteBehavior.Restrict);

        // Índice para las consultas por fecha y hora.
        builder.HasIndex(t => new { t.Fecha, t.Hora }).HasDatabaseName("IX_Turno_Fecha_Hora");
    }
}
