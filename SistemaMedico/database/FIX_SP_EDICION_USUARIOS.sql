-- ================================================================================
-- FIX: STORED PROCEDURES PARA EDICIÓN DE USUARIOS
-- Solución a errores: NUM_COLEGIATURA y RegistrarAccionAdmin
-- ================================================================================

USE RedCLinicas;
GO

PRINT '?? Corrigiendo Stored Procedures para Edición de Usuarios...';
GO

-- ============================================================================
-- FIX 1: CORREGIR SP_OBTENERDOCTORPORIDUSUARIO
-- Problema: No retornaba NUM_COLEGIATURA
-- ============================================================================
PRINT '1?? Corrigiendo sp_ObtenerDoctorPorIdUsuario...';
GO

CREATE OR ALTER PROCEDURE sp_ObtenerDoctorPorIdUsuario
    @IdUsuario VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        d.ID,
        d.ID_USU,
        d.ID_ESP,
        d.NUM_COLEGIATURA,     -- ? CAMPO AGREGADO
        d.ANIOS_EXP,            -- ? CAMPO AGREGADO
        u.NOM,
        u.APE,
        u.EMAIL,
        u.TEL,
        e.NOM_ESP AS NombreEspecialidad
    FROM Doctores d
    INNER JOIN Usuarios u ON d.ID_USU = u.ID
    LEFT JOIN Especialidades e ON d.ID_ESP = e.ID
    WHERE d.ID_USU = @IdUsuario;
END
GO

PRINT '   ? sp_ObtenerDoctorPorIdUsuario corregido';
GO

-- ============================================================================
-- FIX 2: RENOMBRAR SP REGISTRARACCIONADMIN
-- Problema: El código buscaba RegistrarAccionAdmin pero el SP se llama sp_RegistrarAccionAdmin
-- ============================================================================
PRINT '2?? Verificando/Creando sp_RegistrarAccionAdmin...';
GO

-- Verificar si existe el SP sin el prefijo sp_
IF OBJECT_ID('dbo.RegistrarAccionAdmin', 'P') IS NOT NULL
BEGIN
    PRINT '   ? Encontrado SP sin prefijo sp_, eliminando...';
    DROP PROCEDURE dbo.RegistrarAccionAdmin;
END
GO

-- Crear/Actualizar el SP con el nombre correcto
CREATE OR ALTER PROCEDURE sp_RegistrarAccionAdmin
    @IdAdmin VARCHAR(20),
    @TipoAccion VARCHAR(50),
    @Descripcion VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE Administradores
        SET ULTIMA_ACCION = GETDATE(),
            ACCIONES_REALIZADAS = ACCIONES_REALIZADAS + 1
        WHERE ID = @IdAdmin;
        
        -- Si no se encontró el administrador, no falla pero registra el warning
        IF @@ROWCOUNT = 0
        BEGIN
            PRINT 'WARNING: No se encontró el administrador con ID: ' + @IdAdmin;
        END
    END TRY
    BEGIN CATCH
        -- Registrar el error pero no lanzar excepción para no afectar el flujo principal
        PRINT 'ERROR en sp_RegistrarAccionAdmin: ' + ERROR_MESSAGE();
    END CATCH
END
GO

PRINT '   ? sp_RegistrarAccionAdmin creado/actualizado';
GO

-- ============================================================================
-- FIX 3: VERIFICAR ESTRUCTURA DE TABLA DOCTORES
-- ============================================================================
PRINT '3?? Verificando estructura de tabla Doctores...';
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Doctores') AND name = 'NUM_COLEGIATURA')
BEGIN
    PRINT '   ? ERROR: La columna NUM_COLEGIATURA no existe en la tabla Doctores';
    PRINT '   ?? Por favor ejecuta el script de creación de tablas primero';
END
ELSE
BEGIN
    PRINT '   ? Columna NUM_COLEGIATURA existe correctamente';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Doctores') AND name = 'ANIOS_EXP')
BEGIN
    PRINT '   ? ERROR: La columna ANIOS_EXP no existe en la tabla Doctores';
    PRINT '   ?? Por favor ejecuta el script de creación de tablas primero';
END
ELSE
BEGIN
    PRINT '   ? Columna ANIOS_EXP existe correctamente';
END
GO

-- ============================================================================
-- FIX 4: VERIFICAR OTROS SPS RELACIONADOS
-- ============================================================================
PRINT '4?? Verificando otros Stored Procedures relacionados...';
GO

-- Verificar sp_ObtenerDoctorPorId
IF OBJECT_ID('dbo.sp_ObtenerDoctorPorId', 'P') IS NULL
BEGIN
    PRINT '   ? sp_ObtenerDoctorPorId no existe, creando...';
    
    CREATE PROCEDURE sp_ObtenerDoctorPorId
        @Id VARCHAR(20)
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT 
            d.ID,
            d.ID_USU,
            d.ID_ESP,
            d.NUM_COLEGIATURA,
            d.ANIOS_EXP,
            u.NOM,
            u.APE,
            u.EMAIL,
            u.TEL,
            e.NOM_ESP AS NombreEspecialidad
        FROM Doctores d
        INNER JOIN Usuarios u ON d.ID_USU = u.ID
        LEFT JOIN Especialidades e ON d.ID_ESP = e.ID
        WHERE d.ID = @Id;
    END
END
ELSE
BEGIN
    PRINT '   ? sp_ObtenerDoctorPorId existe';
END
GO

-- ============================================================================
-- RESUMEN
-- ============================================================================
PRINT '';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '? CORRECCIONES APLICADAS EXITOSAMENTE';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '';
PRINT 'Cambios realizados:';
PRINT '  ? sp_ObtenerDoctorPorIdUsuario - Retorna NUM_COLEGIATURA y ANIOS_EXP';
PRINT '  ? sp_RegistrarAccionAdmin - Nombre correcto con prefijo sp_';
PRINT '  ? Validación de estructura de tabla Doctores';
PRINT '';
PRINT '?? Ahora puedes probar la funcionalidad de Editar Usuario';
PRINT '';
GO
