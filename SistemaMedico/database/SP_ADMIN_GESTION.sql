-- ================================================================================
-- STORED PROCEDURES - GESTIÓN DE USUARIOS (ADMINISTRADOR)
-- Casos de Uso: Gestionar Cuentas
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando SPs para Gestión de Usuarios (Admin)...';
GO

-- ===============================================================
-- SP: INSERTAR ADMINISTRADOR
-- ===============================================================
CREATE OR ALTER PROCEDURE InsertarAdministrador
    @IdUsuario VARCHAR(20),
    @NivelAcceso INT = 1,
    @PermisosEspeciales VARCHAR(MAX) = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validar que el usuario existe y tiene rol de Administrador
        IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE ID = @IdUsuario AND ID_ROL = 'R0000003')
        BEGIN
            RAISERROR('El usuario no existe o no tiene rol de Administrador', 16, 1);
            RETURN;
        END
        
        -- Validar que el usuario no tenga ya un registro de administrador
        IF EXISTS (SELECT 1 FROM Administradores WHERE ID_USU = @IdUsuario)
        BEGIN
            RAISERROR('El usuario ya tiene un registro de administrador', 16, 1);
            RETURN;
        END
        
        -- Generar el ID del administrador
        EXEC sp_GenerarIdAdministrador @IdGenerado OUTPUT;
        
        -- Insertar el administrador
        INSERT INTO Administradores (
            ID, 
            ID_USU, 
            NIVEL_ACCESO, 
            PERMISOS_ESPECIALES, 
            ULTIMA_ACCION, 
            ACCIONES_REALIZADAS
        )
        VALUES (
            @IdGenerado, 
            @IdUsuario, 
            @NivelAcceso, 
            @PermisosEspeciales, 
            GETDATE(), 
            0
        );
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO

-- ===============================================================
-- SP: LISTAR TODOS LOS USUARIOS (CON FILTROS)
-- ===============================================================
CREATE OR ALTER PROCEDURE Admin_ListarUsuarios
    @FiltroRol VARCHAR(20) = NULL,
    @FiltroBusqueda VARCHAR(100) = NULL,
    @SoloActivos BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        u.ID,
        u.NOM,
        u.APE,
        u.EMAIL,
        u.TEL,
        u.NUM_DOC,
        r.NOM_ROL as Rol,
        u.Activo,
        u.FechaCreacion as FechaRegistro,
        CASE 
            WHEN u.ID_ROL = 'R0000003' THEN (SELECT COUNT(*) FROM Citas c INNER JOIN Pacientes p ON c.ID_PAC = p.ID WHERE p.ID_USU = u.ID)
            WHEN u.ID_ROL = 'R0000002' THEN (SELECT COUNT(*) FROM Citas c INNER JOIN Doctores d ON c.ID_DOC = d.ID WHERE d.ID_USU = u.ID)
            ELSE 0
        END as TotalCitas
    FROM Usuarios u
    INNER JOIN Roles r ON u.ID_ROL = r.ID
    WHERE 
        (@FiltroRol IS NULL OR u.ID_ROL = @FiltroRol)
        AND (@FiltroBusqueda IS NULL OR u.NOM LIKE '%' + @FiltroBusqueda + '%' 
             OR u.APE LIKE '%' + @FiltroBusqueda + '%' 
             OR u.EMAIL LIKE '%' + @FiltroBusqueda + '%')
        AND (@SoloActivos = 0 OR u.Activo = 1)
    ORDER BY u.FechaCreacion DESC;
END
GO

-- ===============================================================
-- SP: OBTENER ADMINISTRADOR POR ID USUARIO
-- ===============================================================
CREATE OR ALTER PROCEDURE ObtenerAdministradorPorIdUsuario
    @IdUsuario VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        a.ID,
        a.ID_USU,
        a.NIVEL_ACCESO,
        a.PERMISOS_ESPECIALES,
        a.ULTIMA_ACCION,
        a.ACCIONES_REALIZADAS,
        u.NOM,
        u.APE,
        u.EMAIL,
        u.TEL,
        u.NUM_DOC
    FROM Administradores a
    INNER JOIN Usuarios u ON a.ID_USU = u.ID
    WHERE a.ID_USU = @IdUsuario;
END
GO

PRINT '? SP creado exitosamente';
GO

-- ===============================================================
-- SP: OBTENER DETALLE COMPLETO DE USUARIO
-- ===============================================================
CREATE OR ALTER PROCEDURE Admin_ObtenerDetalleUsuario
    @IdUsuario VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Información básica del usuario
    SELECT 
        u.ID,
        u.ID_ROL,
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
        r.NOM_ROL as NombreRol,
        s.NOM_SEDE as NombreSede
    FROM Usuarios u
    INNER JOIN Roles r ON u.ID_ROL = r.ID
    LEFT JOIN Sedes s ON u.SEDE_PREF = s.ID
    WHERE u.ID = @IdUsuario;

    -- Si es Paciente
    IF EXISTS (SELECT 1 FROM Pacientes WHERE ID_USU = @IdUsuario)
    BEGIN
        SELECT 
            p.ID as IdPaciente,
            p.PESO,
            p.EDAD,
            COUNT(c.ID) as TotalCitas,
            COUNT(CASE WHEN c.ESTADO = 'F' THEN 1 END) as CitasCompletadas
        FROM Pacientes p
        LEFT JOIN Citas c ON p.ID = c.ID_PAC
        WHERE p.ID_USU = @IdUsuario
        GROUP BY p.ID, p.PESO, p.EDAD;
    END

    -- Si es Doctor
    IF EXISTS (SELECT 1 FROM Doctores WHERE ID_USU = @IdUsuario)
    BEGIN
        SELECT 
            d.ID as IdDoctor,
            d.ID_ESP,
            d.COD_MED,
            d.EXPE,
            e.NOM_ESP as NombreEspecialidad,
            COUNT(c.ID) as TotalCitas,
            COUNT(CASE WHEN c.ESTADO = 'F' THEN 1 END) as CitasRealizadas
        FROM Doctores d
        LEFT JOIN Especialidades e ON d.ID_ESP = e.ID
        LEFT JOIN Citas c ON d.ID = c.ID_DOC
        WHERE d.ID_USU = @IdUsuario
        GROUP BY d.ID, d.ID_ESP, d.COD_MED, d.EXPE, e.NOM_ESP;
    END

    -- Si es Administrador
    IF EXISTS (SELECT 1 FROM Administradores WHERE ID_USU = @IdUsuario)
    BEGIN
        SELECT 
            a.ID as IdAdmin,
            a.NIVEL_ACCESO,
            a.PERMISOS_ESPECIALES,
            a.ULTIMA_ACCION,
            a.ACCIONES_REALIZADAS
        FROM Administradores a
        WHERE a.ID_USU = @IdUsuario;
    END
END
GO

-- ===============================================================
-- SP: CREAR USUARIO COMPLETO (CON ROL)
-- ===============================================================
CREATE OR ALTER PROCEDURE Admin_CrearUsuarioCompleto
    @IdRol VARCHAR(20),
    @TipoDoc VARCHAR(10),
    @NumDoc VARCHAR(20),
    @Apellido VARCHAR(100),
    @Nombre VARCHAR(100),
    @Genero CHAR(1),
    @FechaNac DATE,
    @Email VARCHAR(150),
    @Telefono VARCHAR(20),
    @Password VARCHAR(100),
    @SedePref VARCHAR(20) = NULL,
    -- Parámetros adicionales según el rol
    @IdEspecialidad VARCHAR(20) = NULL,  -- Para Doctor
    @NumColegiatura VARCHAR(50) = NULL,   -- Para Doctor
    @AniosExperiencia INT = NULL,         -- Para Doctor
    @NivelAcceso INT = 1,                 -- Para Administrador
    @IdGeneradoUsuario VARCHAR(20) OUTPUT,
    @IdGeneradoRol VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @PasswordHash VARBINARY(32);
        SET @PasswordHash = HASHBYTES('SHA2_256', @Password);
        
        -- 1. Crear Usuario
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
            @IdGenerado = @IdGeneradoUsuario OUTPUT;
        
        -- 2. Crear registro según el rol
        IF @IdRol = 'R0000003' -- Paciente
        BEGIN
            EXEC sp_InsertarPaciente
                @IdUsuario = @IdGeneradoUsuario,
                @Peso = NULL,
                @Edad = NULL,
                @IdGenerado = @IdGeneradoRol OUTPUT;
        END
        ELSE IF @IdRol = 'R0000002' -- Doctor
        BEGIN
            EXEC sp_InsertarDoctor
                @IdUsuario = @IdGeneradoUsuario,
                @IdEspecialidad = @IdEspecialidad,
                @NumColegiatura = @NumColegiatura,
                @AniosExperiencia = @AniosExperiencia,
                @IdGenerado = @IdGeneradoRol OUTPUT;
        END
        ELSE IF @IdRol = 'R0000001' -- Administrador
        BEGIN
            EXEC InsertarAdministrador
                @IdUsuario = @IdGeneradoUsuario,
                @NivelAcceso = @NivelAcceso,
                @PermisosEspeciales = NULL,
                @IdGenerado = @IdGeneradoRol OUTPUT;
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

-- ===============================================================
-- SP: EDITAR USUARIO (SIN CAMBIAR ROL)
-- ===============================================================
CREATE OR ALTER PROCEDURE sp_Admin_EditarUsuario
    @IdUsuario VARCHAR(20),
    @TipoDoc VARCHAR(10),
    @NumDoc VARCHAR(20),
    @Apellido VARCHAR(100),
    @Nombre VARCHAR(100),
    @Genero CHAR(1),
    @FechaNac DATE,
    @Email VARCHAR(150),
    @Telefono VARCHAR(20),
    @SedePref VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @IdRol VARCHAR(20);
    SELECT @IdRol = ID_ROL FROM Usuarios WHERE ID = @IdUsuario;

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
END
GO

-- ===============================================================
-- SP: DESACTIVAR/REACTIVAR USUARIO
-- ===============================================================
CREATE OR ALTER PROCEDURE sp_Admin_CambiarEstadoUsuario
    @IdUsuario VARCHAR(20),
    @NuevoEstado BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Usuarios
    SET Activo = @NuevoEstado,
        FechaUltimaActualizacion = GETDATE()
    WHERE ID = @IdUsuario;
END
GO

-- ===============================================================
-- SP: RESETEAR CONTRASEÑA
-- ===============================================================
CREATE OR ALTER PROCEDURE sp_Admin_ResetearPassword
    @IdUsuario VARCHAR(20),
    @NuevaPassword VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PasswordHash VARBINARY(32);
    SET @PasswordHash = HASHBYTES('SHA2_256', @NuevaPassword);

    UPDATE Usuarios
    SET PSWDHASH = @PasswordHash,
        FechaUltimaActualizacion = GETDATE()
    WHERE ID = @IdUsuario;
END
GO

-- ===============================================================
-- SP: OBTENER ESTADÍSTICAS DE USUARIOS
-- ===============================================================
CREATE OR ALTER PROCEDURE sp_Admin_ObtenerEstadisticasUsuarios
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        COUNT(*) as TotalUsuarios,
        SUM(CASE WHEN Activo = 1 THEN 1 ELSE 0 END) as UsuariosActivos,
        SUM(CASE WHEN Activo = 0 THEN 1 ELSE 0 END) as UsuariosInactivos,
        SUM(CASE WHEN ID_ROL = 'R0000003' THEN 1 ELSE 0 END) as TotalPacientes,
        SUM(CASE WHEN ID_ROL = 'R0000002' THEN 1 ELSE 0 END) as TotalDoctores,
        SUM(CASE WHEN ID_ROL = 'R0000001' THEN 1 ELSE 0 END) as TotalAdministradores
    FROM Usuarios;
END
GO

PRINT '? SPs de Gestión de Usuarios creados exitosamente';
GO

-- ================================================================================
-- STORED PROCEDURES - GESTIÓN DE CLÍNICA (ADMINISTRADOR)
-- Casos de Uso: Gestionar Clínica, Especialidades, Sedes
-- ================================================================================

PRINT 'Creando SPs para Gestión de Clínica (Admin)...';
GO

-- ===============================================================
-- SP: LISTAR ESPECIALIDADES CON DOCTORES
-- ===============================================================
CREATE OR ALTER PROCEDURE Admin_ListarEspecialidadesConDoctores
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.ID,
        e.NOM_ESP as NombreEspecialidad,
        e.DES_ESP as Descripcion,
        COUNT(d.ID) as TotalDoctores,
        COUNT(CASE WHEN u.Activo = 1 THEN 1 END) as DoctoresActivos
    FROM Especialidades e
    LEFT JOIN Doctores d ON e.ID = d.ID_ESP
    LEFT JOIN Usuarios u ON d.ID_USU = u.ID
    GROUP BY e.ID, e.NOM_ESP, e.DES_ESP
    ORDER BY e.NOM_ESP;
END
GO

-- ===============================================================
-- SP: ASIGNAR DOCTOR A ESPECIALIDAD
-- ===============================================================
CREATE OR ALTER PROCEDURE Admin_AsignarDoctorEspecialidad
    @IdDoctor VARCHAR(20),
    @IdEspecialidad VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Doctores
    SET ID_ESP = @IdEspecialidad
    WHERE ID = @IdDoctor;
END
GO

-- ===============================================================
-- SP: LISTAR SEDES CON ESTADÍSTICAS
-- ===============================================================
CREATE OR ALTER PROCEDURE Admin_ListarSedesConEstadisticas
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.ID,
        s.NOM_SEDE as NombreSede,
        s.DIR_SEDE as Direccion,
        COUNT(DISTINCT u.ID) as UsuariosPrefieren,
        COUNT(DISTINCT c.ID) as TotalCitas
    FROM Sedes s
    LEFT JOIN Usuarios u ON s.ID = u.SEDE_PREF AND u.Activo = 1
    LEFT JOIN Citas c ON s.ID = c.ID_SEDE
    GROUP BY s.ID, s.NOM_SEDE, s.DIR_SEDE
    ORDER BY s.NOM_SEDE;
END
GO

-- ===============================================================
-- SP: REGISTRAR ACCIÓN ADMINISTRATIVA
-- ===============================================================
CREATE OR ALTER PROCEDURE RegistrarAccionAdmin
    @IdAdmin VARCHAR(20),
    @TipoAccion VARCHAR(50),
    @Descripcion VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Administradores
    SET ULTIMA_ACCION = GETDATE(),
        ACCIONES_REALIZADAS = ACCIONES_REALIZADAS + 1
    WHERE ID = @IdAdmin;
    
    -- Opcional: Aquí podrías insertar en una tabla de log de acciones
    -- INSERT INTO LogAccionesAdmin (ID_ADMIN, TIPO_ACCION, DESCRIPCION, FECHA)
    -- VALUES (@IdAdmin, @TipoAccion, @Descripcion, GETDATE());
END
GO

PRINT '? SPs de Gestión de Clínica creados exitosamente';
GO

-- ============================================================================
-- RESUMEN
-- ============================================================================

PRINT '';
PRINT '????????????????????????????????????????????????????????????????????????';
PRINT '           STORED PROCEDURES DE ADMINISTRACIÓN CREADOS                   ';
PRINT '????????????????????????????????????????????????????????????????????????';
PRINT '';
PRINT 'GESTIÓN DE USUARIOS:';
PRINT '  ? InsertarAdministrador';
PRINT '  ? Admin_ListarUsuarios';
PRINT '  ? ObtenerAdministradorPorIdUsuario';
PRINT '  ? Admin_ObtenerDetalleUsuario';
PRINT '  ? Admin_CrearUsuarioCompleto';
PRINT '  ? sp_Admin_EditarUsuario';
PRINT '  ? sp_Admin_CambiarEstadoUsuario';
PRINT '  ? sp_Admin_ResetearPassword';
PRINT '  ? sp_Admin_ObtenerEstadisticasUsuarios';
PRINT '';
PRINT 'GESTIÓN DE CLÍNICA:';
PRINT '  ? Admin_ListarEspecialidadesConDoctores';
PRINT '  ? Admin_AsignarDoctorEspecialidad';
PRINT '  ? Admin_ListarSedesConEstadisticas';
PRINT '  ? RegistrarAccionAdmin';
PRINT '';
GO
