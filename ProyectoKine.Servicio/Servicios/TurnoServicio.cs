using Microsoft.EntityFrameworkCore;
using ProyectoKine.BD.Contexto;
using ProyectoKine.BD.Entidades;
using ProyectoKine.Repositorio.Repositorios;
using ProyectoKine.Servicio.Excepciones;
using ProyectoKine.Servicio.Mapeo;
using ProyectoKine.Shared.Constantes;
using ProyectoKine.Shared.DTOs;

namespace ProyectoKine.Servicio.Servicios;

/// <summary>
/// Reglas de negocio relacionadas con los turnos.
/// </summary>
public interface ITurnoServicio
{
    Task<List<TurnoListadoDTO>> ListarAsync(DateOnly? fecha, string? estado, string? buscar);
    Task<List<TurnoListadoDTO>> ListarHoyAsync();
    Task<TurnoDetalleDTO> ObtenerAsync(int id);
    Task<TurnoDetalleDTO> CrearAsync(TurnoFormDTO dto);
    Task<TurnoDetalleDTO> ActualizarAsync(int id, TurnoFormDTO dto);
    Task CancelarAsync(int id);
}

/// <summary>
/// Implementación de <see cref="ITurnoServicio"/>.
/// </summary>
public class TurnoServicio : ITurnoServicio
{
    private readonly ITurnoRepositorio _turnoRepositorio;
    private readonly IPacienteRepositorio _pacienteRepositorio;
    private readonly AppDbContext _contexto;

    public TurnoServicio(
        ITurnoRepositorio turnoRepositorio,
        IPacienteRepositorio pacienteRepositorio,
        AppDbContext contexto)
    {
        _turnoRepositorio = turnoRepositorio;
        _pacienteRepositorio = pacienteRepositorio;
        // El contexto se usa únicamente para coordinar transacciones entre
        // las operaciones de pacientes y turnos.
        _contexto = contexto;
    }

    public async Task<List<TurnoListadoDTO>> ListarAsync(DateOnly? fecha, string? estado, string? buscar)
    {
        var turnos = await _turnoRepositorio.Listar(fecha, estado, buscar);
        return turnos.Select(t => t.ADtoListado()).ToList();
    }

    public Task<List<TurnoListadoDTO>> ListarHoyAsync()
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        return ListarAsync(hoy, null, null);
    }

    public async Task<TurnoDetalleDTO> ObtenerAsync(int id)
    {
        var turno = await _turnoRepositorio.ObtenerDetalle(id)
            ?? throw new KeyNotFoundException($"No se encontró el turno con id {id}.");

        return turno.ADtoDetalle();
    }

    public async Task<TurnoDetalleDTO> CrearAsync(TurnoFormDTO dto)
    {
        var fecha = dto.Fecha!.Value;
        var hora = dto.Hora!.Value;

        if (fecha < DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ReglaNegocioException("No se puede registrar un turno en una fecha anterior a hoy.");
        }

        // Se usa una transacción para que el paciente y el turno se guarden juntos.
        await using var transaccion = await _contexto.Database.BeginTransactionAsync();

        if (await _turnoRepositorio.ExisteActivoEnHorario(fecha, hora))
        {
            throw new ReglaNegocioException(
                "Ya existe un turno activo para esa fecha y hora. Elegí otro horario.");
        }

        var paciente = await ObtenerOCrearPacienteAsync(dto);

        var turno = new Turno
        {
            Fecha = fecha,
            Hora = hora,
            Estado = EstadosTurno.Pendiente,
            IdPaciente = paciente.IdPaciente,
            Paciente = paciente
        };

        await _turnoRepositorio.Insert(turno);
        await transaccion.CommitAsync();

        return turno.ADtoDetalle();
    }

    public async Task<TurnoDetalleDTO> ActualizarAsync(int id, TurnoFormDTO dto)
    {
        var turno = await _turnoRepositorio.ObtenerParaEditar(id)
            ?? throw new KeyNotFoundException($"No se encontró el turno con id {id}.");

        var fecha = dto.Fecha!.Value;
        var hora = dto.Hora!.Value;

        // Un turno cancelado ya no ocupa un horario; los activos no pueden superponerse.
        if (turno.Estado != EstadosTurno.Cancelado &&
            await _turnoRepositorio.ExisteActivoEnHorario(fecha, hora, id))
        {
            throw new ReglaNegocioException(
                "Ya existe un turno activo para esa fecha y hora. Elegí otro horario.");
        }

        await using var transaccion = await _contexto.Database.BeginTransactionAsync();

        // El paciente puede ser el mismo del turno o uno indicado explícitamente.
        Paciente paciente;
        if (dto.IdPaciente.HasValue)
        {
            paciente = await _pacienteRepositorio.SelectById(dto.IdPaciente.Value)
                ?? throw new KeyNotFoundException($"No se encontró el paciente con id {dto.IdPaciente}.");
        }
        else
        {
            paciente = turno.Paciente;
        }

        paciente.AplicarDatos(dto);
        await _pacienteRepositorio.Update(paciente);

        turno.Fecha = fecha;
        turno.Hora = hora;
        turno.IdPaciente = paciente.IdPaciente;
        turno.Paciente = paciente;

        await _turnoRepositorio.Update(turno);
        await transaccion.CommitAsync();

        return turno.ADtoDetalle();
    }

    public async Task CancelarAsync(int id)
    {
        var turno = await _turnoRepositorio.ObtenerParaEditar(id)
            ?? throw new KeyNotFoundException($"No se encontró el turno con id {id}.");

        // La cancelación es lógica: el turno se conserva con estado Cancelado.
        if (turno.Estado == EstadosTurno.Cancelado)
        {
            return;
        }

        turno.Estado = EstadosTurno.Cancelado;
        await _turnoRepositorio.Update(turno);
    }

    /// <summary>
    /// Busca al paciente por id o por teléfono. Si no existe, lo crea.
    /// Si existe, actualiza sus datos con los del formulario.
    /// </summary>
    private async Task<Paciente> ObtenerOCrearPacienteAsync(TurnoFormDTO dto)
    {
        Paciente? paciente = null;

        if (dto.IdPaciente.HasValue)
        {
            paciente = await _pacienteRepositorio.SelectById(dto.IdPaciente.Value)
                ?? throw new KeyNotFoundException($"No se encontró el paciente con id {dto.IdPaciente}.");
        }
        else
        {
            paciente = await _pacienteRepositorio.BuscarPorTelefono(dto.Telefono);
        }

        if (paciente is null)
        {
            paciente = new Paciente();
            paciente.AplicarDatos(dto);
            return await _pacienteRepositorio.Insert(paciente);
        }

        paciente.AplicarDatos(dto);
        await _pacienteRepositorio.Update(paciente);
        return paciente;
    }
}
