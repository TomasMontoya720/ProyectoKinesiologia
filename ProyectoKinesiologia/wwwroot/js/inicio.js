import {
  apiFetch,
  escaparHtml,
  etiquetaEstado,
  etiquetaModalidad,
  formatearHora,
  mostrarToast
} from './api.js';

document.addEventListener('DOMContentLoaded', iniciar);

function iniciar() {
  const fechaHero = new Intl.DateTimeFormat('es-AR', {
    weekday: 'long',
    day: 'numeric',
    month: 'long'
  }).format(new Date());
  document.getElementById('fecha-hero').textContent = fechaHero;

  cargarResumen();
}

async function cargarResumen() {
  const tbody = document.getElementById('tabla-hoy');

  try {
    const turnos = await apiFetch('/turnos/hoy');
    const activos = turnos.filter((t) => t.estado !== 'Cancelado');

    document.getElementById('stat-hoy').textContent = activos.length;

    // Turnos activos cuya hora todavía no pasó.
    const ahora = new Date();
    const pendientes = activos
      .map((t) => ({ ...t, inicio: new Date(`${t.fecha}T${t.hora}`) }))
      .filter((t) => t.inicio >= ahora)
      .sort((a, b) => a.inicio - b.inicio);

    document.getElementById('stat-pendientes').textContent = pendientes.length;

    const proximo = pendientes[0];
    document.getElementById('stat-proximo').textContent = proximo
      ? `${formatearHora(proximo.hora)} hs`
      : 'Sin pendientes';
    document.getElementById('stat-proximo-detalle').textContent = proximo
      ? `${proximo.nombre} · ${proximo.patologia}`
      : 'No quedan turnos pendientes para hoy';

    if (turnos.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="4">
            <div class="empty-state">
              <div class="empty-state-icon">🗓️</div>
              <div class="fw-semibold">No hay turnos registrados para hoy.</div>
              <div>Podés agregar uno desde el botón “Agregar turno”.</div>
            </div>
          </td>
        </tr>`;
      return;
    }

    tbody.innerHTML = turnos
      .slice(0, 6)
      .map(
        (t) => `
          <tr>
            <td>
              <div class="cell-main">${escaparHtml(t.nombre)}</div>
              <div class="cell-sub">${escaparHtml(t.telefono)}</div>
            </td>
            <td class="fw-semibold text-nowrap">${formatearHora(t.hora)}</td>
            <td>${etiquetaModalidad(t.tipoAtencion, t.direccion)}</td>
            <td>${etiquetaEstado(t.estado)}</td>
          </tr>`
      )
      .join('');
  } catch (error) {
    tbody.innerHTML = `
      <tr>
        <td colspan="4">
          <div class="empty-state text-danger">${escaparHtml(error.message)}</div>
        </td>
      </tr>`;
    mostrarToast(error.message, 'danger');
  }
}
