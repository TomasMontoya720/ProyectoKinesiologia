using ProyectoKine.BD.Entidades;
using ProyectoKine.Repositorio.Repositorios;
using ProyectoKine.Servicio.Excepciones;
using ProyectoKine.Servicio.Mapeo;
using ProyectoKine.Shared.Constantes;
using ProyectoKine.Shared.DTOs;

namespace ProyectoKine.Servicio.Servicios;

/// <summary>
/// Reglas de negocio relacionadas con los pacientes.
/// </summary>
public interface IPacienteServicio
{
    Task<List<PacienteDTO>> ListarAsync(string? buscar);
    Task<PacienteDetalleDTO> ObtenerAsync(int id);
    Task<PacienteDTO> CrearAsync(PacienteFormDTO dto);
    Task<PacienteDTO> ActualizarAsync(int id, PacienteFormDTO dto);
    Task EliminarAsync(int id);
}

/// <summary>
/// Implementación de <see cref="IPacienteServicio"/>.
/// </summary>
public class PacienteServicio : IPacienteServicio
{
    private readonly IPacienteRepositorio _pacienteRepositorio;

    public PacienteServicio(IPacienteRepositorio pacienteRepositorio)
    {
        _pacienteRepositorio = pacienteRepositorio;
    }

    public async Task<List<PacienteDTO>> ListarAsync(string? buscar)
    {
        var pacientes = await _pacienteRepositorio.Buscar(buscar);
        return pacientes.Select(p => p.ADto()).ToList();
    }

    public async Task<PacienteDetalleDTO> ObtenerAsync(int id)
    {
        var paciente = await _pacienteRepositorio.ObtenerConTurnos(id)
            ?? throw new KeyNotFoundException($"No se encontró el paciente con id {id}.");

        return paciente.ADtoDetalle();
    }

    public async Task<PacienteDTO> CrearAsync(PacienteFormDTO dto)
    {
        var paciente = new Paciente();
        paciente.AplicarDatos(dto);

        await _pacienteRepositorio.Insert(paciente);
        return paciente.ADto();
    }

    public async Task<PacienteDTO> ActualizarAsync(int id, PacienteFormDTO dto)
    {
        var paciente = await _pacienteRepositorio.SelectById(id)
            ?? throw new KeyNotFoundException($"No se encontró el paciente con id {id}.");

        paciente.AplicarDatos(dto);
        await _pacienteRepositorio.Update(paciente);
        return paciente.ADto();
    }

    public async Task EliminarAsync(int id)
    {
        var paciente = await _pacienteRepositorio.ObtenerConTurnos(id)
            ?? throw new KeyNotFoundException($"No se encontró el paciente con id {id}.");

        // No se permite eliminar un paciente que tenga turnos registrados:
        // se perdería el historial. Primero deben cancelarse/eliminarse sus turnos.
        if (paciente.Turnos.Count > 0)
        {
            throw new ReglaNegocioException(
                "No se puede eliminar el paciente porque tiene turnos asociados. " +
                "Conserve el historial o elimine primero sus turnos.");
        }

        await _pacienteRepositorio.Delete(id);
    }
}
