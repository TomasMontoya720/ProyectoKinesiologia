IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Paciente] (
    [IdPaciente] int NOT NULL IDENTITY,
    [Nombre] varchar(50) NOT NULL,
    [Edad] int NOT NULL,
    [Sexo] varchar(20) NOT NULL,
    [Telefono] varchar(20) NOT NULL,
    [Patologia] varchar(200) NOT NULL,
    [TipoAtencion] varchar(20) NOT NULL,
    [Direccion] varchar(100) NULL,
    CONSTRAINT [PK_Paciente] PRIMARY KEY ([IdPaciente])
);

CREATE TABLE [Turno] (
    [IdTurno] int NOT NULL IDENTITY,
    [Fecha] date NOT NULL,
    [Hora] time(0) NOT NULL,
    [Estado] varchar(20) NOT NULL DEFAULT 'Pendiente',
    [IdPaciente] int NOT NULL,
    CONSTRAINT [PK_Turno] PRIMARY KEY ([IdTurno]),
    CONSTRAINT [FK_Turno_Paciente] FOREIGN KEY ([IdPaciente]) REFERENCES [Paciente] ([IdPaciente]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Paciente_Telefono] ON [Paciente] ([Telefono]);

CREATE INDEX [IX_Turno_Fecha_Hora] ON [Turno] ([Fecha], [Hora]);

CREATE INDEX [IX_Turno_IdPaciente] ON [Turno] ([IdPaciente]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261114033905_Inicial', N'10.0.15');

COMMIT;
GO

