import {
  apiFetch,
  escaparHtml,
  etiquetaEstado,
  etiquetaModalidad,
  formatearFecha,
  formatearHora,
  mostrarToast
} from './api.js';

const parametros = new URLSearchParams(location.search);
const idPaciente = Number(parametros.get('id'));

document.addEventListener('DOMContentLoaded', iniciar);

function iniciar() {
  document.getElementById('btn-imprimir').addEventListener('click', () => window.print());

  if (!idPaciente || Number.isNaN(idPaciente)) {
    mostrarError('No se indicó un paciente válido. Volvé a la agenda y elegí “Ver”.');
    return;
  }

  cargarPaciente(idPaciente);
}

async function cargarPaciente(id) {
  try {
    const paciente = await apiFetch(`/pacientes/${id}`);
    renderizarPaciente(paciente);
  } catch (error) {
    mostrarError(error.message);
    mostrarToast(error.message, 'danger');
  }
}

function renderizarPaciente(paciente) {
  document.getElementById('cargando-paciente').classList.add('d-none');
  document.getElementById('contenido-paciente').classList.remove('d-none');
  document.title = `KineTurnos · ${paciente.nombre}`;

  document.getElementById('paciente-nombre').textContent = paciente.nombre;
  document.getElementById('paciente-resumen').textContent =
    `${paciente.edad} años · ${paciente.sexo}`;
  document.getElementById('paciente-edad').textContent = `${paciente.edad} años`;
  document.getElementById('paciente-sexo').textContent = paciente.sexo;
  document.getElementById('paciente-patologia').textContent = paciente.patologia;
  document.getElementById('paciente-modalidad').innerHTML =
    etiquetaModalidad(paciente.tipoAtencion, paciente.direccion);
  document.getElementById('subtitulo-paciente').textContent =
    `Ficha de ${paciente.nombre} e historial de turnos.`;

  const telefono = document.getElementById('paciente-telefono');
  telefono.innerHTML = '';
  const enlaceTelefono = document.createElement('a');
  enlaceTelefono.href = `tel:${paciente.telefono}`;
  enlaceTelefono.textContent = paciente.telefono;
  telefono.appendChild(enlaceTelefono);

  const filaDireccion = document.getElementById('fila-direccion');
  if (paciente.tipoAtencion === 'Domicilio' && paciente.direccion) {
    filaDireccion.classList.remove('d-none');
    document.getElementById('paciente-direccion').textContent = paciente.direccion;
  } else {
    filaDireccion.classList.add('d-none');
  }

  renderizarHistorial(paciente.turnos);
}

function renderizarHistorial(turnos) {
  const tbody = document.getElementById('tabla-historial');
  const resumen = document.getElementById('resumen-historial');
  const total = turnos.length;

  resumen.textContent = total === 1 ? '1 turno registrado' : `${total} turnos registrados`;

  if (total === 0) {
    tbody.innerHTML = `
      <tr>
        <td colspan="5">
          <div class="empty-state">
            <div class="empty-state-icon">🗓️</div>
            <div class="fw-semibold">Este paciente todavía no tiene turnos.</div>
            <div>Podés registrar uno desde “Nuevo turno”.</div>
          </div>
        </td>
      </tr>`;
    return;
  }

  tbody.innerHTML = turnos.map((turno) => `
    <tr>
      <td class="text-nowrap">${formatearFecha(turno.fecha)}</td>
      <td class="fw-semibold text-nowrap">${formatearHora(turno.hora)}</td>
      <td>${etiquetaModalidad(turno.tipoAtencion, turno.direccion)}</td>
      <td>${etiquetaEstado(turno.estado)}</td>
      <td class="text-end text-nowrap no-print">
        <a class="btn btn-sm btn-outline-kine" href="turno-form.html?id=${turno.idTurno}">Editar</a>
      </td>
    </tr>`).join('');
}

function mostrarError(mensaje) {
  document.getElementById('cargando-paciente').classList.add('d-none');

  const alerta = document.getElementById('error-paciente');
  alerta.textContent = mensaje;
  alerta.classList.remove('d-none');
}
