-- ================================================================================
-- STORED PROCEDURES CRUD - PAGOS Y ADMINISTRADORES
-- Base de Datos: RedCLinicas
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando Stored Procedures CRUD para Pagos y Administradores...';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- PAGOS - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR PAGO
CREATE OR ALTER PROCEDURE sp_InsertarPago
    @IdCita VARCHAR(20),
    @Monto DECIMAL(10,2),
    @MetodoPago VARCHAR(50),
    @Estado VARCHAR(20) = 'Pendiente',
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdPago @IdGenerado OUTPUT;
        
        INSERT INTO Pagos (ID, ID_CITA, MONTO, METODO_PAGO, ESTADO, FechaPago)
        VALUES (@IdGenerado, @IdCita, @Monto, @MetodoPago, @Estado, GETDATE());
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: OBTENER PAGO POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerPagoPorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.ID,
        p.ID_CITA,
        p.MONTO,
        p.METODO_PAGO,
        p.ESTADO,
        p.FechaPago,
        c.FEC_CITA,
        up.NOM AS NombrePaciente,
        up.APE AS ApellidoPaciente,
        ud.NOM AS NombreDoctor,
        ud.APE AS ApellidoDoctor
    FROM Pagos p
    INNER JOIN Citas c ON p.ID_CITA = c.ID
    INNER JOIN Pacientes pac ON c.ID_PAC = pac.ID
    INNER JOIN Usuarios up ON pac.ID_USU = up.ID
    INNER JOIN Doctores d ON c.ID_DOC = d.ID
    INNER JOIN Usuarios ud ON d.ID_USU = ud.ID
    WHERE p.ID = @Id;
END
GO

-- SP: LISTAR PAGOS POR PACIENTE
CREATE OR ALTER PROCEDURE sp_ListarPagosPorPaciente
    @IdPaciente VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.ID,
        p.MONTO,
        p.METODO_PAGO,
        p.ESTADO,
        p.FechaPago,
        c.FEC_CITA,
        ud.NOM AS NombreDoctor,
        ud.APE AS ApellidoDoctor
    FROM Pagos p
    INNER JOIN Citas c ON p.ID_CITA = c.ID
    INNER JOIN Doctores d ON c.ID_DOC = d.ID
    INNER JOIN Usuarios ud ON d.ID_USU = ud.ID
    WHERE c.ID_PAC = @IdPaciente
    ORDER BY p.FechaPago DESC;
END
GO

-- SP: ACTUALIZAR ESTADO PAGO
CREATE OR ALTER PROCEDURE sp_ActualizarEstadoPago
    @Id VARCHAR(20),
    @Estado VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Pagos
    SET ESTADO = @Estado
    WHERE ID = @Id;
END
GO

-- SP: CONFIRMAR PAGO
CREATE OR ALTER PROCEDURE sp_ConfirmarPago
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Pagos
    SET ESTADO = 'Pagado',
        FechaPago = GETDATE()
    WHERE ID = @Id;
END
GO

-- ????????????????????????????????????????????????????????????????????????????
-- ADMINISTRADORES - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR ADMINISTRADOR
CREATE OR ALTER PROCEDURE sp_InsertarAdministrador
    @IdUsuario VARCHAR(20),
    @NivelAcceso INT = 1,
    @PermisosEspeciales VARCHAR(MAX) = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC sp_GenerarIdAdministrador @IdGenerado OUTPUT;
        
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
        THROW;
    END CATCH
END
GO

-- SP: OBTENER ADMINISTRADOR POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerAdministradorPorId
    @Id VARCHAR(20)
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
    WHERE a.ID = @Id;
END
GO

-- SP: OBTENER ADMINISTRADOR POR ID USUARIO
CREATE OR ALTER PROCEDURE sp_ObtenerAdministradorPorIdUsuario
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

-- SP: LISTAR ADMINISTRADORES
CREATE OR ALTER PROCEDURE sp_ListarAdministradores
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
        u.Activo
    FROM Administradores a
    INNER JOIN Usuarios u ON a.ID_USU = u.ID
    WHERE u.Activo = 1
    ORDER BY a.NIVEL_ACCESO DESC, u.NOM, u.APE;
END
GO

-- SP: ACTUALIZAR ADMINISTRADOR
CREATE OR ALTER PROCEDURE sp_ActualizarAdministrador
    @Id VARCHAR(20),
    @NivelAcceso INT,
    @PermisosEspeciales VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Administradores
    SET NIVEL_ACCESO = @NivelAcceso,
        PERMISOS_ESPECIALES = @PermisosEspeciales
    WHERE ID = @Id;
END
GO

-- SP: REGISTRAR ACCION ADMINISTRATIVA
CREATE OR ALTER PROCEDURE sp_RegistrarAccionAdmin
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
END
GO

PRINT '  ? SPs CRUD Pagos y Administradores creados';
GO
