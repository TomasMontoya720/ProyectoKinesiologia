import {
  apiFetch,
  escaparHtml,
  etiquetaEstado,
  etiquetaModalidad,
  fechaISOLocal,
  formatearFecha,
  formatearHora,
  mostrarMensajeGuardado,
  mostrarToast
} from './api.js';

const estado = {
  modo: 'todos',
  turnos: [],
  turnoACancelar: null
};

let modalCancelar;

document.addEventListener('DOMContentLoaded', iniciar);

function iniciar() {
  modalCancelar = new bootstrap.Modal(document.getElementById('modal-cancelar'));

  // Permite abrir la pantalla con ?modo=hoy o ?fecha=AAAA-MM-DD.
  const parametros = new URLSearchParams(location.search);
  const modoInicial = parametros.get('modo');
  const fechaInicial = parametros.get('fecha');

  if (modoInicial === 'hoy') {
    document.getElementById('modo-hoy').checked = true;
    estado.modo = 'hoy';
  } else if (fechaInicial) {
    document.getElementById('modo-fecha').checked = true;
    document.getElementById('filtro-fecha').value = fechaInicial;
    document.getElementById('filtro-fecha').disabled = false;
    estado.modo = 'fecha';
  }

  document.querySelectorAll('input[name="modo"]').forEach((radio) => {
    radio.addEventListener('change', cambiarModo);
  });

  document.getElementById('filtro-fecha').addEventListener('change', () => {
    if (estado.modo === 'fecha') {
      cargarTurnos();
    }
  });

  document.getElementById('filtro-estado').addEventListener('change', renderizar);
  document.getElementById('filtro-buscar').addEventListener('input', renderizar);
  document.getElementById('btn-actualizar').addEventListener('click', cargarTurnos);
  document.getElementById('btn-confirmar-cancelar').addEventListener('click', confirmarCancelacion);
  document.getElementById('tabla-turnos').addEventListener('click', manejarAcciones);

  cargarTurnos();
  mostrarMensajeGuardado();
}

function cambiarModo() {
  const seleccionado = document.querySelector('input[name="modo"]:checked');
  estado.modo = seleccionado ? seleccionado.value : 'todos';

  const campoFecha = document.getElementById('filtro-fecha');
  campoFecha.disabled = estado.modo !== 'fecha';

  if (estado.modo === 'fecha' && !campoFecha.value) {
    campoFecha.value = fechaISOLocal();
  }

  cargarTurnos();
}

async function cargarTurnos() {
  const tbody = document.getElementById('tabla-turnos');
  tbody.innerHTML = '<tr><td colspan="7" class="empty-state">Cargando turnos...</td></tr>';

  let ruta = '/turnos';

  if (estado.modo === 'hoy') {
    ruta = '/turnos/hoy';
  } else if (estado.modo === 'fecha') {
    const fecha = document.getElementById('filtro-fecha').value;
    if (!fecha) {
      estado.turnos = [];
      renderizar();
      return;
    }
    ruta = `/turnos/fecha/${fecha}`;
  }

  try {
    estado.turnos = await apiFetch(ruta);
    renderizar();
  } catch (error) {
    estado.turnos = [];
    tbody.innerHTML = `
      <tr>
        <td colspan="7">
          <div class="empty-state text-danger">${escaparHtml(error.message)}</div>
        </td>
      </tr>`;
    document.getElementById('resumen-turnos').textContent = 'No se pudieron cargar los turnos';
    mostrarToast(error.message, 'danger');
  }
}

function renderizar() {
  const tbody = document.getElementById('tabla-turnos');
  const filtroEstado = document.getElementById('filtro-estado').value;
  const texto = document.getElementById('filtro-buscar').value.trim().toLowerCase();

  const filtrados = estado.turnos.filter((turno) => {
    if (filtroEstado && turno.estado !== filtroEstado) {
      return false;
    }

    if (!texto) {
      return true;
    }

    return [turno.nombre, turno.telefono, turno.patologia]
      .some((valor) => String(valor ?? '').toLowerCase().includes(texto));
  });

  actualizarEncabezado(filtrados.length);

  if (filtrados.length === 0) {
    tbody.innerHTML = `
      <tr>
        <td colspan="7">
          <div class="empty-state">
            <div class="empty-state-icon">🗓️</div>
            <div class="fw-semibold">No hay turnos para mostrar.</div>
            <div>Probá con otro período, estado o término de búsqueda.</div>
          </div>
        </td>
      </tr>`;
    return;
  }

  tbody.innerHTML = filtrados.map((turno) => {
    const cancelado = turno.estado === 'Cancelado';
    const direccion = turno.tipoAtencion === 'Domicilio' && turno.direccion
      ? `<div class="cell-sub">${escaparHtml(turno.direccion)}</div>`
      : '';

    return `
      <tr>
        <td>
          <div class="cell-main">${escaparHtml(turno.nombre)}</div>
          <div class="cell-sub">${escaparHtml(turno.telefono)}</div>
        </td>
        <td>${escaparHtml(turno.patologia)}</td>
        <td class="text-nowrap">${formatearFecha(turno.fecha)}</td>
        <td class="fw-semibold text-nowrap">${formatearHora(turno.hora)}</td>
        <td>
          ${etiquetaModalidad(turno.tipoAtencion, turno.direccion)}
          ${direccion}
        </td>
        <td>${etiquetaEstado(turno.estado)}</td>
        <td class="text-end text-nowrap">
          <div class="btn-group btn-group-sm" role="group" aria-label="Acciones del turno">
            <a class="btn btn-outline-secondary" href="paciente.html?id=${turno.idPaciente}"
               title="Ver ficha del paciente">Ver</a>
            <a class="btn btn-outline-kine" href="turno-form.html?id=${turno.idTurno}"
               title="Editar turno">Editar</a>
            <button type="button" class="btn btn-outline-danger"
                    data-cancelar="${turno.idTurno}"
                    data-nombre="${escaparHtml(turno.nombre)}"
                    ${cancelado ? 'disabled' : ''}>
              Cancelar
            </button>
          </div>
        </td>
      </tr>`;
  }).join('');
}

function actualizarEncabezado(cantidad) {
  const titulo = document.getElementById('titulo-listado');
  const resumen = document.getElementById('resumen-turnos');
  const total = estado.turnos.length;
  const etiquetaCantidad = cantidad === 1 ? '1 turno' : `${cantidad} turnos`;

  if (estado.modo === 'hoy') {
    titulo.textContent = 'Turnos de hoy';
  } else if (estado.modo === 'fecha') {
    const fecha = document.getElementById('filtro-fecha').value;
    titulo.textContent = `Turnos del ${formatearFecha(fecha)}`;
  } else {
    titulo.textContent = 'Todos los turnos';
  }

  resumen.textContent = cantidad === total
    ? etiquetaCantidad
    : `${etiquetaCantidad} de ${total}`;
}

function manejarAcciones(evento) {
  const boton = evento.target.closest('[data-cancelar]');
  if (!boton) {
    return;
  }

  estado.turnoACancelar = Number(boton.dataset.cancelar);
  document.getElementById('cancelar-nombre').textContent = boton.dataset.nombre;
  modalCancelar.show();
}

async function confirmarCancelacion() {
  if (!estado.turnoACancelar) {
    return;
  }

  const boton = document.getElementById('btn-confirmar-cancelar');
  const id = estado.turnoACancelar;
  boton.disabled = true;
  boton.textContent = 'Cancelando...';

  try {
    await apiFetch(`/turnos/${id}`, { method: 'DELETE' });
    modalCancelar.hide();
    mostrarToast('Turno cancelado correctamente.');
    await cargarTurnos();
  } catch (error) {
    mostrarToast(error.message, 'danger');
  } finally {
    boton.disabled = false;
    boton.textContent = 'Sí, cancelar';
    estado.turnoACancelar = null;
  }
}
