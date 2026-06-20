-- ===================================================================
-- SQL SCRIPT: Reinicio e Inserción de Datos de Prueba (Pedro Elector)
-- Proposito: Vaciar todas las tablas y registrar al elector específico
--            04900525207 para simulaciones, además de datos base activos.
-- Contraseña para usuarios administradores/dirigentes: Password123*
-- ===================================================================

USE [eVote360_Pro_DB];
GO

-- 1. LIMPIEZA DE TABLAS Y REESTABLECIMIENTO DE CONTADORES (Identities)
-- Desactivar restricciones temporalmente
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all";

-- Vaciar tablas
DELETE FROM [Votos];
DELETE FROM [CandidatosPuestos];
DELETE FROM [SolicitudesAlianzas];
DELETE FROM [CodigosVerificacion];
DELETE FROM [Candidatos];
DELETE FROM [AsignacionesDirigentes];
DELETE FROM [Usuarios];
DELETE FROM [Ciudadanos];
DELETE FROM [Elecciones];
DELETE FROM [PartidosPoliticos];
DELETE FROM [PuestosElectivos];

-- Reiniciar contadores de identidad a 0
DBCC CHECKIDENT ('Votos', RESEED, 0);
DBCC CHECKIDENT ('CandidatosPuestos', RESEED, 0);
DBCC CHECKIDENT ('SolicitudesAlianzas', RESEED, 0);
DBCC CHECKIDENT ('CodigosVerificacion', RESEED, 0);
DBCC CHECKIDENT ('Candidatos', RESEED, 0);
DBCC CHECKIDENT ('AsignacionesDirigentes', RESEED, 0);
DBCC CHECKIDENT ('Usuarios', RESEED, 0);
DBCC CHECKIDENT ('Ciudadanos', RESEED, 0);
DBCC CHECKIDENT ('Elecciones', RESEED, 0);
DBCC CHECKIDENT ('PartidosPoliticos', RESEED, 0);
DBCC CHECKIDENT ('PuestosElectivos', RESEED, 0);

-- Reactivar restricciones
EXEC sp_MSforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all";
GO


-- 2. INSERTAR PUESTOS ELECTIVOS (Activos = 1)
INSERT INTO [PuestosElectivos] ([Nombre], [Descripcion], [Estado]) VALUES ('Presidente', 'Máxima autoridad del poder ejecutivo nacional.', 1);
INSERT INTO [PuestosElectivos] ([Nombre], [Descripcion], [Estado]) VALUES ('Senador', 'Representante de la provincia en la cámara alta legislativa.', 1);
INSERT INTO [PuestosElectivos] ([Nombre], [Descripcion], [Estado]) VALUES ('Diputado', 'Representante del pueblo en la cámara baja legislativa.', 1);
INSERT INTO [PuestosElectivos] ([Nombre], [Descripcion], [Estado]) VALUES ('Alcalde', 'Autoridad del gobierno local del municipio.', 1);
GO


-- 3. INSERTAR PARTIDOS POLITICOS (Activos = 1)
INSERT INTO [PartidosPoliticos] ([Nombre], [Descripcion], [Siglas], [LogoUrl], [Estado]) VALUES ('Partido de la Democracia Avanzada', 'Organización progresista enfocada en derechos sociales.', 'PDA', '/images/partidos/pda_logo.png', 1);
INSERT INTO [PartidosPoliticos] ([Nombre], [Descripcion], [Siglas], [LogoUrl], [Estado]) VALUES ('Unión Liberal Dominicana', 'Partido centrado en la libertad económica y libre mercado.', 'ULD', '/images/partidos/uld_logo.png', 1);
INSERT INTO [PartidosPoliticos] ([Nombre], [Descripcion], [Siglas], [LogoUrl], [Estado]) VALUES ('Fuerza del Cambio Democrático', 'Movimiento ecologista y de participación ciudadana.', 'FCD', '/images/partidos/fcd_logo.png', 1);
GO


-- 4. INSERTAR USUARIOS ADMINISTRADORES Y DIRIGENTES (Contraseña: Password123*)
INSERT INTO [Usuarios] ([Nombre], [Apellido], [CorreoElectronico], [NombreUsuario], [PasswordHash], [Rol], [Estado]) 
VALUES ('Juan', 'Administrador', 'juan.admin@evote360.com', 'admin_juan', '$2a$11$ADyrSpGJESzOEBvDcoto6uEnPU16ahzw2tQmSOEZN9edd7RpshaiK', 'Administrador', 1);

INSERT INTO [Usuarios] ([Nombre], [Apellido], [CorreoElectronico], [NombreUsuario], [PasswordHash], [Rol], [Estado]) 
VALUES ('Maria', 'Gomez', 'maria.dirigente@evote360.com', 'dirigente_pda', '$2a$11$ADyrSpGJESzOEBvDcoto6uEnPU16ahzw2tQmSOEZN9edd7RpshaiK', 'Dirigente político', 1);

INSERT INTO [Usuarios] ([Nombre], [Apellido], [CorreoElectronico], [NombreUsuario], [PasswordHash], [Rol], [Estado]) 
VALUES ('Carlos', 'Ramirez', 'carlos.dirigente@evote360.com', 'dirigente_uld', '$2a$11$ADyrSpGJESzOEBvDcoto6uEnPU16ahzw2tQmSOEZN9edd7RpshaiK', 'Dirigente político', 1);
GO


-- 5. ASIGNACIONES DE DIRIGENTES A PARTIDOS
DECLARE @DirPDA INT = (SELECT Id FROM [Usuarios] WHERE [NombreUsuario] = 'dirigente_pda');
DECLARE @DirULD INT = (SELECT Id FROM [Usuarios] WHERE [NombreUsuario] = 'dirigente_uld');

DECLARE @PartPDA INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'PDA');
DECLARE @PartULD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'ULD');

IF @DirPDA IS NOT NULL AND @PartPDA IS NOT NULL
    INSERT INTO [AsignacionesDirigentes] ([UsuarioId], [PartidoPoliticoId]) VALUES (@DirPDA, @PartPDA);

IF @DirULD IS NOT NULL AND @PartULD IS NOT NULL
    INSERT INTO [AsignacionesDirigentes] ([UsuarioId], [PartidoPoliticoId]) VALUES (@DirULD, @PartULD);
GO


-- 6. INSERTAR CIUDADANO ELECTOR (Pedro Emmanuel - Activo y No Votado)
INSERT INTO [Ciudadanos] ([Nombre], [Apellido], [CorreoElectronico], [DocumentoIdentidad], [Estado], [YaVoto]) 
VALUES ('Pedro Emmanuel', 'Rosario Sánchez', 'pedroemmaul540@gmail.com', '04900525207', 1, 0);

-- Insertamos también un par de ciudadanos extras para pruebas adicionales
INSERT INTO [Ciudadanos] ([Nombre], [Apellido], [CorreoElectronico], [DocumentoIdentidad], [Estado], [YaVoto]) 
VALUES ('Ana', 'Reyes', 'ana.reyes@gmail.com', '10100100102', 1, 0);
GO


-- 7. INSERTAR CANDIDATOS (EsActivo = 1, asignados a partidos y puestos)
DECLARE @PartPDA INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'PDA');
DECLARE @PartULD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'ULD');
DECLARE @PartFCD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'FCD');

DECLARE @PuestoPres INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Presidente');
DECLARE @PuestoSen INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Senador');
DECLARE @PuestoDip INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Diputado');
DECLARE @PuestoAlc INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Alcalde');

-- Candidatos Presidenciales
INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) 
VALUES ('Alvaro', 'Guzman', '20200200201', 'alvaro.pda@evote360.com', '/images/candidatos/guzman.jpg', 1, @PartPDA, @PuestoPres);

INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) 
VALUES ('Beatriz', 'Mejia', '20200200202', 'beatriz.uld@evote360.com', '/images/candidatos/mejia.jpg', 1, @PartULD, @PuestoPres);

-- Candidatos Senaduría
INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) 
VALUES ('Diana', 'Herrera', '20200200204', 'diana.pda@evote360.com', '/images/candidatos/herrera.jpg', 1, @PartPDA, @PuestoSen);

INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) 
VALUES ('Eduardo', 'Vargas', '20200200205', 'eduardo.uld@evote360.com', '/images/candidatos/vargas.jpg', 1, @PartULD, @PuestoSen);

-- Candidatos Alcaldía
INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) 
VALUES ('Carlos', 'Mendoza', '20200200206', 'carlos.fcd@evote360.com', '/images/candidatos/mendoza.jpg', 1, @PartFCD, @PuestoAlc);
GO


-- 8. INSERTAR CANDIDATOS PUESTOS (Relación para visualización en cabinas y alianzas)
DECLARE @CandAlvaro INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Alvaro');
DECLARE @CandBeatriz INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Beatriz');
DECLARE @CandDiana INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Diana');
DECLARE @CandEduardo INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Eduardo');
DECLARE @CandCarlos INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Carlos');

DECLARE @PartPDA INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'PDA');
DECLARE @PartULD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'ULD');
DECLARE @PartFCD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'FCD');

DECLARE @PuestoPres INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Presidente');
DECLARE @PuestoSen INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Senador');
DECLARE @PuestoAlc INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Alcalde');

INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandAlvaro, @PuestoPres, @PartPDA, 0);
INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandBeatriz, @PuestoPres, @PartULD, 0);
INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandDiana, @PuestoSen, @PartPDA, 0);
INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandEduardo, @PuestoSen, @PartULD, 0);
INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandCarlos, @PuestoAlc, @PartFCD, 0);
GO


-- 9. INSERTAR ELECCION ACTIVA (Estado = 2 es EstadoEleccion.Activa)
INSERT INTO [Elecciones] ([Nombre], [FechaRealizacion], [Estado]) 
VALUES ('Elecciones Generales Ordinarias 2026', GETDATE(), 2);
GO
