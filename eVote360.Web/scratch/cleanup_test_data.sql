-- ===================================================================
-- SQL SCRIPT: Limpieza de datos de prueba para eVote360 Pro
-- Proposito: Eliminar en orden de dependencias de clave foránea
-- todos los registros simulados/insertados para la auditoría.
-- ===================================================================

USE [eVote360Db];
GO

-- 1. Borrar Votos de los ciudadanos de prueba
DELETE FROM [Votos]
WHERE [CiudadanoId] IN (
    SELECT [Id] FROM [Ciudadanos] 
    WHERE [DocumentoIdentidad] IN ('10100100101', '10100100102', '10100100103', '10100100104', '10100100105', '10100100106')
);

-- 2. Borrar CandidatosPuestos de los candidatos de prueba
DELETE FROM [CandidatosPuestos]
WHERE [CandidatoId] IN (
    SELECT [Id] FROM [Candidatos] 
    WHERE [DocumentoIdentidad] IN ('20200200201', '20200200202', '20200200203', '20200200204', '20200200205')
);

-- 3. Borrar Candidatos de prueba
DELETE FROM [Candidatos]
WHERE [DocumentoIdentidad] IN ('20200200201', '20200200202', '20200200203', '20200200204', '20200200205');

-- 4. Borrar AsignacionesDirigentes de los usuarios de prueba
DELETE FROM [AsignacionesDirigentes]
WHERE [UsuarioId] IN (
    SELECT [Id] FROM [Usuarios] 
    WHERE [NombreUsuario] IN ('admin_juan', 'dirigente_pda', 'dirigente_uld', 'dirigente_fcd', 'dirigente_inactivo')
);

-- 5. Borrar Usuarios de prueba
DELETE FROM [Usuarios]
WHERE [NombreUsuario] IN ('admin_juan', 'dirigente_pda', 'dirigente_uld', 'dirigente_fcd', 'dirigente_inactivo');

-- 6. Borrar Ciudadanos de prueba
DELETE FROM [Ciudadanos]
WHERE [DocumentoIdentidad] IN ('10100100101', '10100100102', '10100100103', '10100100104', '10100100105', '10100100106');

-- 7. Borrar Elecciones de prueba
DELETE FROM [Elecciones]
WHERE [Nombre] IN (
    'Elecciones Generales Ordinarias 2024', 
    'Elecciones Municipales Extraordinarias 2024', 
    'Elecciones Congresionales 2026', 
    'Elecciones Presidenciales Primarias 2026'
);

-- 8. Borrar Partidos Políticos de prueba
DELETE FROM [PartidosPoliticos]
WHERE [Siglas] IN ('PDA', 'ULD', 'FCD', 'PAT', 'PCN');

-- 9. Borrar Puestos Electivos de prueba
DELETE FROM [PuestosElectivos]
WHERE [Nombre] IN ('Presidente', 'Senador', 'Diputado', 'Alcalde', 'Regidor');
GO
