-- ================================================================================
-- STORED PROCEDURES CRUD - DOCTORES
-- Base de Datos: RedCLinicas
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando Stored Procedures CRUD para Doctores...';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- DOCTORES - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR DOCTOR
CREATE OR ALTER PROCEDURE sp_InsertarDoctor
    @IdUsuario VARCHAR(20),
    @IdEspecialidad VARCHAR(20),
    @NumColegiatura VARCHAR(50),
    @AniosExperiencia INT = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdDoctor @IdGenerado OUTPUT;
        
        INSERT INTO Doctores (ID, ID_USU, ID_ESP, NUM_COLEGIATURA, ANIOS_EXP)
        VALUES (@IdGenerado, @IdUsuario, @IdEspecialidad, @NumColegiatura, @AniosExperiencia);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: OBTENER DOCTOR POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerDoctorPorId
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
GO

-- SP: OBTENER DOCTOR POR ID USUARIO
CREATE OR ALTER PROCEDURE sp_ObtenerDoctorPorIdUsuario
    @IdUsuario VARCHAR(20)
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
    WHERE d.ID_USU = @IdUsuario;
END
GO

-- SP: ACTUALIZAR DOCTOR
CREATE OR ALTER PROCEDURE sp_ActualizarDoctor
    @Id VARCHAR(20),
    @IdEspecialidad VARCHAR(20),
    @NumColegiatura VARCHAR(50),
    @AniosExperiencia INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Doctores
    SET ID_ESP = @IdEspecialidad,
        NUM_COLEGIATURA = @NumColegiatura,
        ANIOS_EXP = @AniosExperiencia
    WHERE ID = @Id;
END
GO

-- SP: LISTAR DOCTORES ACTIVOS
CREATE OR ALTER PROCEDURE sp_ListarDoctores
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
    WHERE u.Activo = 1
    ORDER BY u.NOM, u.APE;
END
GO

-- SP: LISTAR DOCTORES POR ESPECIALIDAD
CREATE OR ALTER PROCEDURE sp_ListarDoctoresPorEspecialidad
    @IdEspecialidad VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        d.ID,
        d.ID_USU,
        d.NUM_COLEGIATURA,
        d.ANIOS_EXP,
        u.NOM,
        u.APE,
        u.EMAIL,
        u.TEL
    FROM Doctores d
    INNER JOIN Usuarios u ON d.ID_USU = u.ID
    WHERE d.ID_ESP = @IdEspecialidad AND u.Activo = 1
    ORDER BY u.NOM, u.APE;
END
GO

PRINT '  ? SPs CRUD Doctores creados';
GO
