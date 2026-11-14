# Estructura de carpetas del proyecto

```text
ProyectoKinesiologia/
├── ProyectoKinesiologia.slnx              Solución de Visual Studio
├── README.md                              Guía de instalación y ejecución
├── .gitignore                             Archivos generados que no se versionan
├── docs/                                  Documentación y diagramas Mermaid
│   ├── 01-der.md
│   ├── 02-casos-de-uso.md
│   ├── 03-diagrama-clases.md
│   ├── 04-secuencia-agregar-turno.md
│   ├── 05-arquitectura.md
│   └── 06-estructura-carpetas.md
├── database/
│   ├── ProyectoKine.sql                   Script manual de creación + datos de ejemplo
│   └── ProyectoKine-Migraciones.sql       Script SQL generado por EF Core
│
├── ProyectoKine.Shared/                   Contratos y constantes compartidas
│   ├── Constantes/
│   │   ├── EstadosTurno.cs
│   │   └── TiposAtencion.cs
│   ├── DTOs/
│   │   ├── PacienteDatosDTO.cs            Campos y validaciones comunes
│   │   ├── PacienteDTO.cs                 PacienteDTO, PacienteFormDTO y PacienteDetalleDTO
│   │   ├── TurnoFormDTO.cs                DTO compuesto para alta/edición
│   │   ├── TurnoListadoDTO.cs
│   │   └── TurnoDetalleDTO.cs
│   └── ProyectoKine.Shared.csproj
│
├── ProyectoKine.BD/                       Capa de datos (Code First)
│   ├── Configuraciones/
│   │   ├── PacienteConfiguracion.cs
│   │   └── TurnoConfiguracion.cs
│   ├── Contexto/
│   │   └── AppDbContext.cs
│   ├── Entidades/
│   │   ├── Paciente.cs
│   │   └── Turno.cs
│   ├── Migrations/                        Migración Inicial y snapshot de EF
│   └── ProyectoKine.BD.csproj
│
├── ProyectoKine.Repositorio/              Capa de acceso a datos
│   ├── Repositorios/
│   │   ├── IRepositorio.cs
│   │   ├── Repositorio.cs                 Implementación genérica
│   │   ├── PacienteRepositorio.cs
│   │   └── TurnoRepositorio.cs
│   └── ProyectoKine.Repositorio.csproj
│
├── ProyectoKine.Servicio/                 Capa de negocio
│   ├── Excepciones/
│   │   └── ReglaNegocioException.cs
│   ├── Mapeo/
│   │   └── MapeoExtensiones.cs
│   ├── Servicios/
│   │   ├── PacienteServicio.cs
│   │   └── TurnoServicio.cs
│   └── ProyectoKine.Servicio.csproj
│
└── ProyectoKinesiologia/                  Web API + frontend
    ├── Controllers/
    │   ├── PacientesController.cs
    │   └── TurnosController.cs
    ├── Middleware/
    │   └── ManejadorExcepcionesMiddleware.cs
    ├── Properties/
    │   └── launchSettings.json
    ├── wwwroot/
    │   ├── css/
    │   │   └── estilos.css
    │   ├── js/
    │   │   ├── api.js                     Fetch, formato de datos y toasts
    │   │   ├── inicio.js
    │   │   ├── turnos.js
    │   │   ├── turno-form.js
    │   │   └── paciente.js
    │   ├── lib/bootstrap/                 Bootstrap 5.3.3 local
    │   ├── favicon.svg                    Ícono vectorial del sistema
    │   ├── favicon.png                    Ícono alternativo
    │   ├── index.html                     Inicio
    │   ├── turnos.html                    Lista y filtros
    │   ├── turno-form.html                Alta y edición
    │   └── paciente.html                  Ficha e historial
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    └── ProyectoKinesiologia.Server.csproj
```

## Convenciones

- **Namespaces** alineados con las carpetas (`ProyectoKine.BD.Entidades`, `ProyectoKine.Servicio.Servicios`, etc.).
- **Código comentado** en español, especialmente reglas de negocio y configuraciones.
- **DTOs** separados de las entidades para no exponer el modelo de datos directamente.
- **Migraciones** dentro de `ProyectoKine.BD/Migrations`.
- Los archivos `bin/`, `obj/` y `.vs/` se generan automáticamente y no forman parte del código fuente.
