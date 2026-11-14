namespace ProyectoKine.Shared.Constantes;

/// <summary>
/// Valores posibles para el estado de un turno.
/// Se usan constantes para evitar errores de tipeo en todo el sistema.
/// </summary>
public static class EstadosTurno
{
    public const string Pendiente = "Pendiente";
    public const string Cancelado = "Cancelado";

    /// <summary>Patrón usado por las validaciones de Data Annotations.</summary>
    public const string Patron = "^(Pendiente|Cancelado)$";
}
