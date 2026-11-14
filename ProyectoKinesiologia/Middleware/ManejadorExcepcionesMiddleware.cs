using Microsoft.AspNetCore.Mvc;
using ProyectoKine.Servicio.Excepciones;

namespace ProyectoKinesiologia.Middleware;

/// <summary>
/// Middleware global que transforma las excepciones conocidas en respuestas
/// HTTP con formato ProblemDetails (404, 409) y registra los errores inesperados.
/// </summary>
public class ManejadorExcepcionesMiddleware
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ManejadorExcepcionesMiddleware> _logger;

    public ManejadorExcepcionesMiddleware(
        RequestDelegate siguiente,
        ILogger<ManejadorExcepcionesMiddleware> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (KeyNotFoundException excepcion)
        {
            await EscribirRespuestaAsync(contexto, StatusCodes.Status404NotFound,
                "Recurso no encontrado", excepcion.Message);
        }
        catch (ReglaNegocioException excepcion)
        {
            await EscribirRespuestaAsync(contexto, StatusCodes.Status409Conflict,
                "Conflicto con una regla de negocio", excepcion.Message);
        }
        catch (Exception excepcion)
        {
            _logger.LogError(excepcion, "Error no controlado al procesar {Metodo} {Ruta}",
                contexto.Request.Method, contexto.Request.Path);

            await EscribirRespuestaAsync(contexto, StatusCodes.Status500InternalServerError,
                "Error interno del servidor",
                "Ocurrió un error inesperado. Intentá nuevamente.");
        }
    }

    private static async Task EscribirRespuestaAsync(
        HttpContext contexto,
        int estado,
        string titulo,
        string detalle)
    {
        if (contexto.Response.HasStarted)
        {
            return;
        }

        contexto.Response.Clear();
        contexto.Response.StatusCode = estado;

        var problema = new ProblemDetails
        {
            Status = estado,
            Title = titulo,
            Detail = detalle,
            Instance = contexto.Request.Path
        };

        await contexto.Response.WriteAsJsonAsync(problema);
    }
}
