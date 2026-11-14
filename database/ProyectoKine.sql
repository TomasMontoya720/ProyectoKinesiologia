/* ============================================================================
   ProyectoKine - Script de creación de la base de datos (SQL Server)
   ----------------------------------------------------------------------------
   IMPORTANTE
   - Este script crea la base y las tablas de forma manual.
   - Si ya ejecutaste las migraciones de Entity Framework Core
     (dotnet ef database update), la base ya existe: NO hace falta ejecutarlo.
   - No mezclar ambos métodos sobre la misma base si se van a aplicar
     migraciones después.
   ============================================================================ */

IF DB_ID('ProyectoKine') IS NULL
BEGIN
    CREATE DATABASE ProyectoKine;
END
GO

USE ProyectoKine;
GO

/* ----------------------------------------------------------------------------
   Tabla Paciente
   ---------------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Paciente', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Paciente
    (
        IdPaciente   INT IDENTITY(1,1) NOT NULL,
        Nombre       VARCHAR(50)  NOT NULL,
        Edad         INT          NOT NULL,
        Sexo         VARCHAR(20)  NOT NULL,
        Telefono     VARCHAR(20)  NOT NULL,
        Patologia    VARCHAR(200) NOT NULL,
        TipoAtencion VARCHAR(20)  NOT NULL,
        Direccion    VARCHAR(100) NULL,
        CONSTRAINT PK_Paciente PRIMARY KEY (IdPaciente)
    );

    CREATE INDEX IX_Paciente_Telefono ON dbo.Paciente (Telefono);
END
GO

/* ----------------------------------------------------------------------------
   Tabla Turno
   ---------------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Turno', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Turno
    (
        IdTurno    INT IDENTITY(1,1) NOT NULL,
        Fecha      DATE         NOT NULL,
        Hora       TIME(0)      NOT NULL,
        Estado     VARCHAR(20)  NOT NULL CONSTRAINT DF_Turno_Estado DEFAULT ('Pendiente'),
        IdPaciente INT          NOT NULL,
        CONSTRAINT PK_Turno PRIMARY KEY (IdTurno),
        CONSTRAINT FK_Turno_Paciente FOREIGN KEY (IdPaciente)
            REFERENCES dbo.Paciente (IdPaciente)
            ON DELETE NO ACTION
    );

    CREATE INDEX IX_Turno_Fecha_Hora ON dbo.Turno (Fecha, Hora);
    CREATE INDEX IX_Turno_IdPaciente ON dbo.Turno (IdPaciente);
END
GO

/* ----------------------------------------------------------------------------
   Datos de ejemplo (solo si la tabla está vacía)
   ---------------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Paciente)
BEGIN
    DECLARE @Paciente1 INT;
    DECLARE @Paciente2 INT;
    DECLARE @Paciente3 INT;
    DECLARE @Paciente4 INT;

    INSERT INTO dbo.Paciente (Nombre, Edad, Sexo, Telefono, Patologia, TipoAtencion, Direccion)
    VALUES ('Marta Gómez', 58, 'Femenino', '11 5555-1234', 'Lumbalgia', 'Consultorio', NULL);
    SET @Paciente1 = SCOPE_IDENTITY();

    INSERT INTO dbo.Paciente (Nombre, Edad, Sexo, Telefono, Patologia, TipoAtencion, Direccion)
    VALUES ('Juan Pérez', 42, 'Masculino', '11 5555-5678', 'Rehabilitación de rodilla', 'Domicilio', 'Av. Siempre Viva 742');
    SET @Paciente2 = SCOPE_IDENTITY();

    INSERT INTO dbo.Paciente (Nombre, Edad, Sexo, Telefono, Patologia, TipoAtencion, Direccion)
    VALUES ('Lucía Fernández', 33, 'Femenino', '11 5555-9012', 'Cervicalgia', 'Consultorio', NULL);
    SET @Paciente3 = SCOPE_IDENTITY();

    INSERT INTO dbo.Paciente (Nombre, Edad, Sexo, Telefono, Patologia, TipoAtencion, Direccion)
    VALUES ('Carlos Rodríguez', 67, 'Masculino', '11 5555-3456', 'Rehabilitación postoperatoria de cadera', 'Domicilio', 'Rivadavia 1250, 2º B');
    SET @Paciente4 = SCOPE_IDENTITY();

    INSERT INTO dbo.Turno (Fecha, Hora, Estado, IdPaciente)
    VALUES
        (CAST(GETDATE() AS DATE), '09:00', 'Pendiente', @Paciente1),
        (CAST(GETDATE() AS DATE), '10:30', 'Pendiente', @Paciente2),
        (CAST(GETDATE() AS DATE), '12:00', 'Cancelado', @Paciente3),
        (DATEADD(DAY, 1, CAST(GETDATE() AS DATE)), '09:30', 'Pendiente', @Paciente4),
        (DATEADD(DAY, 1, CAST(GETDATE() AS DATE)), '11:00', 'Pendiente', @Paciente1);
END
GO

/* ----------------------------------------------------------------------------
   Verificación rápida
   ---------------------------------------------------------------------------- */
SELECT * FROM dbo.Paciente;
SELECT * FROM dbo.Turno;
GO
