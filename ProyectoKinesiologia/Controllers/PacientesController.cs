using Microsoft.AspNetCore.Mvc;
using ProyectoKine.Servicio.Servicios;
using ProyectoKine.Shared.DTOs;

namespace ProyectoKinesiologia.Controllers;

/// <summary>
/// Operaciones sobre los pacientes de la kinesióloga.
/// </summary>
[ApiController]
[Route("api/pacientes")]
[Produces("application/json")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteServicio _pacienteServicio;

    public PacientesController(IPacienteServicio pacienteServicio)
    {
        _pacienteServicio = pacienteServicio;
    }

    /// <summary>
    /// Lista todos los pacientes. Permite buscar por nombre, teléfono o patología.
    /// </summary>
    /// <param name="buscar">Texto opcional para filtrar.</param>
    [HttpGet]
    [ProducesResponseType(typeof(List<PacienteDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PacienteDTO>>> Listar([FromQuery] string? buscar)
    {
        var pacientes = await _pacienteServicio.ListarAsync(buscar);
        return Ok(pacientes);
    }

    /// <summary>
    /// Obtiene los datos de un paciente junto con su historial de turnos.
    /// </summary>
    /// <param name="id">Id del paciente.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PacienteDetalleDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PacienteDetalleDTO>> Obtener(int id)
    {
        var paciente = await _pacienteServicio.ObtenerAsync(id);
        return Ok(paciente);
    }

    /// <summary>
    /// Lista los turnos de un paciente.
    /// </summary>
    /// <param name="id">Id del paciente.</param>
    [HttpGet("{id:int}/turnos")]
    [ProducesResponseType(typeof(List<TurnoListadoDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<TurnoListadoDTO>>> ListarTurnos(int id)
    {
        var paciente = await _pacienteServicio.ObtenerAsync(id);
        return Ok(paciente.Turnos);
    }

    /// <summary>
    /// Registra un paciente nuevo.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PacienteDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PacienteDTO>> Crear([FromBody] PacienteFormDTO dto)
    {
        var paciente = await _pacienteServicio.CrearAsync(dto);
        return CreatedAtAction(nameof(Obtener), new { id = paciente.IdPaciente }, paciente);
    }

    /// <summary>
    /// Modifica los datos de un paciente existente.
    /// </summary>
    /// <param name="id">Id del paciente.</param>
    /// <param name="dto">Datos actualizados del paciente.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PacienteDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PacienteDTO>> Actualizar(int id, [FromBody] PacienteFormDTO dto)
    {
        var paciente = await _pacienteServicio.ActualizarAsync(id, dto);
        return Ok(paciente);
    }

    /// <summary>
    /// Elimina un paciente. No se permite si tiene turnos asociados.
    /// </summary>
    /// <param name="id">Id del paciente.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _pacienteServicio.EliminarAsync(id);
        return NoContent();
    }
}
