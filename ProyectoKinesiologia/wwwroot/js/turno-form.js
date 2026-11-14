import {
  apiFetch,
  fechaISOLocal,
  guardarMensaje,
  mostrarToast
} from './api.js';

const parametros = new URLSearchParams(location.search);
const idTurno = parametros.get('id');

let formulario;
let botonGuardar;
let bloqueDireccion;
let campoDireccion;

document.addEventListener('DOMContentLoaded', iniciar);

function iniciar() {
  formulario = document.getElementById('form-turno');
  botonGuardar = document.getElementById('btn-guardar');
  bloqueDireccion = document.getElementById('bloque-direccion');
  campoDireccion = document.getElementById('direccion');

  document.getElementById('tipoAtencion').addEventListener('change', actualizarDireccion);
  formulario.addEventListener('submit', guardar);

  // En un alta no se permiten fechas anteriores a hoy.
  if (!idTurno) {
    document.getElementById('fecha').min = fechaISOLocal();
    return;
  }

  cargarTurno(idTurno);
}

async function cargarTurno(id) {
  try {
    const turno = await apiFetch(`/turnos/${id}`);
    const paciente = turno.paciente;

    document.getElementById('titulo-formulario').textContent = 'Editar turno';
    document.getElementById('subtitulo-formulario').textContent =
      'Modificá los datos del paciente o el horario del turno.';
    document.title = 'KineTurnos · Editar turno';
    botonGuardar.textContent = 'Guardar cambios';

    document.getElementById('idPaciente').value = paciente.idPaciente;
    document.getElementById('nombre').value = paciente.nombre;
    document.getElementById('edad').value = paciente.edad;
    document.getElementById('sexo').value = paciente.sexo;
    document.getElementById('telefono').value = paciente.telefono;
    document.getElementById('patologia').value = paciente.patologia;
    document.getElementById('tipoAtencion').value = paciente.tipoAtencion;
    document.getElementById('direccion').value = paciente.direccion || '';

    document.getElementById('fecha').value = String(turno.fecha).slice(0, 10);
    document.getElementById('hora').value = String(turno.hora).slice(0, 5);

    // Un turno existente puede editarse aunque la fecha ya haya pasado.
    document.getElementById('fecha').removeAttribute('min');

    actualizarDireccion();
  } catch (error) {
    formulario.querySelectorAll('input, select, button').forEach((elemento) => {
      elemento.disabled = true;
    });
    mostrarErrorGeneral(error.message);
    mostrarToast(error.message, 'danger');
  }
}

function actualizarDireccion() {
  const esDomicilio = document.getElementById('tipoAtencion').value === 'Domicilio';

  bloqueDireccion.classList.toggle('d-none', !esDomicilio);
  campoDireccion.required = esDomicilio;

  if (!esDomicilio) {
    campoDireccion.value = '';
    campoDireccion.classList.remove('is-invalid');
  }
}

async function guardar(evento) {
  evento.preventDefault();
  evento.stopPropagation();

  formulario.classList.add('was-validated');

  // Se valida también la dirección condicional antes de enviar.
  actualizarDireccion();
  if (!formulario.checkValidity()) {
    return;
  }

  limpiarErrores();

  const hora = document.getElementById('hora').value;
  const datos = {
    idPaciente: Number(document.getElementById('idPaciente').value) || null,
    nombre: document.getElementById('nombre').value.trim(),
    edad: Number(document.getElementById('edad').value),
    sexo: document.getElementById('sexo').value,
    telefono: document.getElementById('telefono').value.trim(),
    patologia: document.getElementById('patologia').value.trim(),
    fecha: document.getElementById('fecha').value,
    hora: hora.length === 5 ? `${hora}:00` : hora,
    tipoAtencion: document.getElementById('tipoAtencion').value,
    direccion: document.getElementById('tipoAtencion').value === 'Domicilio'
      ? document.getElementById('direccion').value.trim()
      : null
  };

  botonGuardar.disabled = true;
  botonGuardar.innerHTML =
    '<span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>Guardando...';

  try {
    if (idTurno) {
      await apiFetch(`/turnos/${idTurno}`, { method: 'PUT', body: datos });
      guardarMensaje('Turno actualizado correctamente.');
    } else {
      await apiFetch('/turnos', { method: 'POST', body: datos });
      guardarMensaje('Turno registrado correctamente.');
    }

    location.href = 'turnos.html';
  } catch (error) {
    botonGuardar.disabled = false;
    botonGuardar.textContent = idTurno ? 'Guardar cambios' : 'Guardar turno';

    if (error.status === 400) {
      mostrarErroresValidacion(error.datos);
    } else {
      mostrarErrorGeneral(error.message);
      mostrarToast(error.message, 'danger');
    }

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}

function mostrarErroresValidacion(datos) {
  const contenedor = document.getElementById('errores');
  const lista = document.getElementById('lista-errores');
  lista.innerHTML = '';

  const mensajes = datos && datos.errors
    ? Object.values(datos.errors).flat()
    : [datos && (datos.detail || datos.title) || 'Revisá los datos ingresados.'];

  mensajes.forEach((mensaje) => {
    const elemento = document.createElement('li');
    elemento.textContent = mensaje;
    lista.appendChild(elemento);
  });

  contenedor.classList.remove('d-none');
}

function mostrarErrorGeneral(mensaje) {
  const contenedor = document.getElementById('errores');
  const lista = document.getElementById('lista-errores');
  lista.innerHTML = '';

  const elemento = document.createElement('li');
  elemento.textContent = mensaje;
  lista.appendChild(elemento);
  contenedor.classList.remove('d-none');
}

function limpiarErrores() {
  document.getElementById('errores').classList.add('d-none');
  document.getElementById('lista-errores').innerHTML = '';
}
