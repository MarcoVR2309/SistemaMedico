-- ================================================================================
-- STORED PROCEDURES CRUD - ROLES
-- Base de Datos: RedCLinicas
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando Stored Procedures CRUD para Roles...';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- ROLES - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR ROL
CREATE OR ALTER PROCEDURE sp_InsertarRol
    @Nombre VARCHAR(50),
    @Descripcion VARCHAR(200) = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Verificar si el rol ya existe
        IF EXISTS (SELECT 1 FROM Roles WHERE NOM_ROL = @Nombre)
        BEGIN
            RAISERROR('Ya existe un rol con ese nombre', 16, 1);
            RETURN;
        END
        
        EXEC GenerarIdRol @IdGenerado OUTPUT;
        
        INSERT INTO Roles (ID, NOM_ROL, DESCRIP)
        VALUES (@IdGenerado, @Nombre, @Descripcion);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: OBTENER ROL POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerRolPorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ID,
        NOM_ROL,
        DESCRIP
    FROM Roles
    WHERE ID = @Id;
END
GO

-- SP: OBTENER ROL POR NOMBRE
CREATE OR ALTER PROCEDURE sp_ObtenerRolPorNombre
    @Nombre VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ID,
        NOM_ROL,
        DESCRIP
    FROM Roles
    WHERE NOM_ROL = @Nombre;
END
GO

-- SP: LISTAR ROLES
CREATE OR ALTER PROCEDURE sp_ListarRoles
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ID,
        NOM_ROL,
        DESCRIP
    FROM Roles
    ORDER BY NOM_ROL;
END
GO

-- SP: ACTUALIZAR ROL
CREATE OR ALTER PROCEDURE sp_ActualizarRol
    @Id VARCHAR(20),
    @Nombre VARCHAR(50),
    @Descripcion VARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Verificar si otro rol ya tiene ese nombre
        IF EXISTS (SELECT 1 FROM Roles WHERE NOM_ROL = @Nombre AND ID <> @Id)
        BEGIN
            RAISERROR('Ya existe otro rol con ese nombre', 16, 1);
            RETURN;
        END
        
        UPDATE Roles
        SET NOM_ROL = @Nombre,
            DESCRIP = @Descripcion
        WHERE ID = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- SP: ELIMINAR ROL (Solo si no tiene usuarios asignados)
CREATE OR ALTER PROCEDURE sp_EliminarRol
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Verificar si hay usuarios con este rol
        IF EXISTS (SELECT 1 FROM Usuarios WHERE ID_ROL = @Id)
        BEGIN
            RAISERROR('No se puede eliminar el rol porque tiene usuarios asignados', 16, 1);
            RETURN;
        END
        
        DELETE FROM Roles WHERE ID = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- SP: CONTAR USUARIOS POR ROL
CREATE OR ALTER PROCEDURE sp_ContarUsuariosPorRol
    @IdRol VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        r.ID,
        r.NOM_ROL,
        COUNT(u.ID) AS TotalUsuarios
    FROM Roles r
    LEFT JOIN Usuarios u ON r.ID = u.ID_ROL AND u.Activo = 1
    WHERE r.ID = @IdRol
    GROUP BY r.ID, r.NOM_ROL;
END
GO

-- SP: OBTENER ESTADISTICAS DE ROLES
CREATE OR ALTER PROCEDURE sp_ObtenerEstadisticasRoles
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        r.ID,
        r.NOM_ROL,
        r.DESCRIP,
        COUNT(u.ID) AS TotalUsuarios,
        COUNT(CASE WHEN u.Activo = 1 THEN 1 END) AS UsuariosActivos,
        COUNT(CASE WHEN u.Activo = 0 THEN 1 END) AS UsuariosInactivos
    FROM Roles r
    LEFT JOIN Usuarios u ON r.ID = u.ID_ROL
    GROUP BY r.ID, r.NOM_ROL, r.DESCRIP
    ORDER BY r.NOM_ROL;
END
GO

PRINT '  ? SPs CRUD Roles creados';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- RESUMEN
-- ????????????????????????????????????????????????????????????????????????????

PRINT '';
PRINT '??????????????????????????????????????????????????????????????????????????';
PRINT '?              STORED PROCEDURES CRUD ROLES CREADOS                      ?';
PRINT '??????????????????????????????????????????????????????????????????????????';
PRINT '';
PRINT 'ROLES:';
PRINT '  ? sp_InsertarRol';
PRINT '  ? sp_ObtenerRolPorId';
PRINT '  ? sp_ObtenerRolPorNombre';
PRINT '  ? sp_ListarRoles';
PRINT '  ? sp_ActualizarRol';
PRINT '  ? sp_EliminarRol';
PRINT '  ? sp_ContarUsuariosPorRol';
PRINT '  ? sp_ObtenerEstadisticasRoles';
PRINT '';
GO
