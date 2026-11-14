# Diagrama de Casos de Uso

Actor principal: **Kinesióloga** (única usuaria, sin login ni roles).

```mermaid
flowchart LR
    K([Kinesióloga])

    subgraph KineTurnos[Sistema KineTurnos]
        CU1([Registrar turno])
        CU2([Editar turno])
        CU3([Cancelar turno])
        CU4([Consultar turnos])
        CU5([Consultar datos del paciente])
        CU6([Validar datos obligatorios])
        CU7([Filtrar turnos del día o por fecha])
        CU8([Confirmar cancelación])
        CU9([Buscar paciente por nombre o teléfono])
    end

    K --> CU1
    K --> CU2
    K --> CU3
    K --> CU4
    K --> CU5

    CU1 -.->|include| CU6
    CU2 -.->|include| CU6
    CU3 -.->|include| CU8
    CU4 -.->|extend| CU7
    CU4 -.->|extend| CU9
    CU5 -.->|extend| CU4
```

## Descripción de los casos de uso

| Caso de uso | Descripción | Requisito |
|---|---|---|
| Registrar turno | Carga los datos del paciente y del turno (fecha, hora, modalidad y dirección si es a domicilio). | RF01, RF06, RF07 |
| Editar turno | Modifica los datos del paciente y/o el horario y la modalidad. | RF02 |
| Cancelar turno | Cambia el estado del turno a `Cancelado` previa confirmación. | RF03 |
| Consultar turnos | Muestra todos los turnos, los del día o los de una fecha determinada. | RF04 |
| Consultar datos del paciente | Muestra la ficha completa y el historial de turnos. | RF05 |
| Validar datos obligatorios | Valida campos requeridos en el frontend y en la API. | RF06, RF07 |

## Flujo general

1. La kinesióloga abre el inicio y elige **Agregar turno** o **Ver turnos**.
2. En el alta completa los datos y el sistema valida antes de guardar.
3. En la agenda puede filtrar, buscar, editar o cancelar.
4. Desde cada fila puede abrir la ficha del paciente con su historial.
