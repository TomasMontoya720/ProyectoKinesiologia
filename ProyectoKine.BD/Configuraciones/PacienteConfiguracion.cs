using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProyectoKine.BD.Entidades;

namespace ProyectoKine.BD.Configuraciones;

/// <summary>
/// Configuración de la tabla Paciente: nombres, tipos de datos SQL y longitudes.
/// </summary>
public class PacienteConfiguracion : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Paciente");

        builder.HasKey(p => p.IdPaciente);
        builder.Property(p => p.IdPaciente).HasColumnName("IdPaciente").ValueGeneratedOnAdd();

        builder.Property(p => p.Nombre)
               .HasColumnName("Nombre")
               .HasColumnType("varchar(50)")
               .IsRequired();

        builder.Property(p => p.Edad)
               .HasColumnName("Edad")
               .IsRequired();

        builder.Property(p => p.Sexo)
               .HasColumnName("Sexo")
               .HasColumnType("varchar(20)")
               .IsRequired();

        builder.Property(p => p.Telefono)
               .HasColumnName("Telefono")
               .HasColumnType("varchar(20)")
               .IsRequired();

        builder.Property(p => p.Patologia)
               .HasColumnName("Patologia")
               .HasColumnType("varchar(200)")
               .IsRequired();

        builder.Property(p => p.TipoAtencion)
               .HasColumnName("TipoAtencion")
               .HasColumnType("varchar(20)")
               .IsRequired();

        builder.Property(p => p.Direccion)
               .HasColumnName("Direccion")
               .HasColumnType("varchar(100)")
               .IsRequired(false);

        // Índice para acelerar la búsqueda de pacientes por teléfono.
        // No es único porque distintas personas podrían compartir un teléfono.
        builder.HasIndex(p => p.Telefono).HasDatabaseName("IX_Paciente_Telefono");
    }
}
