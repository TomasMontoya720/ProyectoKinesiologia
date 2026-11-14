using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProyectoKine.BD.Contexto;
using ProyectoKine.BD.Entidades;
using ProyectoKine.Shared.Constantes;

namespace ProyectoKine.Repositorio.Repositorios;

/// <summary>
/// Operaciones específicas de persistencia para los turnos.
/// </summary>
public interface ITurnoRepositorio : IRepositorio<Turno>
{
    /// <summary>Lista turnos con los datos del paciente, aplicando filtros opcionales.</summary>
    Task<List<Turno>> Listar(DateOnly? fecha, string? estado, string? buscar);

    /// <summary>Obtiene un turno incluyendo los datos del paciente.</summary>
    Task<Turno?> ObtenerDetalle(int id);

    /// <summary>Obtiene un turno con seguimiento de cambios para poder editarlo.</summary>
    Task<Turno?> ObtenerParaEditar(int id);

    /// <summary>Indica si ya existe un turno activo en la misma fecha y hora.</summary>
    Task<bool> ExisteActivoEnHorario(DateOnly fecha, TimeOnly hora, int? excluirIdTurno = null);
}

/// <summary>
/// Implementación de <see cref="ITurnoRepositorio"/> con LINQ y EF Core.
/// </summary>
public class TurnoRepositorio : Repositorio<Turno>, ITurnoRepositorio
{
    public TurnoRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    protected override Expression<Func<Turno, bool>> PredicadoPorId(int id)
        => turno => turno.IdTurno == id;

    protected override int ObtenerId(Turno entidad) => entidad.IdTurno;

    public async Task<List<Turno>> Listar(DateOnly? fecha, string? estado, string? buscar)
    {
        var consulta = Contexto.Turnos
            .Include(t => t.Paciente)
            .AsNoTracking()
            .AsQueryable();

        if (fecha.HasValue)
        {
            consulta = consulta.Where(t => t.Fecha == fecha.Value);
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            consulta = consulta.Where(t => t.Estado == estado);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var filtro = buscar.Trim();
            consulta = consulta.Where(t =>
                t.Paciente.Nombre.Contains(filtro) ||
                t.Paciente.Telefono.Contains(filtro) ||
                t.Paciente.Patologia.Contains(filtro));
        }

        return await consulta
            .OrderBy(t => t.Fecha)
            .ThenBy(t => t.Hora)
            .ToListAsync();
    }

    public async Task<Turno?> ObtenerDetalle(int id)
    {
        return await Contexto.Turnos
            .Include(t => t.Paciente)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdTurno == id);
    }

    public async Task<Turno?> ObtenerParaEditar(int id)
    {
        // Sin AsNoTracking para que EF Core detecte los cambios al guardar.
        return await Contexto.Turnos
            .Include(t => t.Paciente)
            .FirstOrDefaultAsync(t => t.IdTurno == id);
    }

    public async Task<bool> ExisteActivoEnHorario(DateOnly fecha, TimeOnly hora, int? excluirIdTurno = null)
    {
        return await Contexto.Turnos.AnyAsync(t =>
            t.Fecha == fecha &&
            t.Hora == hora &&
            t.Estado != EstadosTurno.Cancelado &&
            (!excluirIdTurno.HasValue || t.IdTurno != excluirIdTurno.Value));
    }
}
