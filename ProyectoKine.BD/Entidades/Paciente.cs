namespace ProyectoKine.BD.Entidades;

/// <summary>
/// Representa a un paciente de la kinesióloga.
/// Un paciente puede tener muchos turnos.
/// </summary>
public class Paciente
{
    /// <summary>Clave primaria autogenerada (columna IdPaciente).</summary>
    public int IdPaciente { get; set; }

    public string Nombre { get; set; } = string.Empty;

    /// <summary>Edad en años.</summary>
    public int Edad { get; set; }

    public string Sexo { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Patologia { get; set; } = string.Empty;

    /// <summary>Modalidad de atención actual del paciente: Consultorio o Domicilio.</summary>
    public string TipoAtencion { get; set; } = string.Empty;

    /// <summary>Dirección del domicilio (solo si la atención es a domicilio).</summary>
    public string? Direccion { get; set; }

    /// <summary>Turnos asociados al paciente (relación 1:N).</summary>
    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
