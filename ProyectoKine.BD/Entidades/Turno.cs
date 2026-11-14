using ProyectoKine.Shared.Constantes;

namespace ProyectoKine.BD.Entidades;

/// <summary>
/// Representa un turno asignado a un paciente.
/// </summary>
public class Turno
{
    /// <summary>Clave primaria autogenerada (columna IdTurno).</summary>
    public int IdTurno { get; set; }

    /// <summary>Fecha del turno (se guarda como DATE en SQL Server).</summary>
    public DateOnly Fecha { get; set; }

    /// <summary>Hora del turno (se guarda como TIME(0) en SQL Server).</summary>
    public TimeOnly Hora { get; set; }

    /// <summary>Estado del turno: Pendiente o Cancelado.</summary>
    public string Estado { get; set; } = EstadosTurno.Pendiente;

    /// <summary>Clave foránea al paciente.</summary>
    public int IdPaciente { get; set; }

    /// <summary>Paciente al que pertenece el turno.</summary>
    public Paciente Paciente { get; set; } = null!;
}
