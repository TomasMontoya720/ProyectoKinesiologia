using System.ComponentModel.DataAnnotations;
using ProyectoKine.Shared.Constantes;

namespace ProyectoKine.Shared.DTOs;

/// <summary>
/// DTO compuesto que recibe el endpoint de turnos para crear o editar un turno
/// junto con los datos del paciente en una sola operación.
/// </summary>
public class TurnoFormDTO : PacienteDatosDTO, IValidatableObject
{
    /// <summary>
    /// Id del paciente cuando ya existe. Si se omite, el servicio lo busca por
    /// teléfono y lo crea si no lo encuentra.
    /// </summary>
    public int? IdPaciente { get; set; }

    /// <summary>Fecha del turno. Se usa nullable para poder validar el campo vacío.</summary>
    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateOnly? Fecha { get; set; }

    /// <summary>Hora del turno. Se usa nullable para poder validar el campo vacío.</summary>
    [Required(ErrorMessage = "La hora es obligatoria.")]
    public TimeOnly? Hora { get; set; }

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
