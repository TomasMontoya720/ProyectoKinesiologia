# KineTurnos · Gestión de turnos de kinesiología

Aplicación web para que una kinesióloga administre los turnos de sus pacientes. Permite registrar pacientes y turnos, editarlos, cancelarlos, consultar la agenda del día o de una fecha y ver la ficha de cada paciente.

**Usuario:** kinesióloga (única usuaria). No hay login, registro de usuarios, roles ni permisos.

## Tecnologías

- **Backend:** ASP.NET Core Web API (.NET 10), controllers, Entity Framework Core, LINQ, inyección de dependencias, DTOs y Swagger.
- **Frontend:** HTML5, CSS3, Bootstrap 5.3.3 y JavaScript con Fetch API.
- **Base de datos:** SQL Server (LocalDB en desarrollo), Code First y migraciones.
- **Arquitectura:** tres capas (frontend → Web API → SQL Server), con proyectos separados de datos, repositorio y servicios.

## Funcionalidades

1. **Agregar turno:** datos del paciente + fecha, hora y modalidad (Consultorio/Domicilio con dirección).
2. **Editar turno:** modifica paciente, fecha, hora, modalidad y dirección.
3. **Cancelar turno:** confirmación y cambio de estado a `Cancelado` (baja lógica).
4. **Ver turnos:** todos, los de hoy o los de una fecha; búsqueda por nombre, teléfono o patología.
5. **Consultar paciente:** ficha completa e historial de turnos.
6. **Validaciones:** campos obligatorios, fecha/hora no vacías, dirección obligatoria para Domicilio, edad y teléfono válidos, horario duplicado.

## Requisitos previos

- [.NET SDK 10](https://dotnet.microsoft.com/download) o superior.
- SQL Server LocalDB (incluido con Visual Studio) o una instancia de SQL Server.
- Herramienta `dotnet-ef` (solo si se quieren ejecutar comandos de migración manualmente):

```bash
dotnet tool install --global dotnet-ef --version 10.0.15
```

Verificar LocalDB:

```bash
sqllocaldb info MSSQLLocalDB
sqllocaldb start MSSQLLocalDB
```

## Puesta en marcha

### 1. Restaurar y compilar

Desde la carpeta raíz de la solución:

```bash
dotnet restore ProyectoKinesiologia.slnx
dotnet build ProyectoKinesiologia.slnx
```

### 2. Elegir cómo crear la base de datos

La cadena de conexión está en `ProyectoKinesiologia/appsettings.json`:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProyectoKine;Trusted_Connection=true;TrustServerCertificate=True"
```

**Opción A (recomendada): migraciones de Entity Framework Core**

```bash
dotnet ef database update --project ProyectoKine.BD --startup-project ProyectoKinesiologia
```

> En ambiente `Development`, la API también aplica automáticamente las migraciones al iniciar.

**Opción B: script SQL manual**

Abrir `database/ProyectoKine.sql` en SQL Server Management Studio y ejecutarlo. Incluye el esquema y datos de ejemplo.

> No mezclar ambos métodos sobre la misma base. Si se usó el script y luego se quieren migraciones, hay que recrear la base.

### 3. Ejecutar la API

```bash
dotnet run --project ProyectoKinesiologia
```

La aplicación se abre en el **mismo origen** que la API:

- **Frontend:** <http://localhost:5132/>
- **Swagger:** <http://localhost:5132/swagger>
- **API:** <http://localhost:5132/api/turnos>

> **Importante:** abrí siempre el frontend desde `http://localhost:5132` (o `http://127.0.0.1:5132`). No abras los `.html` con doble clic (quedan como `file:///`) ni mezcles orígenes: el frontend llama a `/api/...` en el mismo puerto desde el que se sirve.
>
> El perfil `http` es el predeterminado y es el que usa Visual Studio al presionar F5. Si preferís HTTPS, confiá primero el certificado con `dotnet dev-certs https --trust`, ejecutá `dotnet run --project ProyectoKinesiologia --launch-profile https` y entrá a <https://localhost:7251/> (ese mismo origen también sirve la API).

## Endpoints

### Pacientes

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/pacientes?buscar=texto` | Lista y busca pacientes |
| GET | `/api/pacientes/{id}` | Ficha del paciente + historial |
| GET | `/api/pacientes/{id}/turnos` | Turnos del paciente |
| POST | `/api/pacientes` | Crea un paciente |
| PUT | `/api/pacientes/{id}` | Modifica un paciente |
| DELETE | `/api/pacientes/{id}` | Elimina (409 si tiene turnos) |

### Turnos

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/turnos?fecha=&estado=&buscar=` | Lista con filtros opcionales |
| GET | `/api/turnos/hoy` | Turnos del día |
| GET | `/api/turnos/fecha/{fecha}` | Turnos de una fecha (`AAAA-MM-DD`) |
| GET | `/api/turnos/{id}` | Detalle con paciente |
| POST | `/api/turnos` | Crea paciente (si hace falta) + turno |
| PUT | `/api/turnos/{id}` | Modifica turno + paciente |
| DELETE | `/api/turnos/{id}` | Cancela el turno (estado `Cancelado`) |

### Ejemplo: crear un turno

```http
POST http://localhost:5132/api/turnos
Content-Type: application/json
```

```json
{
  "idPaciente": null,
  "nombre": "Ana López",
  "edad": 45,
  "sexo": "Femenino",
  "telefono": "11 5555-0000",
  "patologia": "Rehabilitación de hombro",
  "fecha": "2026-11-20",
  "hora": "10:30:00",
  "tipoAtencion": "Domicilio",
  "direccion": "Av. Callao 123"
}
```

Respuesta `201 Created`:

```json
{
  "idTurno": 1,
  "fecha": "2026-11-20",
  "hora": "10:30:00",
  "estado": "Pendiente",
  "paciente": {
    "idPaciente": 1,
    "nombre": "Ana López",
    "edad": 45,
    "sexo": "Femenino",
    "telefono": "11 5555-0000",
    "patologia": "Rehabilitación de hombro",
    "tipoAtencion": "Domicilio",
    "direccion": "Av. Callao 123"
  }
}
```

Si el teléfono ya existe, se reutiliza el paciente y se actualizan sus datos. Si ya hay un turno activo a esa fecha y hora, se devuelve `409 Conflict`.

### Ejemplo: cancelar

```http
DELETE http://localhost:5132/api/turnos/1
```

Respuesta `204 No Content`. El turno queda en la base con `Estado = 'Cancelado'`.

### Códigos de error

| Código | Cuándo |
|---|---|
| 400 | Validación de campos (ProblemDetails con `errors`) |
| 404 | Paciente o turno inexistente |
| 409 | Horario ocupado, fecha pasada, paciente con turnos |
| 500 | Error inesperado (se registra en el log) |

## Estructura de la solución

```text
ProyectoKine.Shared         DTOs y constantes
ProyectoKine.BD             Entidades, DbContext, configuraciones y migraciones
ProyectoKine.Repositorio    Repositorios EF Core + LINQ
ProyectoKine.Servicio       Reglas de negocio, mapeo y transacciones
ProyectoKinesiologia        Web API, Swagger y frontend en wwwroot
database/                   Scripts SQL
docs/                       Diagramas y documentación
```

Detalle completo: [`docs/06-estructura-carpetas.md`](docs/06-estructura-carpetas.md).

## Documentación y diagramas

- [DER](docs/01-der.md)
- [Casos de uso](docs/02-casos-de-uso.md)
- [Diagrama de clases](docs/03-diagrama-clases.md)
- [Secuencia · Agregar turno](docs/04-secuencia-agregar-turno.md)
- [Arquitectura](docs/05-arquitectura.md)
- [Estructura de carpetas](docs/06-estructura-carpetas.md)

Los diagramas están escritos en Mermaid y se visualizan directamente en GitHub o en VS Code (extensión Markdown Preview Mermaid Support).

## Comandos útiles

Crear una migración nueva después de cambiar el modelo:

```bash
dotnet ef migrations add NombreDeLaMigracion --project ProyectoKine.BD --startup-project ProyectoKinesiologia --output-dir Migrations
```

Generar el script SQL de las migraciones:

```bash
dotnet ef migrations script --project ProyectoKine.BD --startup-project ProyectoKinesiologia --output database/ProyectoKine-Migraciones.sql
```

Recrear la base desde cero:

```bash
dotnet ef database drop --project ProyectoKine.BD --startup-project ProyectoKinesiologia --force
dotnet ef database update --project ProyectoKine.BD --startup-project ProyectoKinesiologia
```

## Solución de problemas

- **No conecta a LocalDB:** ejecutar `sqllocaldb start MSSQLLocalDB` y verificar la cadena de conexión.
- **Puerto ocupado:** cambiar `applicationUrl` en `ProyectoKinesiologia/Properties/launchSettings.json`.
- **`dotnet ef` no existe:** instalar la herramienta global indicada en requisitos.
- **Cambié el modelo y la base no coincide:** crear una migración nueva y ejecutar `database update`.
- **Aparece "No se pudo conectar con la API...":** revisar la barra de direcciones. Debe ser `http://localhost:5132/...` (o `http://127.0.0.1:5132/...`). Si dice `https://localhost:7251` y ese perfil no está activo, o si dice `file:///`, abrí la app desde el puerto correcto.
- **No cargan los estilos o scripts:** confirmar que se abre la aplicación desde `http://localhost:5132` y no abriendo los `.html` directamente como archivos.
- **La API responde pero el frontend muestra error de conexión:** verificar que el proceso `dotnet run` siga activo y que no estés mezclando el puerto de la página con el de la API.

## Uso de la aplicación

1. En **Inicio** se ve el resumen del día y los accesos rápidos.
2. **Agregar turno** abre el formulario; la dirección aparece solo si se elige Domicilio.
3. **Ver turnos** permite filtrar por todos, hoy o una fecha, buscar por paciente y cancelar con confirmación.
4. **Ver** (en cada fila) abre la ficha del paciente con su historial.
5. **Editar** permite modificar el paciente y el turno.
