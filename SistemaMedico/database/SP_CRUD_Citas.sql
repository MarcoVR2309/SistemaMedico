-- ================================================================================
-- STORED PROCEDURES CRUD - CITAS
-- Base de Datos: RedCLinicas
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando Stored Procedures CRUD para Citas...';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- CITAS - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR CITA
CREATE OR ALTER PROCEDURE sp_InsertarCita
    @IdPaciente VARCHAR(20),
    @IdDoctor VARCHAR(20),
    @IdSede VARCHAR(20),
    @FechaCita DATETIME,
    @Motivo VARCHAR(500) = NULL,
    @Estado VARCHAR(20) = 'Pendiente',
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdCita @IdGenerado OUTPUT;
        
        INSERT INTO Citas (ID, ID_PAC, ID_DOC, ID_SEDE, FEC_CITA, MOTIVO, ESTADO, FechaCreacion)
        VALUES (@IdGenerado, @IdPaciente, @IdDoctor, @IdSede, @FechaCita, @Motivo, @Estado, GETDATE());
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: OBTENER CITA POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerCitaPorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.ID,
        c.ID_PAC,
        c.ID_DOC,
        c.ID_SEDE,
        c.FEC_CITA,
        c.MOTIVO,
        c.ESTADO,
        c.FechaCreacion,
        up.NOM AS NombrePaciente,
        up.APE AS ApellidoPaciente,
        up.EMAIL AS EmailPaciente,
        ud.NOM AS NombreDoctor,
        ud.APE AS ApellidoDoctor,
        s.NOM_SEDE AS NombreSede,
        e.NOM_ESP AS Especialidad
    FROM Citas c
    INNER JOIN Pacientes p ON c.ID_PAC = p.ID
    INNER JOIN Usuarios up ON p.ID_USU = up.ID
    INNER JOIN Doctores d ON c.ID_DOC = d.ID
    INNER JOIN Usuarios ud ON d.ID_USU = ud.ID
    LEFT JOIN Sedes s ON c.ID_SEDE = s.ID
    LEFT JOIN Especialidades e ON d.ID_ESP = e.ID
    WHERE c.ID = @Id;
END
GO

-- SP: LISTAR CITAS POR PACIENTE
CREATE OR ALTER PROCEDURE sp_ListarCitasPorPaciente
    @IdPaciente VARCHAR(20),
    @FechaInicio DATE = NULL,
    @FechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.ID,
        c.FEC_CITA,
        c.MOTIVO,
        c.ESTADO,
        ud.NOM AS NombreDoctor,
        ud.APE AS ApellidoDoctor,
        s.NOM_SEDE AS NombreSede,
        e.NOM_ESP AS Especialidad
    FROM Citas c
    INNER JOIN Doctores d ON c.ID_DOC = d.ID
    INNER JOIN Usuarios ud ON d.ID_USU = ud.ID
    LEFT JOIN Sedes s ON c.ID_SEDE = s.ID
    LEFT JOIN Especialidades e ON d.ID_ESP = e.ID
    WHERE c.ID_PAC = @IdPaciente
      AND (@FechaInicio IS NULL OR CAST(c.FEC_CITA AS DATE) >= @FechaInicio)
      AND (@FechaFin IS NULL OR CAST(c.FEC_CITA AS DATE) <= @FechaFin)
    ORDER BY c.FEC_CITA DESC;
END
GO

-- SP: LISTAR CITAS POR DOCTOR
CREATE OR ALTER PROCEDURE sp_ListarCitasPorDoctor
    @IdDoctor VARCHAR(20),
    @FechaInicio DATE = NULL,
    @FechaFin DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.ID,
        c.FEC_CITA,
        c.MOTIVO,
        c.ESTADO,
        up.NOM AS NombrePaciente,
        up.APE AS ApellidoPaciente,
        up.TEL AS TelefonoPaciente,
        s.NOM_SEDE AS NombreSede
    FROM Citas c
    INNER JOIN Pacientes p ON c.ID_PAC = p.ID
    INNER JOIN Usuarios up ON p.ID_USU = up.ID
    LEFT JOIN Sedes s ON c.ID_SEDE = s.ID
    WHERE c.ID_DOC = @IdDoctor
      AND (@FechaInicio IS NULL OR CAST(c.FEC_CITA AS DATE) >= @FechaInicio)
      AND (@FechaFin IS NULL OR CAST(c.FEC_CITA AS DATE) <= @FechaFin)
    ORDER BY c.FEC_CITA ASC;
END
GO

-- SP: ACTUALIZAR ESTADO CITA
CREATE OR ALTER PROCEDURE sp_ActualizarEstadoCita
    @Id VARCHAR(20),
    @Estado VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Citas
    SET ESTADO = @Estado
    WHERE ID = @Id;
END
GO

-- SP: CANCELAR CITA
CREATE OR ALTER PROCEDURE sp_CancelarCita
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Citas
    SET ESTADO = 'Cancelada'
    WHERE ID = @Id;
END
GO

-- SP: CONFIRMAR CITA
CREATE OR ALTER PROCEDURE sp_ConfirmarCita
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Citas
    SET ESTADO = 'Confirmada'
    WHERE ID = @Id;
END
GO

PRINT '  ? SPs CRUD Citas creados';
GO
