# Diagrama de Clases

Vista simplificada de las clases principales del backend y de la relación entre capas.

```mermaid
classDiagram
    direction LR

    class Paciente {
        +int IdPaciente
        +string Nombre
        +int Edad
        +string Sexo
        +string Telefono
        +string Patologia
        +string TipoAtencion
        +string? Direccion
        +ICollection~Turno~ Turnos
    }

    class Turno {
        +int IdTurno
        +DateOnly Fecha
        +TimeOnly Hora
        +string Estado
        +int IdPaciente
        +Paciente Paciente
    }

    Paciente "1" --> "0..*" Turno : tiene

    class PacienteDatosDTO {
        <<DTO>>
        +string Nombre
        +int Edad
        +string Sexo
        +string Telefono
        +string Patologia
        +string TipoAtencion
        +string? Direccion
    }

    class PacienteFormDTO {
        <<DTO>>
    }

    class TurnoFormDTO {
        <<DTO>>
        +int? IdPaciente
        +DateOnly? Fecha
        +TimeOnly? Hora
    }

    class PacienteDTO {
        <<DTO>>
        +int IdPaciente
    }

    class PacienteDetalleDTO {
        <<DTO>>
        +List~TurnoListadoDTO~ Turnos
    }

    class TurnoListadoDTO {
        <<DTO>>
        +int IdTurno
        +int IdPaciente
        +string Nombre
        +string Telefono
        +string Patologia
        +DateOnly Fecha
        +TimeOnly Hora
        +string TipoAtencion
        +string Estado
    }

    class TurnoDetalleDTO {
        <<DTO>>
        +int IdTurno
        +DateOnly Fecha
        +TimeOnly Hora
        +string Estado
        +PacienteDTO Paciente
    }

    PacienteDatosDTO <|-- PacienteFormDTO
    PacienteDatosDTO <|-- TurnoFormDTO
    PacienteDatosDTO <|-- PacienteDTO
    PacienteDTO <|-- PacienteDetalleDTO

    class AppDbContext {
        +DbSet~Paciente~ Pacientes
        +DbSet~Turno~ Turnos
        +OnModelCreating(ModelBuilder)
    }

    class IRepositorio~E~ {
        <<interface>>
        +Select() List~E~
        +SelectById(int) E
        +Insert(E) E
        +Update(E) bool
        +Delete(int) bool
        +Existe(int) bool
    }

    class IPacienteRepositorio {
        <<interface>>
        +BuscarPorTelefono(string) Paciente
        +Buscar(string?) List~Paciente~
        +ObtenerConTurnos(int) Paciente
    }

    class ITurnoRepositorio {
        <<interface>>
        +Listar(DateOnly?, string?, string?) List~Turno~
        +ObtenerDetalle(int) Turno
        +ObtenerParaEditar(int) Turno
        +ExisteActivoEnHorario(DateOnly, TimeOnly, int?) bool
    }

    class PacienteRepositorio {
        +BuscarPorTelefono(string) Paciente
        +Buscar(string?) List~Paciente~
        +ObtenerConTurnos(int) Paciente
    }

    class TurnoRepositorio {
        +Listar(DateOnly?, string?, string?) List~Turno~
        +ObtenerDetalle(int) Turno
        +ObtenerParaEditar(int) Turno
        +ExisteActivoEnHorario(DateOnly, TimeOnly, int?) bool
    }

    IRepositorio~E~ <|.. IPacienteRepositorio
    IRepositorio~E~ <|.. ITurnoRepositorio
    IPacienteRepositorio <|.. PacienteRepositorio
    ITurnoRepositorio <|.. TurnoRepositorio

    class IPacienteServicio {
        <<interface>>
        +ListarAsync(string?) List~PacienteDTO~
        +ObtenerAsync(int) PacienteDetalleDTO
        +CrearAsync(PacienteFormDTO) PacienteDTO
        +ActualizarAsync(int, PacienteFormDTO) PacienteDTO
        +EliminarAsync(int)
    }

    class ITurnoServicio {
        <<interface>>
        +ListarAsync(DateOnly?, string?, string?) List~TurnoListadoDTO~
        +ListarHoyAsync() List~TurnoListadoDTO~
        +ObtenerAsync(int) TurnoDetalleDTO
        +CrearAsync(TurnoFormDTO) TurnoDetalleDTO
        +ActualizarAsync(int, TurnoFormDTO) TurnoDetalleDTO
        +CancelarAsync(int)
    }

    class PacienteServicio
    class TurnoServicio

    IPacienteServicio <|.. PacienteServicio
    ITurnoServicio <|.. TurnoServicio

    class PacientesController {
        +Listar(string?)
        +Obtener(int)
        +ListarTurnos(int)
        +Crear(PacienteFormDTO)
        +Actualizar(int, PacienteFormDTO)
        +Eliminar(int)
    }

    class TurnosController {
        +Listar(DateOnly?, string?, string?)
        +ListarHoy()
        +ListarPorFecha(DateTime)
        +Obtener(int)
        +Crear(TurnoFormDTO)
        +Actualizar(int, TurnoFormDTO)
        +Cancelar(int)
    }

    PacientesController --> IPacienteServicio
    TurnosController --> ITurnoServicio
    PacienteServicio --> IPacienteRepositorio
    TurnoServicio --> ITurnoRepositorio
    TurnoServicio --> IPacienteRepositorio
    TurnoServicio --> AppDbContext
    PacienteRepositorio --> AppDbContext
    TurnoRepositorio --> AppDbContext
```

> En el diagrama, `PacienteServicio` depende de `IPacienteRepositorio`; `TurnoServicio` usa `ITurnoRepositorio`, `IPacienteRepositorio` y `AppDbContext` (solo para coordinar la transacción de alta).
