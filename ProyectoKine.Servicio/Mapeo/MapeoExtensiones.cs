using ProyectoKine.BD.Entidades;
using ProyectoKine.Shared.Constantes;
using ProyectoKine.Shared.DTOs;

namespace ProyectoKine.Servicio.Mapeo;

/// <summary>
/// Métodos de extensión para convertir entidades a DTO y para aplicar los datos
/// de un formulario sobre una entidad, normalizando los campos.
/// </summary>
public static class MapeoExtensiones
{
    /// <summary>Copia los datos del formulario sobre la entidad Paciente.</summary>
    public static void AplicarDatos(this Paciente paciente, PacienteDatosDTO dto)
    {
        paciente.Nombre = dto.Nombre.Trim();
        paciente.Edad = dto.Edad;
        paciente.Sexo = dto.Sexo.Trim();
        paciente.Telefono = dto.Telefono.Trim();
        paciente.Patologia = dto.Patologia.Trim();
        paciente.TipoAtencion = dto.TipoAtencion;

        // La dirección solo se conserva cuando la atención es a domicilio.
        paciente.Direccion = dto.TipoAtencion == TiposAtencion.Domicilio &&
                             !string.IsNullOrWhiteSpace(dto.Direccion)
            ? dto.Direccion.Trim()
            : null;
    }

    /// <summary>Convierte un Paciente en su DTO de respuesta.</summary>
    public static PacienteDTO ADto(this Paciente paciente)
    {
        return new PacienteDTO
        {
            IdPaciente = paciente.IdPaciente,
            Nombre = paciente.Nombre,
            Edad = paciente.Edad,
            Sexo = paciente.Sexo,
            Telefono = paciente.Telefono,
            Patologia = paciente.Patologia,
            TipoAtencion = paciente.TipoAtencion,
            Direccion = paciente.Direccion
        };
    }

    /// <summary>Convierte un Paciente y sus turnos en el DTO de detalle.</summary>
    public static PacienteDetalleDTO ADtoDetalle(this Paciente paciente)
    {
        return new PacienteDetalleDTO
        {
            IdPaciente = paciente.IdPaciente,
            Nombre = paciente.Nombre,
            Edad = paciente.Edad,
            Sexo = paciente.Sexo,
            Telefono = paciente.Telefono,
            Patologia = paciente.Patologia,
            TipoAtencion = paciente.TipoAtencion,
            Direccion = paciente.Direccion,
            Turnos = paciente.Turnos
                .OrderByDescending(t => t.Fecha)
                .ThenByDescending(t => t.Hora)
                .Select(t => t.ADtoListado(paciente))
                .ToList()
        };
    }

    /// <summary>Convierte un turno en su DTO para la tabla del frontend.</summary>
    public static TurnoListadoDTO ADtoListado(this Turno turno)
        => turno.ADtoListado(turno.Paciente);

    /// <summary>Convierte un turno usando los datos del paciente indicado.</summary>
    public static TurnoListadoDTO ADtoListado(this Turno turno, Paciente paciente)
    {
        return new TurnoListadoDTO
        {
            IdTurno = turno.IdTurno,
            IdPaciente = paciente.IdPaciente,
            Nombre = paciente.Nombre,
            Telefono = paciente.Telefono,
            Patologia = paciente.Patologia,
            Fecha = turno.Fecha,
            Hora = turno.Hora,
            TipoAtencion = paciente.TipoAtencion,
            Direccion = paciente.Direccion,
            Estado = turno.Estado
        };
    }

    /// <summary>Convierte un turno en su DTO de detalle con el paciente incluido.</summary>
    public static TurnoDetalleDTO ADtoDetalle(this Turno turno)
    {
        return new TurnoDetalleDTO
        {
            IdTurno = turno.IdTurno,
            Fecha = turno.Fecha,
            Hora = turno.Hora,
            Estado = turno.Estado,
            Paciente = turno.Paciente.ADto()
        };
    }
}
