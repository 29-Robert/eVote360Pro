-- ===================================================================
-- SQL SCRIPT: Datos de prueba para eVote360 Pro (Idempotente)
-- Proposito: Cargar datos reales y simulados para auditar
-- las reglas de negocio, resumenes y bloqueos corregidos sin duplicados.
-- Contraseña para todos los usuarios: Password123*
-- ===================================================================

USE [eVote360Db];
GO

-- 1. PUESTOS ELECTIVOS (Insertar solo si no existen por Nombre)
IF NOT EXISTS (SELECT 1 FROM [PuestosElectivos] WHERE [Nombre] = 'Presidente')
    INSERT INTO [PuestosElectivos] ([Nombre], [Descripcion], [Estado]) VALUES ('Presidente', 'Máxima autoridad del poder ejecutivo nacional.', 1);

IF NOT EXISTS (SELECT 1 FROM [PuestosElectivos] WHERE [Nombre] = 'Senador')
    INSERT INTO [PuestosElectivos] ([Nombre], [Descripcion], [Estado]) VALUES ('Senador', 'Representante de la provincia en la cámara alta legislativa.', 1);

IF NOT EXISTS (SELECT 1 FROM [PuestosElectivos] WHERE [Nombre] = 'Diputado')
    INSERT INTO [PuestosElectivos] ([Nombre], [Descripcion], [Estado]) VALUES ('Diputado', 'Representante del pueblo en la cámara baja legislativa.', 1);

IF NOT EXISTS (SELECT 1 FROM [PuestosElectivos] WHERE [Nombre] = 'Alcalde')
    INSERT INTO [PuestosElectivos] ([Nombre], [Descripcion], [Estado]) VALUES ('Alcalde', 'Autoridad del gobierno local del municipio.', 1);

IF NOT EXISTS (SELECT 1 FROM [PuestosElectivos] WHERE [Nombre] = 'Regidor')
    INSERT INTO [PuestosElectivos] ([Nombre], [Descripcion], [Estado]) VALUES ('Regidor', 'Miembro de la junta municipal del ayuntamiento.', 0);
GO

-- 2. PARTIDOS POLITICOS (Insertar solo si no existen por Siglas)
IF NOT EXISTS (SELECT 1 FROM [PartidosPoliticos] WHERE [Siglas] = 'PDA')
    INSERT INTO [PartidosPoliticos] ([Nombre], [Descripcion], [Siglas], [LogoUrl], [Estado]) VALUES ('Partido de la Democracia Avanzada', 'Organización progresista enfocada en derechos sociales.', 'PDA', '/images/partidos/pda_logo.png', 1);

IF NOT EXISTS (SELECT 1 FROM [PartidosPoliticos] WHERE [Siglas] = 'ULD')
    INSERT INTO [PartidosPoliticos] ([Nombre], [Descripcion], [Siglas], [LogoUrl], [Estado]) VALUES ('Unión Liberal Dominicana', 'Partido centrado en la libertad económica y libre mercado.', 'ULD', '/images/partidos/uld_logo.png', 1);

IF NOT EXISTS (SELECT 1 FROM [PartidosPoliticos] WHERE [Siglas] = 'FCD')
    INSERT INTO [PartidosPoliticos] ([Nombre], [Descripcion], [Siglas], [LogoUrl], [Estado]) VALUES ('Fuerza del Cambio Democrático', 'Movimiento ecologista y de participación ciudadana.', 'FCD', '/images/partidos/fcd_logo.png', 1);

IF NOT EXISTS (SELECT 1 FROM [PartidosPoliticos] WHERE [Siglas] = 'PAT')
    INSERT INTO [PartidosPoliticos] ([Nombre], [Descripcion], [Siglas], [LogoUrl], [Estado]) VALUES ('Partido Alianza de los Trabajadores', 'Defensa de los derechos laborales y sindicales.', 'PAT', '/images/partidos/pat_logo.png', 1);

IF NOT EXISTS (SELECT 1 FROM [PartidosPoliticos] WHERE [Siglas] = 'PCN')
    INSERT INTO [PartidosPoliticos] ([Nombre], [Descripcion], [Siglas], [LogoUrl], [Estado]) VALUES ('Partido Conservador Nacional', 'Enfoque tradicional y valores patrios históricos.', 'PCN', '/images/partidos/pcn_logo.png', 0);
GO

-- 3. USUARIOS (Password: Password123* para todos, BCrypt Hash generado con BCrypt.Net-Next 4.2.0)
-- Hash generado: $2a$11$ADyrSpGJESzOEBvDcoto6uEnPU16ahzw2tQmSOEZN9edd7RpshaiK
IF NOT EXISTS (SELECT 1 FROM [Usuarios] WHERE [NombreUsuario] = 'admin_juan')
    INSERT INTO [Usuarios] ([Nombre], [Apellido], [CorreoElectronico], [NombreUsuario], [PasswordHash], [Rol], [Estado]) VALUES ('Juan', 'Administrador', 'juan.admin@evote360.com', 'admin_juan', '$2a$11$ADyrSpGJESzOEBvDcoto6uEnPU16ahzw2tQmSOEZN9edd7RpshaiK', 'Administrador', 1);

IF NOT EXISTS (SELECT 1 FROM [Usuarios] WHERE [NombreUsuario] = 'dirigente_pda')
    INSERT INTO [Usuarios] ([Nombre], [Apellido], [CorreoElectronico], [NombreUsuario], [PasswordHash], [Rol], [Estado]) VALUES ('Maria', 'Gomez', 'maria.dirigente@evote360.com', 'dirigente_pda', '$2a$11$ADyrSpGJESzOEBvDcoto6uEnPU16ahzw2tQmSOEZN9edd7RpshaiK', 'Dirigente político', 1);

IF NOT EXISTS (SELECT 1 FROM [Usuarios] WHERE [NombreUsuario] = 'dirigente_uld')
    INSERT INTO [Usuarios] ([Nombre], [Apellido], [CorreoElectronico], [NombreUsuario], [PasswordHash], [Rol], [Estado]) VALUES ('Carlos', 'Ramirez', 'carlos.dirigente@evote360.com', 'dirigente_uld', '$2a$11$ADyrSpGJESzOEBvDcoto6uEnPU16ahzw2tQmSOEZN9edd7RpshaiK', 'Dirigente político', 1);

IF NOT EXISTS (SELECT 1 FROM [Usuarios] WHERE [NombreUsuario] = 'dirigente_fcd')
    INSERT INTO [Usuarios] ([Nombre], [Apellido], [CorreoElectronico], [NombreUsuario], [PasswordHash], [Rol], [Estado]) VALUES ('Patricia', 'Sosa', 'patricia.dirigente@evote360.com', 'dirigente_fcd', '$2a$11$ADyrSpGJESzOEBvDcoto6uEnPU16ahzw2tQmSOEZN9edd7RpshaiK', 'Dirigente político', 1);

IF NOT EXISTS (SELECT 1 FROM [Usuarios] WHERE [NombreUsuario] = 'dirigente_inactivo')
    INSERT INTO [Usuarios] ([Nombre], [Apellido], [CorreoElectronico], [NombreUsuario], [PasswordHash], [Rol], [Estado]) VALUES ('Luis', 'Inactivo', 'luis.inactivo@evote360.com', 'dirigente_inactivo', '$2a$11$ADyrSpGJESzOEBvDcoto6uEnPU16ahzw2tQmSOEZN9edd7RpshaiK', 'Dirigente político', 0);
GO

-- 4. ASIGNACIONES DE DIRIGENTES (Insertar solo si existe el usuario/partido y no tiene asignacion activa)
DECLARE @DirPDA INT = (SELECT Id FROM [Usuarios] WHERE [NombreUsuario] = 'dirigente_pda');
DECLARE @DirULD INT = (SELECT Id FROM [Usuarios] WHERE [NombreUsuario] = 'dirigente_uld');
DECLARE @DirFCD INT = (SELECT Id FROM [Usuarios] WHERE [NombreUsuario] = 'dirigente_fcd');

DECLARE @PartPDA INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'PDA');
DECLARE @PartULD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'ULD');
DECLARE @PartFCD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'FCD');

IF @DirPDA IS NOT NULL AND @PartPDA IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [AsignacionesDirigentes] WHERE [UsuarioId] = @DirPDA)
    INSERT INTO [AsignacionesDirigentes] ([UsuarioId], [PartidoPoliticoId]) VALUES (@DirPDA, @PartPDA);

IF @DirULD IS NOT NULL AND @PartULD IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [AsignacionesDirigentes] WHERE [UsuarioId] = @DirULD)
    INSERT INTO [AsignacionesDirigentes] ([UsuarioId], [PartidoPoliticoId]) VALUES (@DirULD, @PartULD);

IF @DirFCD IS NOT NULL AND @PartFCD IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [AsignacionesDirigentes] WHERE [UsuarioId] = @DirFCD)
    INSERT INTO [AsignacionesDirigentes] ([UsuarioId], [PartidoPoliticoId]) VALUES (@DirFCD, @PartFCD);
GO

-- 5. CIUDADANOS (Insertar solo si no existe por DocumentoIdentidad)
IF NOT EXISTS (SELECT 1 FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100101')
    INSERT INTO [Ciudadanos] ([Nombre], [Apellido], [CorreoElectronico], [DocumentoIdentidad], [Estado], [YaVoto]) VALUES ('Pedro', 'Martinez', 'pedro.ciudadano@gmail.com', '10100100101', 1, 0);

IF NOT EXISTS (SELECT 1 FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100102')
    INSERT INTO [Ciudadanos] ([Nombre], [Apellido], [CorreoElectronico], [DocumentoIdentidad], [Estado], [YaVoto]) VALUES ('Ana', 'Reyes', 'ana.reyes@gmail.com', '10100100102', 1, 0);

IF NOT EXISTS (SELECT 1 FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100103')
    INSERT INTO [Ciudadanos] ([Nombre], [Apellido], [CorreoElectronico], [DocumentoIdentidad], [Estado], [YaVoto]) VALUES ('Jose', 'Santos', 'jose.santos@gmail.com', '10100100103', 1, 0);

IF NOT EXISTS (SELECT 1 FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100104')
    INSERT INTO [Ciudadanos] ([Nombre], [Apellido], [CorreoElectronico], [DocumentoIdentidad], [Estado], [YaVoto]) VALUES ('Lucia', 'Perez', 'lucia.perez@gmail.com', '10100100104', 1, 0);

IF NOT EXISTS (SELECT 1 FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100105')
    INSERT INTO [Ciudadanos] ([Nombre], [Apellido], [CorreoElectronico], [DocumentoIdentidad], [Estado], [YaVoto]) VALUES ('Julio', 'Ortega', 'julio.ortega@gmail.com', '10100100105', 0, 0);

IF NOT EXISTS (SELECT 1 FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100106')
    INSERT INTO [Ciudadanos] ([Nombre], [Apellido], [CorreoElectronico], [DocumentoIdentidad], [Estado], [YaVoto]) VALUES ('Carmen', 'Castillo', 'carmen.castillo@gmail.com', '10100100106', 1, 1);
GO

-- 6. CANDIDATOS (Insertar solo si no existe por DocumentoIdentidad)
DECLARE @PartPDA INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'PDA');
DECLARE @PartULD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'ULD');
DECLARE @PartFCD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'FCD');

DECLARE @PuestoPres INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Presidente');
DECLARE @PuestoSen INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Senador');

IF NOT EXISTS (SELECT 1 FROM [Candidatos] WHERE [DocumentoIdentidad] = '20200200201')
    INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) VALUES ('Alvaro', 'Guzman', '20200200201', 'alvaro.pda@evote360.com', '/images/candidatos/guzman.jpg', 1, @PartPDA, @PuestoPres);

IF NOT EXISTS (SELECT 1 FROM [Candidatos] WHERE [DocumentoIdentidad] = '20200200202')
    INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) VALUES ('Beatriz', 'Mejia', '20200200202', 'beatriz.uld@evote360.com', '/images/candidatos/mejia.jpg', 1, @PartULD, @PuestoPres);

IF NOT EXISTS (SELECT 1 FROM [Candidatos] WHERE [DocumentoIdentidad] = '20200200203')
    INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) VALUES ('Claudio', 'Nunes', '20200200203', 'claudio.fcd@evote360.com', '/images/candidatos/nunes.jpg', 1, @PartFCD, @PuestoPres);

IF NOT EXISTS (SELECT 1 FROM [Candidatos] WHERE [DocumentoIdentidad] = '20200200204')
    INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) VALUES ('Diana', 'Herrera', '20200200204', 'diana.pda@evote360.com', '/images/candidatos/herrera.jpg', 1, @PartPDA, @PuestoSen);

IF NOT EXISTS (SELECT 1 FROM [Candidatos] WHERE [DocumentoIdentidad] = '20200200205')
    INSERT INTO [Candidatos] ([Nombre], [Apellido], [DocumentoIdentidad], [CorreoElectronico], [Foto], [EsActivo], [PartidoPoliticoId], [PuestoElectivoId]) VALUES ('Eduardo', 'Vargas', '20200200205', 'eduardo.uld@evote360.com', '/images/candidatos/vargas.jpg', 1, @PartULD, @PuestoSen);
GO

-- 7. CANDIDATOS PUESTOS (Insertar solo si existe relacion y no está mapeada)
DECLARE @CandAlvaro INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Alvaro');
DECLARE @CandBeatriz INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Beatriz');
DECLARE @CandClaudio INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Claudio');
DECLARE @CandDiana INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Diana');
DECLARE @CandEduardo INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Eduardo');

DECLARE @PartPDA INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'PDA');
DECLARE @PartULD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'ULD');
DECLARE @PartFCD INT = (SELECT Id FROM [PartidosPoliticos] WHERE [Siglas] = 'FCD');

DECLARE @PuestoPres INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Presidente');
DECLARE @PuestoSen INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Senador');

IF @CandAlvaro IS NOT NULL AND @PuestoPres IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [CandidatosPuestos] WHERE [CandidatoId] = @CandAlvaro AND [PuestoElectivoId] = @PuestoPres)
    INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandAlvaro, @PuestoPres, @PartPDA, 0);

IF @CandBeatriz IS NOT NULL AND @PuestoPres IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [CandidatosPuestos] WHERE [CandidatoId] = @CandBeatriz AND [PuestoElectivoId] = @PuestoPres)
    INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandBeatriz, @PuestoPres, @PartULD, 0);

IF @CandClaudio IS NOT NULL AND @PuestoPres IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [CandidatosPuestos] WHERE [CandidatoId] = @CandClaudio AND [PuestoElectivoId] = @PuestoPres)
    INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandClaudio, @PuestoPres, @PartFCD, 0);

IF @CandDiana IS NOT NULL AND @PuestoSen IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [CandidatosPuestos] WHERE [CandidatoId] = @CandDiana AND [PuestoElectivoId] = @PuestoSen)
    INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandDiana, @PuestoSen, @PartPDA, 0);

IF @CandEduardo IS NOT NULL AND @PuestoSen IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [CandidatosPuestos] WHERE [CandidatoId] = @CandEduardo AND [PuestoElectivoId] = @PuestoSen)
    INSERT INTO [CandidatosPuestos] ([CandidatoId], [PuestoElectivoId], [PartidoPoliticoId], [EsAliado]) VALUES (@CandEduardo, @PuestoSen, @PartULD, 0);
GO

-- 8. ELECCIONES (Insertar solo si no existe por Nombre)
IF NOT EXISTS (SELECT 1 FROM [Elecciones] WHERE [Nombre] = 'Elecciones Generales Ordinarias 2024')
    INSERT INTO [Elecciones] ([Nombre], [FechaRealizacion], [Estado]) VALUES ('Elecciones Generales Ordinarias 2024', '2024-05-19 00:00:00', 3);

IF NOT EXISTS (SELECT 1 FROM [Elecciones] WHERE [Nombre] = 'Elecciones Municipales Extraordinarias 2024')
    INSERT INTO [Elecciones] ([Nombre], [FechaRealizacion], [Estado]) VALUES ('Elecciones Municipales Extraordinarias 2024', '2024-02-18 00:00:00', 3);

IF NOT EXISTS (SELECT 1 FROM [Elecciones] WHERE [Nombre] = 'Elecciones Congresionales 2026')
    INSERT INTO [Elecciones] ([Nombre], [FechaRealizacion], [Estado]) VALUES ('Elecciones Congresionales 2026', '2026-05-16 00:00:00', 1);

IF NOT EXISTS (SELECT 1 FROM [Elecciones] WHERE [Nombre] = 'Elecciones Presidenciales Primarias 2026')
    INSERT INTO [Elecciones] ([Nombre], [FechaRealizacion], [Estado]) VALUES ('Elecciones Presidenciales Primarias 2026', '2026-06-20 00:00:00', 2);
GO

-- 9. VOTOS (Insertar solo si existe la eleccion y no se ha registrado el voto)
DECLARE @Elec1 INT = (SELECT Id FROM [Elecciones] WHERE [Nombre] = 'Elecciones Generales Ordinarias 2024');
DECLARE @PuestoPres INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Presidente');
DECLARE @PuestoSen INT = (SELECT Id FROM [PuestosElectivos] WHERE [Nombre] = 'Senador');

DECLARE @CandAlvaro INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Alvaro');
DECLARE @CandBeatriz INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Beatriz');
DECLARE @CandClaudio INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Claudio');
DECLARE @CandDiana INT = (SELECT Id FROM [Candidatos] WHERE [Nombre] = 'Diana');

-- Obtener IDs reales de los ciudadanos creados para evitar inconsistencias
DECLARE @Ciu1 INT = (SELECT Id FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100101');
DECLARE @Ciu2 INT = (SELECT Id FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100102');
DECLARE @Ciu3 INT = (SELECT Id FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100103');
DECLARE @Ciu4 INT = (SELECT Id FROM [Ciudadanos] WHERE [DocumentoIdentidad] = '10100100104');

IF @Elec1 IS NOT NULL AND @PuestoPres IS NOT NULL AND @CandAlvaro IS NOT NULL AND @Ciu1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Votos] WHERE [CiudadanoId] = @Ciu1 AND [EleccionId] = @Elec1 AND [PuestoElectivoId] = @PuestoPres)
    INSERT INTO [Votos] ([CiudadanoId], [EleccionId], [PuestoElectivoId], [CandidatoId], [FechaVoto]) VALUES (@Ciu1, @Elec1, @PuestoPres, @CandAlvaro, GETDATE());

IF @Elec1 IS NOT NULL AND @PuestoSen IS NOT NULL AND @CandDiana IS NOT NULL AND @Ciu1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Votos] WHERE [CiudadanoId] = @Ciu1 AND [EleccionId] = @Elec1 AND [PuestoElectivoId] = @PuestoSen)
    INSERT INTO [Votos] ([CiudadanoId], [EleccionId], [PuestoElectivoId], [CandidatoId], [FechaVoto]) VALUES (@Ciu1, @Elec1, @PuestoSen, @CandDiana, GETDATE());

IF @Elec1 IS NOT NULL AND @PuestoPres IS NOT NULL AND @CandBeatriz IS NOT NULL AND @Ciu2 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Votos] WHERE [CiudadanoId] = @Ciu2 AND [EleccionId] = @Elec1 AND [PuestoElectivoId] = @PuestoPres)
    INSERT INTO [Votos] ([CiudadanoId], [EleccionId], [PuestoElectivoId], [CandidatoId], [FechaVoto]) VALUES (@Ciu2, @Elec1, @PuestoPres, @CandBeatriz, GETDATE());

IF @Elec1 IS NOT NULL AND @PuestoPres IS NOT NULL AND @CandClaudio IS NOT NULL AND @Ciu3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Votos] WHERE [CiudadanoId] = @Ciu3 AND [EleccionId] = @Elec1 AND [PuestoElectivoId] = @PuestoPres)
    INSERT INTO [Votos] ([CiudadanoId], [EleccionId], [PuestoElectivoId], [CandidatoId], [FechaVoto]) VALUES (@Ciu3, @Elec1, @PuestoPres, @CandClaudio, GETDATE());

IF @Elec1 IS NOT NULL AND @PuestoSen IS NOT NULL AND @Ciu3 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Votos] WHERE [CiudadanoId] = @Ciu3 AND [EleccionId] = @Elec1 AND [PuestoElectivoId] = @PuestoSen)
    INSERT INTO [Votos] ([CiudadanoId], [EleccionId], [PuestoElectivoId], [CandidatoId], [FechaVoto]) VALUES (@Ciu3, @Elec1, @PuestoSen, NULL, GETDATE());

IF @Elec1 IS NOT NULL AND @PuestoPres IS NOT NULL AND @CandAlvaro IS NOT NULL AND @Ciu4 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Votos] WHERE [CiudadanoId] = @Ciu4 AND [EleccionId] = @Elec1 AND [PuestoElectivoId] = @PuestoPres)
    INSERT INTO [Votos] ([CiudadanoId], [EleccionId], [PuestoElectivoId], [CandidatoId], [FechaVoto]) VALUES (@Ciu4, @Elec1, @PuestoPres, @CandAlvaro, GETDATE());
GO
