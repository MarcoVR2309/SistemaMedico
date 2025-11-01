-- ================================================================================
-- STORED PROCEDURES CRUD - CONSULTAS MEDICAS
-- Base de Datos: RedCLinicas
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando Stored Procedures CRUD para Consultas Medicas...';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- CONSULTAS MEDICAS - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR CONSULTA MEDICA
CREATE OR ALTER PROCEDURE sp_InsertarConsultaMedica
    @IdCita VARCHAR(20),
    @Diagnostico VARCHAR(MAX),
    @Tratamiento VARCHAR(MAX) = NULL,
    @Observaciones VARCHAR(MAX) = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdConsultaMedica @IdGenerado OUTPUT;
        
        INSERT INTO ConsultasMedicas (ID, ID_CITA, DIAGNOSTICO, TRATAMIENTO, OBSERVACIONES, FechaCreacion)
        VALUES (@IdGenerado, @IdCita, @Diagnostico, @Tratamiento, @Observaciones, GETDATE());
        
        UPDATE Citas SET ESTADO = 'Atendida' WHERE ID = @IdCita;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: OBTENER CONSULTA MEDICA POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerConsultaMedicaPorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        cm.ID,
        cm.ID_CITA,
        cm.DIAGNOSTICO,
        cm.TRATAMIENTO,
        cm.OBSERVACIONES,
        cm.FechaCreacion,
        c.FEC_CITA,
        up.NOM AS NombrePaciente,
        up.APE AS ApellidoPaciente,
        ud.NOM AS NombreDoctor,
        ud.APE AS ApellidoDoctor
    FROM ConsultasMedicas cm
    INNER JOIN Citas c ON cm.ID_CITA = c.ID
    INNER JOIN Pacientes p ON c.ID_PAC = p.ID
    INNER JOIN Usuarios up ON p.ID_USU = up.ID
    INNER JOIN Doctores d ON c.ID_DOC = d.ID
    INNER JOIN Usuarios ud ON d.ID_USU = ud.ID
    WHERE cm.ID = @Id;
END
GO

-- SP: OBTENER CONSULTA POR ID CITA
CREATE OR ALTER PROCEDURE sp_ObtenerConsultaPorIdCita
    @IdCita VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        cm.ID,
        cm.DIAGNOSTICO,
        cm.TRATAMIENTO,
        cm.OBSERVACIONES,
        cm.FechaCreacion
    FROM ConsultasMedicas cm
    WHERE cm.ID_CITA = @IdCita;
END
GO

-- SP: LISTAR CONSULTAS POR PACIENTE
CREATE OR ALTER PROCEDURE sp_ListarConsultasPorPaciente
    @IdPaciente VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        cm.ID,
        cm.DIAGNOSTICO,
        cm.TRATAMIENTO,
        cm.FechaCreacion,
        c.FEC_CITA,
        ud.NOM AS NombreDoctor,
        ud.APE AS ApellidoDoctor,
        e.NOM_ESP AS Especialidad
    FROM ConsultasMedicas cm
    INNER JOIN Citas c ON cm.ID_CITA = c.ID
    INNER JOIN Doctores d ON c.ID_DOC = d.ID
    INNER JOIN Usuarios ud ON d.ID_USU = ud.ID
    LEFT JOIN Especialidades e ON d.ID_ESP = e.ID
    WHERE c.ID_PAC = @IdPaciente
    ORDER BY cm.FechaCreacion DESC;
END
GO

-- SP: ACTUALIZAR CONSULTA MEDICA
CREATE OR ALTER PROCEDURE sp_ActualizarConsultaMedica
    @Id VARCHAR(20),
    @Diagnostico VARCHAR(MAX),
    @Tratamiento VARCHAR(MAX) = NULL,
    @Observaciones VARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ConsultasMedicas
    SET DIAGNOSTICO = @Diagnostico,
        TRATAMIENTO = @Tratamiento,
        OBSERVACIONES = @Observaciones
    WHERE ID = @Id;
END
GO

PRINT '  ? SPs CRUD Consultas Medicas creados';
GO
