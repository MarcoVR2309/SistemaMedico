-- ================================================================================
-- STORED PROCEDURES - ADMIN GESTION USUARIOS
-- Base de Datos: RedCLinicas
-- Dependencias: SP generadores (GenerarIdUsuario, GenerarIdDoctor, GenerarIdPaciente,
--                GenerarIdHorarioDoctor) y SP CRUD existentes para Usuarios, Doctores,
--                Pacientes y Especialidades.
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando Stored Procedures para Gestion de Usuarios (Admin)...';
GO

-- ================================================================================
-- LISTAR USUARIOS CON INDICADORES
-- ================================================================================

CREATE OR ALTER PROCEDURE sp_Admin_ListarUsuarios
    @TextoBusqueda      VARCHAR(200) = NULL,
    @IdRol              VARCHAR(20)  = NULL,
    @Estado             VARCHAR(10)  = NULL -- ACTIVO | INACTIVO | NULL (todos)
AS
BEGIN
    SET NOCOUNT ON;

    WITH UsuariosFiltrados AS (
        SELECT
            u.ID,
            u.ID_ROL,
            r.NOM_ROL AS NombreRol,
            u.TIPO_DOC,
            u.NUM_DOC,
            u.APE,
            u.NOM,
            u.GEN,
            u.FEC_NAC,
            u.EMAIL,
            u.TEL,
            u.SEDE_PREF,
            u.Activo,
            u.FechaCreacion,
            u.FechaUltimaActualizacion,
            d.ID AS IdDoctor,
            d.NUM_COLEGIATURA,
            d.ANIOS_EXP,
            e.ID AS IdEspecialidad,
            e.NOM_ESP AS NombreEspecialidad,
            p.ID AS IdPaciente,
            CONCAT(u.NOM, ' ', u.APE) AS NombreCompleto
        FROM Usuarios u
        INNER JOIN Roles r ON r.ID = u.ID_ROL
        LEFT JOIN Doctores d ON d.ID_USU = u.ID
        LEFT JOIN Especialidades e ON e.ID = d.ID_ESP
        LEFT JOIN Pacientes p ON p.ID_USU = u.ID
        WHERE
            (@TextoBusqueda IS NULL OR @TextoBusqueda = '' OR
                u.EMAIL LIKE '%' + @TextoBusqueda + '%' OR
                u.NOM LIKE '%' + @TextoBusqueda + '%' OR
                u.APE LIKE '%' + @TextoBusqueda + '%' OR
                u.NUM_DOC LIKE '%' + @TextoBusqueda + '%')
            AND (@IdRol IS NULL OR @IdRol = '' OR u.ID_ROL = @IdRol)
            AND (
                @Estado IS NULL OR @Estado = '' OR
                (@Estado = 'ACTIVO' AND u.Activo = 1) OR
                (@Estado = 'INACTIVO' AND u.Activo = 0)
            )
    )
    SELECT
        COUNT(1) AS TotalUsuarios,
        SUM(CASE WHEN NombreRol = 'Doctor' THEN 1 ELSE 0 END)     AS TotalDoctores,
        SUM(CASE WHEN NombreRol = 'Paciente' THEN 1 ELSE 0 END)   AS TotalPacientes,
        SUM(CASE WHEN Activo = 1 THEN 1 ELSE 0 END)               AS TotalActivos
    FROM UsuariosFiltrados;

    SELECT
        uf.ID,
        uf.NombreCompleto,
        uf.EMAIL,
        uf.NombreRol,
        uf.TEL,
        uf.Activo,
        uf.FechaCreacion,
        uf.IdDoctor,
        uf.NUM_COLEGIATURA,
        uf.ANIOS_EXP,
        uf.IdEspecialidad,
        uf.NombreEspecialidad,
        uf.IdPaciente
    FROM UsuariosFiltrados uf
    ORDER BY uf.FechaCreacion DESC;
END
GO

-- ================================================================================
-- OBTENER DETALLE DE USUARIO (ADMIN)
-- ================================================================================

CREATE OR ALTER PROCEDURE sp_Admin_ObtenerUsuarioDetalle
    @IdUsuario VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        u.ID,
        u.ID_ROL,
        r.NOM_ROL AS NombreRol,
        u.TIPO_DOC,
        u.NUM_DOC,
        u.APE,
        u.NOM,
        u.GEN,
        u.FEC_NAC,
        u.EMAIL,
        u.TEL,
        u.SEDE_PREF,
        u.Activo,
        u.FechaCreacion,
        u.FechaUltimaActualizacion,
        d.ID AS IdDoctor,
        d.ID_ESP,
        d.NUM_COLEGIATURA,
        d.ANIOS_EXP,
        e.NOM_ESP AS NombreEspecialidad,
        p.ID AS IdPaciente,
        p.PESO,
        p.EDAD
    FROM Usuarios u
    INNER JOIN Roles r ON r.ID = u.ID_ROL
    LEFT JOIN Doctores d ON d.ID_USU = u.ID
    LEFT JOIN Especialidades e ON e.ID = d.ID_ESP
    LEFT JOIN Pacientes p ON p.ID_USU = u.ID
    WHERE u.ID = @IdUsuario;
END
GO

-- ================================================================================
-- CREAR USUARIO (ADMIN)
-- ================================================================================

CREATE OR ALTER PROCEDURE sp_Admin_CrearUsuario
    @IdRol              VARCHAR(20),
    @TipoDoc            VARCHAR(10) = NULL,
    @NumDoc             VARCHAR(20) = NULL,
    @Apellido           VARCHAR(100),
    @Nombre             VARCHAR(100),
    @Genero             CHAR(1),
    @FechaNac           DATE = NULL,
    @Email              VARCHAR(150),
    @Telefono           VARCHAR(20) = NULL,
    @PasswordHash       VARBINARY(MAX),
    @SedePref           VARCHAR(20) = NULL,
    @IdEspecialidad     VARCHAR(20) = NULL,
    @NumColegiatura     VARCHAR(50) = NULL,
    @AnosExperiencia    INT = NULL,
    @PesoPaciente       DECIMAL(5,2) = NULL,
    @EdadPaciente       DATE = NULL,
    @IdUsuarioGenerado  VARCHAR(20) OUTPUT,
    @IdRelacionado      VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NombreRol VARCHAR(100);

    SELECT @NombreRol = NOM_ROL
    FROM Roles
    WHERE ID = @IdRol;

    IF @NombreRol IS NULL
    BEGIN
        THROW 50001, 'El rol especificado no existe.', 1;
    END;

    IF EXISTS (SELECT 1 FROM Usuarios WHERE EMAIL = @Email)
    BEGIN
        THROW 50002, 'Ya existe un usuario registrado con el email proporcionado.', 1;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        EXEC sp_InsertarUsuario
            @IdRol = @IdRol,
            @TipoDoc = @TipoDoc,
            @NumDoc = @NumDoc,
            @Apellido = @Apellido,
            @Nombre = @Nombre,
            @Genero = @Genero,
            @FechaNac = @FechaNac,
            @Email = @Email,
            @Telefono = @Telefono,
            @PasswordHash = @PasswordHash,
            @SedePref = @SedePref,
            @IdGenerado = @IdUsuarioGenerado OUTPUT;

        SET @IdRelacionado = NULL;

        IF @NombreRol = 'Doctor'
        BEGIN
            IF @IdEspecialidad IS NULL OR @IdEspecialidad = ''
            BEGIN
                THROW 50003, 'Debe seleccionar una especialidad para el rol Doctor.', 1;
            END;

            EXEC sp_InsertarDoctor
                @IdUsuario = @IdUsuarioGenerado,
                @IdEspecialidad = @IdEspecialidad,
                @NumColegiatura = @NumColegiatura,
                @AniosExperiencia = @AnosExperiencia,
                @IdGenerado = @IdRelacionado OUTPUT;
        END
        ELSE IF @NombreRol = 'Paciente'
        BEGIN
            EXEC sp_InsertarPaciente
                @IdUsuario = @IdUsuarioGenerado,
                @Peso = @PesoPaciente,
                @Edad = @EdadPaciente,
                @IdGenerado = @IdRelacionado OUTPUT;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ================================================================================
-- ACTUALIZAR USUARIO (ADMIN)
-- ================================================================================

CREATE OR ALTER PROCEDURE sp_Admin_ActualizarUsuario
    @IdUsuario          VARCHAR(20),
    @IdRol              VARCHAR(20),
    @TipoDoc            VARCHAR(10) = NULL,
    @NumDoc             VARCHAR(20) = NULL,
    @Apellido           VARCHAR(100),
    @Nombre             VARCHAR(100),
    @Genero             CHAR(1),
    @FechaNac           DATE = NULL,
    @Email              VARCHAR(150),
    @Telefono           VARCHAR(20) = NULL,
    @SedePref           VARCHAR(20) = NULL,
    @IdEspecialidad     VARCHAR(20) = NULL,
    @NumColegiatura     VARCHAR(50) = NULL,
    @AnosExperiencia    INT = NULL,
    @PesoPaciente       DECIMAL(5,2) = NULL,
    @EdadPaciente       DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @NombreRol VARCHAR(100);

    SELECT @NombreRol = NOM_ROL
    FROM Roles
    WHERE ID = @IdRol;

    IF @NombreRol IS NULL
    BEGIN
        THROW 50011, 'El rol especificado no existe.', 1;
    END;

    BEGIN TRY
        BEGIN TRANSACTION;

        EXEC sp_ActualizarUsuario
            @Id = @IdUsuario,
            @IdRol = @IdRol,
            @TipoDoc = @TipoDoc,
            @NumDoc = @NumDoc,
            @Apellido = @Apellido,
            @Nombre = @Nombre,
            @Genero = @Genero,
            @FechaNac = @FechaNac,
            @Email = @Email,
            @Telefono = @Telefono,
            @SedePref = @SedePref;

        IF @NombreRol = 'Doctor'
        BEGIN
            DECLARE @IdDoctor VARCHAR(20);
            SELECT @IdDoctor = ID FROM Doctores WHERE ID_USU = @IdUsuario;

            IF @IdDoctor IS NULL
            BEGIN
                -- No existia registro como doctor, se crea
                EXEC sp_InsertarDoctor
                    @IdUsuario = @IdUsuario,
                    @IdEspecialidad = @IdEspecialidad,
                    @NumColegiatura = @NumColegiatura,
                    @AniosExperiencia = @AnosExperiencia,
                    @IdGenerado = @IdDoctor OUTPUT;
            END
            ELSE
            BEGIN
                EXEC sp_ActualizarDoctor
                    @Id = @IdDoctor,
                    @IdEspecialidad = @IdEspecialidad,
                    @NumColegiatura = @NumColegiatura,
                    @AniosExperiencia = @AnosExperiencia;
            END
        END
        ELSE
        BEGIN
            -- Si el rol ya no es Doctor, eliminar registro asociado
            DELETE FROM Doctores WHERE ID_USU = @IdUsuario;
        END

        IF @NombreRol = 'Paciente'
        BEGIN
            DECLARE @IdPaciente VARCHAR(20);
            SELECT @IdPaciente = ID FROM Pacientes WHERE ID_USU = @IdUsuario;

            IF @IdPaciente IS NULL
            BEGIN
                EXEC sp_InsertarPaciente
                    @IdUsuario = @IdUsuario,
                    @Peso = @PesoPaciente,
                    @Edad = @EdadPaciente,
                    @IdGenerado = @IdPaciente OUTPUT;
            END
            ELSE
            BEGIN
                EXEC sp_ActualizarPaciente
                    @Id = @IdPaciente,
                    @Peso = @PesoPaciente,
                    @Edad = @EdadPaciente;
            END
        END
        ELSE
        BEGIN
            DELETE FROM Pacientes WHERE ID_USU = @IdUsuario;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ================================================================================
-- CAMBIAR ESTADO (ACTIVAR / DESACTIVAR)
-- ================================================================================

CREATE OR ALTER PROCEDURE sp_Admin_CambiarEstadoUsuario
    @IdUsuario  VARCHAR(20),
    @Activar    BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET Activo = CASE WHEN @Activar = 1 THEN 1 ELSE 0 END,
        FechaUltimaActualizacion = GETDATE()
    WHERE ID = @IdUsuario;

    SELECT Activo FROM Usuarios WHERE ID = @IdUsuario;
END
GO

-- ================================================================================
-- RESETEAR PASSWORD
-- ================================================================================

CREATE OR ALTER PROCEDURE sp_Admin_ResetearPasswordUsuario
    @IdUsuario    VARCHAR(20),
    @PasswordHash VARBINARY(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Usuarios
    SET PSWDHASH = @PasswordHash,
        FechaUltimaActualizacion = GETDATE()
    WHERE ID = @IdUsuario;
END
GO

PRINT '  -> SPs Gestion de Usuarios (Admin) creados';
GO

