using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProyectoKine.BD.Contexto;

namespace ProyectoKine.Repositorio.Repositorios;

/// <summary>
/// Implementación genérica de <see cref="IRepositorio{E}"/> sobre Entity Framework Core.
/// Las clases derivadas solo indican cuál es la clave primaria y su valor.
/// </summary>
public abstract class Repositorio<E> : IRepositorio<E> where E : class
{
    protected readonly AppDbContext Contexto;

    protected Repositorio(AppDbContext contexto)
    {
        Contexto = contexto;
    }

    /// <summary>Expresión LINQ que compara la clave primaria de la entidad con un id.</summary>
    protected abstract Expression<Func<E, bool>> PredicadoPorId(int id);

    /// <summary>Obtiene el valor de la clave primaria de una entidad.</summary>
    protected abstract int ObtenerId(E entidad);

    public virtual async Task<List<E>> Select()
    {
        // AsNoTracking se usa en consultas de solo lectura para mejorar el rendimiento.
        return await Contexto.Set<E>().AsNoTracking().ToListAsync();
    }

    public virtual async Task<E?> SelectById(int id)
    {
        return await Contexto.Set<E>().FirstOrDefaultAsync(PredicadoPorId(id));
    }

    public virtual async Task<E> Insert(E entity)
    {
        await Contexto.Set<E>().AddAsync(entity);
        await Contexto.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> Update(E entity)
    {
        if (!await Existe(ObtenerId(entity)))
        {
            return false;
        }

        Contexto.Set<E>().Update(entity);
        await Contexto.SaveChangesAsync();
        return true;
    }

    public virtual async Task<bool> Delete(int id)
    {
        var entidad = await SelectById(id);
        if (entidad is null)
        {
            return false;
        }

        Contexto.Set<E>().Remove(entidad);
        await Contexto.SaveChangesAsync();
        return true;
    }

    public virtual Task<bool> Existe(int id)
    {
        return Contexto.Set<E>().AnyAsync(PredicadoPorId(id));
    }
}
