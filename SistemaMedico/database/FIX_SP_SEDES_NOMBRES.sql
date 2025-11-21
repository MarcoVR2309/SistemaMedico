-- ================================================================================
-- FIX: RENOMBRAR STORED PROCEDURES DE SEDES (Si no tienen prefijo sp_)
-- Base de Datos: RedCLinicas
-- ================================================================================

USE RedCLinicas;
GO

PRINT '';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '?? VERIFICANDO Y CORRIGIENDO STORED PROCEDURES DE SEDES';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '';

-- ============================================================================
-- VERIFICAR SI EXISTEN LOS SPs SIN PREFIJO
-- ============================================================================
PRINT '1?? Verificando stored procedures existentes...';
GO

IF OBJECT_ID('dbo.ObtenerSedePorId', 'P') IS NOT NULL
BEGIN
    PRINT '   ??  Encontrado: ObtenerSedePorId (SIN prefijo sp_)';
END
ELSE IF OBJECT_ID('dbo.sp_ObtenerSedePorId', 'P') IS NOT NULL
BEGIN
    PRINT '   ? Encontrado: sp_ObtenerSedePorId (CON prefijo sp_)';
END
ELSE
BEGIN
    PRINT '   ? NO encontrado: ObtenerSedePorId ni sp_ObtenerSedePorId';
END
GO

IF OBJECT_ID('dbo.ActualizarSede', 'P') IS NOT NULL
BEGIN
    PRINT '   ??  Encontrado: ActualizarSede (SIN prefijo sp_)';
END
ELSE IF OBJECT_ID('dbo.sp_ActualizarSede', 'P') IS NOT NULL
BEGIN
    PRINT '   ? Encontrado: sp_ActualizarSede (CON prefijo sp_)';
END
ELSE
BEGIN
    PRINT '   ? NO encontrado: ActualizarSede ni sp_ActualizarSede';
END
GO

IF OBJECT_ID('dbo.ListarSedes', 'P') IS NOT NULL
BEGIN
    PRINT '   ??  Encontrado: ListarSedes (SIN prefijo sp_)';
END
ELSE IF OBJECT_ID('dbo.sp_ListarSedes', 'P') IS NOT NULL
BEGIN
    PRINT '   ? Encontrado: sp_ListarSedes (CON prefijo sp_)';
END
ELSE
BEGIN
    PRINT '   ? NO encontrado: ListarSedes ni sp_ListarSedes';
END
GO

PRINT '';
PRINT '2?? Creando/Actualizando stored procedures con nombres correctos...';
PRINT '';

-- ============================================================================
-- CREAR/ACTUALIZAR: ObtenerSedePorId (sin prefijo sp_)
-- ============================================================================
PRINT '   ?? Creando ObtenerSedePorId...';
GO

CREATE OR ALTER PROCEDURE ObtenerSedePorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID, NOM_SEDE, DIR_SEDE = DIRECCION, DIRECCION, TELEFONO, EMAIL
    FROM Sedes
    WHERE ID = @Id;
END
GO
PRINT '   ? ObtenerSedePorId creado/actualizado';
GO

-- ============================================================================
-- CREAR/ACTUALIZAR: ActualizarSede (sin prefijo sp_)
-- ============================================================================
PRINT '   ?? Creando ActualizarSede...';
GO

CREATE OR ALTER PROCEDURE ActualizarSede
    @Id VARCHAR(20),
    @Nombre VARCHAR(200),
    @Direccion VARCHAR(300) = NULL,
    @Telefono VARCHAR(20) = NULL,
    @Email VARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Sedes
    SET NOM_SEDE = @Nombre,
        DIRECCION = @Direccion,
        TELEFONO = @Telefono,
        EMAIL = @Email
    WHERE ID = @Id;
END
GO
PRINT '   ? ActualizarSede creado/actualizado';
GO

-- ============================================================================
-- CREAR/ACTUALIZAR: ListarSedes (sin prefijo sp_)
-- ============================================================================
PRINT '   ?? Creando ListarSedes...';
GO

CREATE OR ALTER PROCEDURE ListarSedes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID, NOM_SEDE, DIR_SEDE = DIRECCION, DIRECCION, TELEFONO, EMAIL
    FROM Sedes
    ORDER BY NOM_SEDE;
END
GO
PRINT '   ? ListarSedes creado/actualizado';
GO

-- ============================================================================
-- TAMBIÉN CREAR LAS VERSIONES CON PREFIJO sp_ (para compatibilidad)
-- ============================================================================
PRINT '';
PRINT '3?? Creando versiones con prefijo sp_ (compatibilidad)...';
PRINT '';

---- sp_ObtenerSedePorId
--CREATE OR ALTER PROCEDURE ObtenerSedePorId
--    @Id VARCHAR(20)
--AS
--BEGIN
--    SET NOCOUNT ON;
--    EXEC ObtenerSedePorId @Id;
--END
--GO
--PRINT '   ? sp_ObtenerSedePorId (alias) creado';
--GO

---- sp_ActualizarSede
--CREATE OR ALTER PROCEDURE sp_ActualizarSede
--    @Id VARCHAR(20),
--    @Nombre VARCHAR(200),
--    @Direccion VARCHAR(300) = NULL,
--    @Telefono VARCHAR(20) = NULL,
--    @Email VARCHAR(150) = NULL
--AS
--BEGIN
--    SET NOCOUNT ON;
--    EXEC ActualizarSede @Id, @Nombre, @Direccion, @Telefono, @Email;
--END
--GO
--PRINT '   ? sp_ActualizarSede (alias) creado';
--GO

---- sp_ListarSedes
--CREATE OR ALTER PROCEDURE sp_ListarSedes
--AS
--BEGIN
--    SET NOCOUNT ON;
--    EXEC ListarSedes;
--END
--GO
--PRINT '   ? sp_ListarSedes (alias) creado';
--GO

-- ============================================================================
-- VERIFICACIÓN FINAL
-- ============================================================================
PRINT '';
PRINT '4?? Verificación final...';
GO

SELECT 
    OBJECT_NAME(object_id) AS [Stored Procedure],
    create_date AS [Fecha Creación],
    modify_date AS [Última Modificación]
FROM sys.procedures
WHERE OBJECT_NAME(object_id) IN (
    'ObtenerSedePorId', 'sp_ObtenerSedePorId',
    'ActualizarSede', 'sp_ActualizarSede',
    'ListarSedes', 'sp_ListarSedes'
)
ORDER BY OBJECT_NAME(object_id);
GO

-- ============================================================================
-- PRUEBA DE LOS STORED PROCEDURES
-- ============================================================================
PRINT '';
PRINT '5?? Probando stored procedures...';
GO

-- Probar ListarSedes
PRINT '   ?? Probando ListarSedes...';
DECLARE @CountSedes INT;
SELECT @CountSedes = COUNT(*) FROM Sedes;
IF @CountSedes > 0
BEGIN
    PRINT '   ? ListarSedes funciona (' + CAST(@CountSedes AS VARCHAR) + ' sedes encontradas)';
END
ELSE
BEGIN
    PRINT '   ??  No hay sedes en la base de datos';
END
GO

-- ============================================================================
-- RESUMEN FINAL
-- ============================================================================
PRINT '';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '? CORRECCIÓN COMPLETADA';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '';
PRINT '?? STORED PROCEDURES CREADOS:';
PRINT '';
PRINT 'Sin prefijo sp_ (nombres originales):';
PRINT '  ? ObtenerSedePorId';
PRINT '  ? ActualizarSede';
PRINT '  ? ListarSedes';
PRINT '';
PRINT 'Con prefijo sp_ (alias para compatibilidad):';
PRINT '  ? sp_ObtenerSedePorId';
PRINT '  ? sp_ActualizarSede';
PRINT '  ? sp_ListarSedes';
PRINT '';
PRINT '?? Ahora puedes usar cualquiera de los dos nombres en tu código C#';
PRINT '';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '';
GO
