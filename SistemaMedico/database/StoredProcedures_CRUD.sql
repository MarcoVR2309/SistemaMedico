-- ================================================================================
-- STORED PROCEDURES CRUD - USUARIOS Y PACIENTES
-- Base de Datos: RedCLinicas
-- Usa los SPs generadores ya existentes (GenerarIdUsuario, GenerarIdPaciente)
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando Stored Procedures CRUD...';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- USUARIOS - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR USUARIO
CREATE OR ALTER PROCEDURE sp_InsertarUsuario
    @IdRol VARCHAR(20),
    @TipoDoc VARCHAR(10) = NULL,
    @NumDoc VARCHAR(20) = NULL,
    @Apellido VARCHAR(100),
    @Nombre VARCHAR(100),
    @Genero CHAR(1),
    @FechaNac DATE = NULL,
    @Email VARCHAR(150),
    @Telefono VARCHAR(20) = NULL,
    @PasswordHash VARBINARY(MAX),
    @SedePref VARCHAR(20) = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdUsuario @IdGenerado OUTPUT;
        
        INSERT INTO Usuarios (
            ID, ID_ROL, TIPO_DOC, NUM_DOC, APE, NOM, GEN, FEC_NAC, 
            EMAIL, TEL, PSWDHASH, SEDE_PREF, Activo, FechaCreacion, FechaUltimaActualizacion
        )
        VALUES (
            @IdGenerado, @IdRol, @TipoDoc, @NumDoc, @Apellido, @Nombre, @Genero, @FechaNac,
            @Email, @Telefono, @PasswordHash, @SedePref, 1, GETDATE(), GETDATE()
        );
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: OBTENER USUARIO POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerUsuarioPorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ID, ID_ROL, TIPO_DOC, NUM_DOC, APE, NOM, GEN, FEC_NAC,
        EMAIL, TEL, PSWDHASH, SEDE_PREF, Activo, FechaCreacion, FechaUltimaActualizacion
    FROM Usuarios
    WHERE ID = @Id;
END
GO

-- SP: OBTENER USUARIO POR EMAIL
CREATE OR ALTER PROCEDURE sp_ObtenerUsuarioPorEmail
    @Email VARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ID, ID_ROL, TIPO_DOC, NUM_DOC, APE, NOM, GEN, FEC_NAC,
        EMAIL, TEL, PSWDHASH, SEDE_PREF, Activo, FechaCreacion, FechaUltimaActualizacion
    FROM Usuarios
    WHERE EMAIL = @Email AND Activo = 1;
END
GO

-- SP: ACTUALIZAR USUARIO
CREATE OR ALTER PROCEDURE sp_ActualizarUsuario
    @Id VARCHAR(20),
    @IdRol VARCHAR(20),
    @TipoDoc VARCHAR(10) = NULL,
    @NumDoc VARCHAR(20) = NULL,
    @Apellido VARCHAR(100),
    @Nombre VARCHAR(100),
    @Genero CHAR(1),
    @FechaNac DATE = NULL,
    @Email VARCHAR(150),
    @Telefono VARCHAR(20) = NULL,
    @SedePref VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE Usuarios
        SET ID_ROL = @IdRol,
            TIPO_DOC = @TipoDoc,
            NUM_DOC = @NumDoc,
            APE = @Apellido,
            NOM = @Nombre,
            GEN = @Genero,
            FEC_NAC = @FechaNac,
            EMAIL = @Email,
            TEL = @Telefono,
            SEDE_PREF = @SedePref,
            FechaUltimaActualizacion = GETDATE()
        WHERE ID = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

-- SP: DESACTIVAR USUARIO
CREATE OR ALTER PROCEDURE sp_DesactivarUsuario
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Usuarios
    SET Activo = 0,
        FechaUltimaActualizacion = GETDATE()
    WHERE ID = @Id;
END
GO

-- SP: LISTAR USUARIOS ACTIVOS
CREATE OR ALTER PROCEDURE sp_ListarUsuariosActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ID, ID_ROL, TIPO_DOC, NUM_DOC, APE, NOM, GEN, FEC_NAC,
        EMAIL, TEL, SEDE_PREF, Activo, FechaCreacion, FechaUltimaActualizacion
    FROM Usuarios
    WHERE Activo = 1
    ORDER BY FechaCreacion DESC;
END
GO

PRINT '  ? SPs CRUD Usuarios creados';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- PACIENTES - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR PACIENTE
CREATE OR ALTER PROCEDURE sp_InsertarPaciente
    @IdUsuario VARCHAR(20),
    @Peso DECIMAL(5,2) = NULL,
    @Edad DATE = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdPaciente @IdGenerado OUTPUT;
        
        INSERT INTO Pacientes (ID, ID_USU, PESO, EDAD)
        VALUES (@IdGenerado, @IdUsuario, @Peso, @Edad);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: OBTENER PACIENTE POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerPacientePorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID, ID_USU, PESO, EDAD
    FROM Pacientes
    WHERE ID = @Id;
END
GO

-- SP: OBTENER PACIENTE POR ID USUARIO
CREATE OR ALTER PROCEDURE sp_ObtenerPacientePorIdUsuario
    @IdUsuario VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID, ID_USU, PESO, EDAD
    FROM Pacientes
    WHERE ID_USU = @IdUsuario;
END
GO

-- SP: ACTUALIZAR PACIENTE
CREATE OR ALTER PROCEDURE sp_ActualizarPaciente
    @Id VARCHAR(20),
    @Peso DECIMAL(5,2) = NULL,
    @Edad DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Pacientes
    SET PESO = @Peso,
        EDAD = @Edad
    WHERE ID = @Id;
END
GO

-- SP: LISTAR PACIENTES
CREATE OR ALTER PROCEDURE sp_ListarPacientes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.ID,
        p.ID_USU,
        p.PESO,
        p.EDAD,
        u.NOM,
        u.APE,
        u.EMAIL,
        u.TEL
    FROM Pacientes p
    INNER JOIN Usuarios u ON p.ID_USU = u.ID
    WHERE u.Activo = 1
    ORDER BY u.NOM, u.APE;
END
GO

PRINT '  ? SPs CRUD Pacientes creados';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- RESUMEN
-- ????????????????????????????????????????????????????????????????????????????

PRINT '';
PRINT '??????????????????????????????????????????????????????????????????????????';
PRINT '?            STORED PROCEDURES CRUD CREADOS EXITOSAMENTE                 ?';
PRINT '??????????????????????????????????????????????????????????????????????????';
PRINT '';
PRINT 'USUARIOS:';
PRINT '  ? sp_InsertarUsuario';
PRINT '  ? sp_ObtenerUsuarioPorId';
PRINT '  ? sp_ObtenerUsuarioPorEmail';
PRINT '  ? sp_ActualizarUsuario';
PRINT '  ? sp_DesactivarUsuario';
PRINT '  ? sp_ListarUsuariosActivos';
PRINT '';
PRINT 'PACIENTES:';
PRINT '  ? sp_InsertarPaciente';
PRINT '  ? sp_ObtenerPacientePorId';
PRINT '  ? sp_ObtenerPacientePorIdUsuario';
PRINT '  ? sp_ActualizarPaciente';
PRINT '  ? sp_ListarPacientes';
PRINT '';
GO
