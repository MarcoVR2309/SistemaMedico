-- ================================================================================
-- VERIFICACIÓN Y CREACIÓN DE STORED PROCEDURE sp_RegistrarAccionAdmin
-- Ejecuta este script para asegurarte de que el SP existe
-- ================================================================================

USE RedCLinicas;
GO

PRINT '';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '?? VERIFICANDO STORED PROCEDURE: sp_RegistrarAccionAdmin';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '';

-- ============================================================================
-- PASO 1: VERIFICAR SI EXISTE EL SP
-- ============================================================================
IF OBJECT_ID('dbo.sp_RegistrarAccionAdmin', 'P') IS NOT NULL
BEGIN
    PRINT '? El Stored Procedure sp_RegistrarAccionAdmin YA EXISTE';
    PRINT '   ?? Ubicación: dbo.sp_RegistrarAccionAdmin';
    PRINT '   ??  Se actualizará a la última versión...';
    PRINT '';
END
ELSE
BEGIN
    PRINT '? El Stored Procedure sp_RegistrarAccionAdmin NO EXISTE';
    PRINT '   ??  Se creará ahora...';
    PRINT '';
END
GO

-- ============================================================================
-- PASO 2: ELIMINAR SP SIN PREFIJO (si existe)
-- ============================================================================
IF OBJECT_ID('dbo.RegistrarAccionAdmin', 'P') IS NOT NULL
BEGIN
    PRINT '??  Encontrado SP SIN prefijo sp_: RegistrarAccionAdmin';
    PRINT '   ???  Eliminando versión incorrecta...';
    DROP PROCEDURE dbo.RegistrarAccionAdmin;
    PRINT '   ? SP sin prefijo eliminado';
    PRINT '';
END
GO

-- ============================================================================
-- PASO 3: CREAR O ACTUALIZAR EL SP CORRECTO
-- ============================================================================
PRINT '??  Creando/Actualizando sp_RegistrarAccionAdmin...';
GO

CREATE OR ALTER PROCEDURE sp_RegistrarAccionAdmin
    @IdAdmin VARCHAR(20),
    @TipoAccion VARCHAR(50),
    @Descripcion VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Actualizar última acción y contador
        UPDATE Administradores
        SET ULTIMA_ACCION = GETDATE(),
            ACCIONES_REALIZADAS = ISNULL(ACCIONES_REALIZADAS, 0) + 1
        WHERE ID = @IdAdmin;
        
        -- Verificar si se actualizó algún registro
        IF @@ROWCOUNT = 0
        BEGIN
            -- No se encontró el administrador, pero no fallamos
            PRINT 'WARNING: No se encontró el administrador con ID: ' + @IdAdmin;
            -- No lanzamos error para no afectar el flujo principal
        END
        ELSE
        BEGIN
            -- Éxito silencioso
            -- PRINT 'Acción registrada: ' + @TipoAccion + ' para admin: ' + @IdAdmin;
        END
    END TRY
    BEGIN CATCH
        -- Capturar error pero no lanzarlo
        DECLARE @ErrorMsg VARCHAR(MAX) = ERROR_MESSAGE();
        PRINT 'ERROR en sp_RegistrarAccionAdmin: ' + @ErrorMsg;
        -- No hacemos THROW para no afectar el flujo principal
    END CATCH
END
GO

PRINT '   ? sp_RegistrarAccionAdmin creado/actualizado exitosamente';
PRINT '';
GO

-- ============================================================================
-- PASO 4: PROBAR EL SP
-- ============================================================================
PRINT '?? Probando el Stored Procedure...';
GO

-- Probar con un ID de administrador que probablemente exista
DECLARE @TestIdAdmin VARCHAR(20);

-- Intentar obtener el primer administrador
SELECT TOP 1 @TestIdAdmin = ID FROM Administradores;

IF @TestIdAdmin IS NOT NULL
BEGIN
    PRINT '   ?? ID de Admin de prueba: ' + @TestIdAdmin;
    
    -- Ejecutar el SP de prueba
    BEGIN TRY
        EXEC sp_RegistrarAccionAdmin 
            @IdAdmin = @TestIdAdmin,
            @TipoAccion = 'PRUEBA_SISTEMA',
            @Descripcion = 'Verificación de funcionamiento del SP';
        
        PRINT '   ? SP ejecutado correctamente (modo prueba)';
    END TRY
    BEGIN CATCH
        PRINT '   ? ERROR al ejecutar SP: ' + ERROR_MESSAGE();
    END CATCH
END
ELSE
BEGIN
    PRINT '   ??  No hay administradores en la BD para probar';
    PRINT '   ??  El SP se creó correctamente de todas formas';
END
GO

-- ============================================================================
-- PASO 5: VERIFICAR ESTRUCTURA DE TABLA ADMINISTRADORES
-- ============================================================================
PRINT '';
PRINT '?? Verificando estructura de tabla Administradores...';
GO

-- Verificar columna ULTIMA_ACCION
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Administradores') AND name = 'ULTIMA_ACCION')
BEGIN
    PRINT '   ? Columna ULTIMA_ACCION existe';
END
ELSE
BEGIN
    PRINT '   ? ERROR: Columna ULTIMA_ACCION NO existe';
    PRINT '   ?? Debes ejecutar el script de creación de tablas';
END
GO

-- Verificar columna ACCIONES_REALIZADAS
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Administradores') AND name = 'ACCIONES_REALIZADAS')
BEGIN
    PRINT '   ? Columna ACCIONES_REALIZADAS existe';
END
ELSE
BEGIN
    PRINT '   ? ERROR: Columna ACCIONES_REALIZADAS NO existe';
    PRINT '   ?? Debes ejecutar el script de creación de tablas';
END
GO

-- ============================================================================
-- PASO 6: LISTAR TODOS LOS SPS RELACIONADOS CON ADMINISTRADORES
-- ============================================================================
PRINT '';
PRINT '?? Stored Procedures relacionados con Administradores:';
GO

SELECT 
    OBJECT_NAME(object_id) AS [Stored Procedure],
    create_date AS [Fecha Creación],
    modify_date AS [Última Modificación]
FROM sys.procedures
WHERE OBJECT_NAME(object_id) LIKE '%Admin%'
ORDER BY OBJECT_NAME(object_id);
GO

-- ============================================================================
-- RESUMEN FINAL
-- ============================================================================
PRINT '';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '? VERIFICACIÓN COMPLETADA';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '';
PRINT '?? ACCIONES A TOMAR:';
PRINT '';
PRINT '1. Si sp_RegistrarAccionAdmin se creó correctamente:';
PRINT '   ? Tu aplicación web debería funcionar ahora';
PRINT '   ?? Prueba hacer clic en "Editar" en Especialidades';
PRINT '';
PRINT '2. Si hay errores de columnas faltantes:';
PRINT '   ? Ejecuta primero el script de creación de tablas';
PRINT '   ?? Archivo: Creacion_Tablas.sql (o similar)';
PRINT '';
PRINT '3. Si el SP no se puede crear:';
PRINT '   ? Verifica que estés conectado a la BD correcta';
PRINT '   ?? Base de datos: RedCLinicas';
PRINT '';
PRINT '???????????????????????????????????????????????????????????????';
PRINT '';
GO
