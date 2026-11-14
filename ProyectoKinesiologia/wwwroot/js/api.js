// ---------------------------------------------------------------------------
// Utilidades comunes del frontend: llamadas a la API, formato y alertas.
// ---------------------------------------------------------------------------

export const API = '/api';

/**
 * Realiza una llamada fetch a la API y devuelve el JSON parseado.
 * Lanza un Error con el mensaje del ProblemDetails cuando la respuesta falla.
 */
export async function apiFetch(ruta, opciones = {}) {
  const { body, headers, ...resto } = opciones;
  const configuracion = {
    ...resto,
    headers: {
      ...(body ? { 'Content-Type': 'application/json' } : {}),
      ...(headers || {})
    }
  };

  if (body !== undefined && body !== null) {
    configuracion.body = typeof body === 'string' ? body : JSON.stringify(body);
  }

  let respuesta;
  try {
    respuesta = await fetch(`${API}${ruta}`, configuracion);
  } catch {
    throw new Error(armarMensajeSinConexion());
  }

  if (respuesta.status === 204) {
    return null;
  }

  const texto = await respuesta.text();
  let datos = null;
  if (texto) {
    try {
      datos = JSON.parse(texto);
    } catch {
      datos = texto;
    }
  }

  if (!respuesta.ok) {
    const error = new Error(armarMensajeError(datos, respuesta.status));
    error.status = respuesta.status;
    error.datos = datos;
    throw error;
  }

  return datos;
}

/**
 * Arma un mensaje útil cuando el navegador no pudo alcanzar la API.
 * El frontend se sirve desde la misma aplicación que la API, por lo que la
 * página debe abrirse con el mismo origen (no como archivo ni desde otro puerto).
 */
function armarMensajeSinConexion() {
  if (location.protocol === 'file:') {
    return 'La página se abrió directamente como archivo. Iniciá la API y entrá desde ' +
      'http://localhost:5132 (no abras los .html con doble clic).';
  }

  return `No se pudo conectar con la API en ${location.origin}. ` +
    'Verificá que el servidor esté en ejecución y que la página se haya abierto ' +
    'desde el mismo puerto de la API (por ejemplo http://localhost:5132).';
}

/** Extrae un mensaje legible de una respuesta ProblemDetails. */
export function armarMensajeError(datos, estado) {
  if (datos && datos.errors) {
    return Object.values(datos.errors).flat().join(' ');
  }
  return (datos && (datos.detail || datos.title)) || `Ocurrió un error (HTTP ${estado}).`;
}

/** Escapa texto para poder insertarlo con innerHTML sin riesgos. */
export function escaparHtml(valor) {
  return String(valor ?? '')
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#39;');
}

/** Devuelve la fecha local en formato AAAA-MM-DD. */
export function fechaISOLocal(fecha = new Date()) {
  const ajustada = new Date(fecha.getTime() - fecha.getTimezoneOffset() * 60000);
  return ajustada.toISOString().slice(0, 10);
}

/** Convierte 2026-11-14 en 14/11/2026. */
export function formatearFecha(fecha) {
  if (!fecha) return '';
  const [anio, mes, dia] = String(fecha).slice(0, 10).split('-');
  return `${dia}/${mes}/${anio}`;
}

/** Convierte 09:30:00 en 09:30. */
export function formatearHora(hora) {
  return String(hora ?? '').slice(0, 5);
}

/** Devuelve el HTML de un badge según el estado del turno. */
export function etiquetaEstado(estado) {
  return estado === 'Cancelado'
    ? '<span class="badge text-bg-secondary">Cancelado</span>'
    : '<span class="badge text-bg-success">Pendiente</span>';
}

/** Devuelve el HTML de un badge según la modalidad de atención. */
export function etiquetaModalidad(tipo, direccion) {
  if (tipo === 'Domicilio') {
    const titulo = direccion ? ` title="${escaparHtml(direccion)}"` : '';
    return `<span class="badge text-bg-info"${titulo}>Domicilio</span>`;
  }
  return '<span class="badge text-bg-light border text-dark">Consultorio</span>';
}

/** Muestra una notificación flotante (Bootstrap toast). */
export function mostrarToast(mensaje, tipo = 'success') {
  let contenedor = document.getElementById('contenedor-toasts');
  if (!contenedor) {
    contenedor = document.createElement('div');
    contenedor.id = 'contenedor-toasts';
    contenedor.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    contenedor.style.zIndex = '1090';
    document.body.appendChild(contenedor);
  }

  const clases = {
    success: 'text-bg-success',
    danger: 'text-bg-danger',
    warning: 'text-bg-warning',
    info: 'text-bg-info'
  };
  const clase = clases[tipo] || clases.success;

  const elemento = document.createElement('div');
  elemento.className = `toast align-items-center border-0 ${clase}`;
  elemento.setAttribute('role', 'alert');
  elemento.setAttribute('aria-live', 'assertive');
  elemento.setAttribute('aria-atomic', 'true');
  elemento.innerHTML = `
    <div class="d-flex">
      <div class="toast-body">${escaparHtml(mensaje)}</div>
      <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Cerrar"></button>
    </div>`;

  contenedor.appendChild(elemento);
  const toast = new bootstrap.Toast(elemento, { delay: 4500 });
  toast.show();
  elemento.addEventListener('hidden.bs.toast', () => elemento.remove());
}

/** Guarda un mensaje para mostrarlo después de una redirección. */
export function guardarMensaje(mensaje, tipo = 'success') {
  sessionStorage.setItem('kine-mensaje', JSON.stringify({ mensaje, tipo }));
}

/** Muestra (y borra) el mensaje guardado antes de una redirección. */
export function mostrarMensajeGuardado() {
  const guardado = sessionStorage.getItem('kine-mensaje');
  if (!guardado) return;

  sessionStorage.removeItem('kine-mensaje');
  try {
    const { mensaje, tipo } = JSON.parse(guardado);
    mostrarToast(mensaje, tipo);
  } catch {
    // Si el contenido guardado no es válido, se ignora.
  }
}
