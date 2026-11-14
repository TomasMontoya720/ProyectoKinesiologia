namespace ProyectoKine.Shared.DTOs;

/// <summary>
/// DTO plano utilizado para mostrar los turnos en la tabla del frontend.
/// Incluye los datos del paciente para evitar consultas adicionales.
/// </summary>
public class TurnoListadoDTO
{
    public int IdTurno { get; set; }
    public int IdPaciente { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Patologia { get; set; } = string.Empty;
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public string TipoAtencion { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string Estado { get; set; } = string.Empty;
}
