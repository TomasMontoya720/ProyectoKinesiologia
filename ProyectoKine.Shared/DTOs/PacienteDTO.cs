using System.ComponentModel.DataAnnotations;
using ProyectoKine.Shared.Constantes;

namespace ProyectoKine.Shared.DTOs;

/// <summary>
/// DTO de respuesta con los datos de un paciente.
/// </summary>
public class PacienteDTO : PacienteDatosDTO
{
    public int IdPaciente { get; set; }
}

/// <summary>
/// DTO utilizado para crear o modificar un paciente desde el endpoint de pacientes.
/// </summary>
public class PacienteFormDTO : PacienteDatosDTO, IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // La dirección es obligatoria únicamente para la atención a domicilio.
        if (TipoAtencion == TiposAtencion.Domicilio && string.IsNullOrWhiteSpace(Direccion))
        {
            yield return new ValidationResult(
                "La dirección es obligatoria cuando la atención es a domicilio.",
                new[] { nameof(Direccion) });
        }
    }
}

/// <summary>
/// DTO de respuesta con los datos del paciente y su historial de turnos.
/// </summary>
public class PacienteDetalleDTO : PacienteDTO
{
    public List<TurnoListadoDTO> Turnos { get; set; } = new();
}
