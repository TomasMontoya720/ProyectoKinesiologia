namespace ProyectoKine.Shared.DTOs;

/// <summary>
/// DTO de respuesta con todos los datos de un turno y su paciente.
/// </summary>
public class TurnoDetalleDTO
{
    public int IdTurno { get; set; }
    public DateOnly Fecha { get; set; }
    public TimeOnly Hora { get; set; }
    public string Estado { get; set; } = string.Empty;
    public PacienteDTO Paciente { get; set; } = new();
}
