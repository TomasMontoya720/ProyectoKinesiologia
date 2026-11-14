using Microsoft.AspNetCore.Mvc;
using ProyectoKine.Servicio.Servicios;
using ProyectoKine.Shared.DTOs;

namespace ProyectoKinesiologia.Controllers;

/// <summary>
/// Operaciones sobre los turnos de los pacientes.
/// </summary>
[ApiController]
[Route("api/turnos")]
[Produces("application/json")]
public class TurnosController : ControllerBase
{
    private readonly ITurnoServicio _turnoServicio;

    public TurnosController(ITurnoServicio turnoServicio)
    {
        _turnoServicio = turnoServicio;
    }

    /// <summary>
    /// Lista los turnos. Permite filtrar por fecha, estado y texto de búsqueda.
    /// </summary>
    /// <param name="fecha">Fecha opcional en formato AAAA-MM-DD.</param>
    /// <param name="estado">Estado opcional: Pendiente o Cancelado.</param>
    /// <param name="buscar">Nombre, teléfono o patología del paciente.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<TurnoListadoDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TurnoListadoDTO>>> Listar(
        [FromQuery] DateOnly? fecha,
        [FromQuery] string? estado,
        [FromQuery] string? buscar)
    {
        var turnos = await _turnoServicio.ListarAsync(fecha, estado, buscar);
        return Ok(turnos);
    }

    /// <summary>
    /// Devuelve los turnos del día de hoy.
    /// </summary>
    [HttpGet("hoy")]
    [ProducesResponseType(typeof(List<TurnoListadoDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TurnoListadoDTO>>> ListarHoy()
    {
        var turnos = await _turnoServicio.ListarHoyAsync();
        return Ok(turnos);
    }

    /// <summary>
    /// Devuelve los turnos de una fecha determinada.
    /// </summary>
    /// <param name="fecha">Fecha en formato AAAA-MM-DD, por ejemplo 2026-11-14.</param>
    [HttpGet("fecha/{fecha}")]
    [ProducesResponseType(typeof(List<TurnoListadoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<TurnoListadoDTO>>> ListarPorFecha(DateTime fecha)
    {
        var turnos = await _turnoServicio.ListarAsync(DateOnly.FromDateTime(fecha), null, null);
        return Ok(turnos);
    }

    /// <summary>
    /// Obtiene el detalle de un turno con los datos del paciente.
    /// </summary>
    /// <param name="id">Id del turno.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TurnoDetalleDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TurnoDetalleDTO>> Obtener(int id)
    {
        var turno = await _turnoServicio.ObtenerAsync(id);
        return Ok(turno);
    }

    /// <summary>
    /// Registra un turno nuevo. Si el paciente no existe, lo crea; si existe
    /// (mismo teléfono o IdPaciente), reutiliza y actualiza sus datos.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TurnoDetalleDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TurnoDetalleDTO>> Crear([FromBody] TurnoFormDTO dto)
    {
        var turno = await _turnoServicio.CrearAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = turno.IdTurno }, turno);
    }

    /// <summary>
    /// Modifica un turno y los datos del paciente asociado.
    /// </summary>
    /// <param name="id">Id del turno.</param>
    /// <param name="dto">Datos actualizados del turno y del paciente.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TurnoDetalleDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TurnoDetalleDTO>> Actualizar(int id, [FromBody] TurnoFormDTO dto)
    {
        var turno = await _turnoServicio.ActualizarAsync(id, dto);
        return Ok(turno);
    }

    /// <summary>
    /// Cancela un turno. La cancelación es lógica: el registro se conserva
    /// con estado Cancelado para mantener el historial.
    /// </summary>
    /// <param name="id">Id del turno.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(int id)
    {
        await _turnoServicio.CancelarAsync(id);
        return NoContent();
    }
}
