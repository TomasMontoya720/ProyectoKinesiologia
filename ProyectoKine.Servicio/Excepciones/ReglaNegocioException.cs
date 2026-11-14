namespace ProyectoKine.Servicio.Excepciones;

/// <summary>
/// Excepción para reglas de negocio que deben devolver HTTP 409 (Conflict),
/// por ejemplo: turno duplicado o paciente con turnos asociados.
/// </summary>
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}
