# Diagrama de Secuencia · Agregar Turno

Flujo completo desde el formulario HTML hasta SQL Server, incluyendo las validaciones y los errores posibles.

```mermaid
sequenceDiagram
    autonumber
    actor K as Kinesióloga
    participant F as turno-form.html
    participant C as TurnosController
    participant M as Middleware de errores
    participant S as TurnoServicio
    participant PR as PacienteRepositorio
    participant TR as TurnoRepositorio
    participant DB as SQL Server

    K->>F: Completa datos del paciente y del turno
    F->>F: Validación HTML5 + JavaScript
    alt Datos incompletos
        F-->>K: Marca campos inválidos y no envía
    else Datos válidos
        F->>C: POST /api/turnos (TurnoFormDTO)
        C->>C: Valida Data Annotations
        alt Validación de modelo falla
            C-->>F: 400 ProblemDetails (errores)
            F-->>K: Muestra la lista de errores
        else Modelo válido
            C->>S: CrearAsync(dto)
            S->>S: Verifica que la fecha no sea pasada
            alt Fecha inválida
                S-->>M: ReglaNegocioException
                M-->>F: 409 ProblemDetails
                F-->>K: Muestra el mensaje
            else Fecha válida
                S->>S: Abre transacción
                S->>TR: ExisteActivoEnHorario(fecha, hora)
                TR->>DB: SELECT Turno WHERE fecha y hora
                DB-->>TR: Resultado
                alt Horario ocupado
                    S-->>M: ReglaNegocioException
                    M-->>F: 409 ProblemDetails
                    F-->>K: "Ya existe un turno activo..."
                else Horario libre
                    S->>PR: BuscarPorTelefono(telefono)
                    PR->>DB: SELECT Paciente
                    DB-->>PR: Paciente o NULL
                    alt Paciente existente
                        S->>PR: Update(paciente)
                        PR->>DB: UPDATE Paciente
                    else Paciente nuevo
                        S->>PR: Insert(paciente)
                        PR->>DB: INSERT Paciente
                        DB-->>PR: IdPaciente generado
                    end
                    S->>TR: Insert(turno)
                    TR->>DB: INSERT Turno (Estado = Pendiente)
                    S->>S: Commit de la transacción
                    S-->>C: TurnoDetalleDTO
                    C-->>F: 201 Created + datos del turno
                    F->>F: Guarda mensaje de confirmación
                    F-->>K: Redirige a turnos.html y muestra el toast
                end
            end
        end
    end
```

## Puntos clave

- El formulario y la API validan los datos (doble validación).
- El paciente y el turno se guardan en una **transacción**: si falla el turno, no queda un paciente huérfano.
- El paciente se reutiliza por teléfono cuando ya existe.
- Un turno cancelado no bloquea el horario; uno pendiente sí.
- Los errores de negocio se convierten en HTTP 409 y los recursos inexistentes en HTTP 404 mediante el middleware.
