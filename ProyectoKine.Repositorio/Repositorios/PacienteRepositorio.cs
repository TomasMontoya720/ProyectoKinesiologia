using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProyectoKine.BD.Contexto;
using ProyectoKine.BD.Entidades;

namespace ProyectoKine.Repositorio.Repositorios;

/// <summary>
/// Operaciones específicas de persistencia para los pacientes.
/// </summary>
public interface IPacienteRepositorio : IRepositorio<Paciente>
{
    /// <summary>Busca un paciente por su teléfono exacto (para reutilizar registros).</summary>
    Task<Paciente?> BuscarPorTelefono(string telefono);

    /// <summary>Busca pacientes por nombre, teléfono o patología.</summary>
    Task<List<Paciente>> Buscar(string? texto);

    /// <summary>Obtiene el paciente con su historial de turnos.</summary>
    Task<Paciente?> ObtenerConTurnos(int id);
}

/// <summary>
/// Implementación de <see cref="IPacienteRepositorio"/> con LINQ y EF Core.
/// </summary>
public class PacienteRepositorio : Repositorio<Paciente>, IPacienteRepositorio
{
    public PacienteRepositorio(AppDbContext contexto) : base(contexto)
    {
    }

    protected override Expression<Func<Paciente, bool>> PredicadoPorId(int id)
        => paciente => paciente.IdPaciente == id;

    protected override int ObtenerId(Paciente entidad) => entidad.IdPaciente;

    public async Task<Paciente?> BuscarPorTelefono(string telefono)
    {
        var numero = telefono.Trim();
        return await Contexto.Pacientes.FirstOrDefaultAsync(p => p.Telefono == numero);
    }

    public async Task<List<Paciente>> Buscar(string? texto)
    {
        var consulta = Contexto.Pacientes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var filtro = texto.Trim();
            consulta = consulta.Where(p =>
                p.Nombre.Contains(filtro) ||
                p.Telefono.Contains(filtro) ||
                p.Patologia.Contains(filtro));
        }

        return await consulta.OrderBy(p => p.Nombre).ToListAsync();
    }

    public async Task<Paciente?> ObtenerConTurnos(int id)
    {
        return await Contexto.Pacientes
            .Include(p => p.Turnos)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPaciente == id);
    }
}
