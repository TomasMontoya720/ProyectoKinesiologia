namespace ProyectoKine.Shared.Constantes;

/// <summary>
/// Modalidades de atención disponibles para un turno.
/// </summary>
public static class TiposAtencion
{
    public const string Consultorio = "Consultorio";
    public const string Domicilio = "Domicilio";

    /// <summary>Patrón usado por las validaciones de Data Annotations.</summary>
    public const string Patron = "^(Consultorio|Domicilio)$";
}
