using System.ComponentModel.DataAnnotations;

namespace ProyectoKine.Shared.DTOs;

/// <summary>
/// Datos comunes de un paciente. Se utiliza como base para los DTO de alta,
/// edición y respuesta, de modo de no repetir las validaciones.
/// </summary>
public class PacienteDatosDTO
{
    /// <summary>Nombre y apellido del paciente.</summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Edad del paciente en años.</summary>
    [Range(1, 120, ErrorMessage = "La edad debe estar entre 1 y 120 años.")]
    public int Edad { get; set; }

    /// <summary>Sexo del paciente (Masculino, Femenino, Otro).</summary>
    [Required(ErrorMessage = "El sexo es obligatorio.")]
    [StringLength(20, ErrorMessage = "El sexo no puede superar los 20 caracteres.")]
    public string Sexo { get; set; } = string.Empty;

    /// <summary>Teléfono de contacto.</summary>
    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "El teléfono debe tener entre 6 y 20 caracteres.")]
    [RegularExpression(@"^[0-9+\-\s()]+$", ErrorMessage = "El teléfono solo puede contener números, espacios, +, - y paréntesis.")]
    public string Telefono { get; set; } = string.Empty;

    /// <summary>Patología o motivo de consulta.</summary>
    [Required(ErrorMessage = "La patología es obligatoria.")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "La patología debe tener entre 3 y 200 caracteres.")]
    public string Patologia { get; set; } = string.Empty;

    /// <summary>Modalidad de atención: Consultorio o Domicilio.</summary>
    [Required(ErrorMessage = "El tipo de atención es obligatorio.")]
    [RegularExpression("^(Consultorio|Domicilio)$", ErrorMessage = "El tipo de atención debe ser Consultorio o Domicilio.")]
    public string TipoAtencion { get; set; } = Constantes.TiposAtencion.Consultorio;

    /// <summary>Dirección del domicilio. Solo se exige cuando la atención es a domicilio.</summary>
    [StringLength(100, ErrorMessage = "La dirección no puede superar los 100 caracteres.")]
    public string? Direccion { get; set; }
}
