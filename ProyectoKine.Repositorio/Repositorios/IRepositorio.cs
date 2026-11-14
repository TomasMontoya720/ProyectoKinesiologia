namespace ProyectoKine.Repositorio.Repositorios;

/// <summary>
/// Contrato genérico de un repositorio. Define las operaciones básicas de
/// persistencia que comparten todas las entidades.
/// </summary>
/// <typeparam name="E">Entidad administrada por el repositorio.</typeparam>
public interface IRepositorio<E> where E : class
{
    /// <summary>Devuelve todas las entidades.</summary>
    Task<List<E>> Select();

    /// <summary>Busca una entidad por su clave primaria.</summary>
    Task<E?> SelectById(int id);

    /// <summary>Inserta una entidad y guarda los cambios.</summary>
    Task<E> Insert(E entity);

    /// <summary>Actualiza una entidad existente.</summary>
    Task<bool> Update(E entity);

    /// <summary>Elimina una entidad por su clave primaria.</summary>
    Task<bool> Delete(int id);

    /// <summary>Indica si existe una entidad con la clave indicada.</summary>
    Task<bool> Existe(int id);
}
