-- ================================================================================
-- STORED PROCEDURES CRUD - RECETAS MEDICAS
-- Base de Datos: RedCLinicas
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando Stored Procedures CRUD para Recetas Medicas...';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- RECETAS MEDICAS - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR RECETA MEDICA
CREATE OR ALTER PROCEDURE sp_InsertarRecetaMedica
    @IdConsulta VARCHAR(20),
    @Indicaciones VARCHAR(MAX) = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdRecetaMedica @IdGenerado OUTPUT;
        
        INSERT INTO RecetasMedicas (ID, ID_CONSULTA, INDICACIONES, FechaEmision)
        VALUES (@IdGenerado, @IdConsulta, @Indicaciones, GETDATE());
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: OBTENER RECETA POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerRecetaPorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        rm.ID,
        rm.ID_CONSULTA,
        rm.INDICACIONES,
        rm.FechaEmision,
        cm.DIAGNOSTICO,
        up.NOM AS NombrePaciente,
        up.APE AS ApellidoPaciente,
        ud.NOM AS NombreDoctor,
        ud.APE AS ApellidoDoctor,
        d.NUM_COLEGIATURA
    FROM RecetasMedicas rm
    INNER JOIN ConsultasMedicas cm ON rm.ID_CONSULTA = cm.ID
    INNER JOIN Citas c ON cm.ID_CITA = c.ID
    INNER JOIN Pacientes p ON c.ID_PAC = p.ID
    INNER JOIN Usuarios up ON p.ID_USU = up.ID
    INNER JOIN Doctores d ON c.ID_DOC = d.ID
    INNER JOIN Usuarios ud ON d.ID_USU = ud.ID
    WHERE rm.ID = @Id;
END
GO

-- SP: LISTAR RECETAS POR PACIENTE
CREATE OR ALTER PROCEDURE sp_ListarRecetasPorPaciente
    @IdPaciente VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        rm.ID,
        rm.INDICACIONES,
        rm.FechaEmision,
        cm.DIAGNOSTICO,
        ud.NOM AS NombreDoctor,
        ud.APE AS ApellidoDoctor
    FROM RecetasMedicas rm
    INNER JOIN ConsultasMedicas cm ON rm.ID_CONSULTA = cm.ID
    INNER JOIN Citas c ON cm.ID_CITA = c.ID
    INNER JOIN Doctores d ON c.ID_DOC = d.ID
    INNER JOIN Usuarios ud ON d.ID_USU = ud.ID
    WHERE c.ID_PAC = @IdPaciente
    ORDER BY rm.FechaEmision DESC;
END
GO

-- ????????????????????????????????????????????????????????????????????????????
-- RECETAS DETALLE - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR DETALLE RECETA
CREATE OR ALTER PROCEDURE sp_InsertarRecetaDetalle
    @IdReceta VARCHAR(20),
    @Medicamento VARCHAR(200),
    @Dosis VARCHAR(100),
    @Frecuencia VARCHAR(100),
    @Duracion VARCHAR(100),
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdRecetaDetalle @IdGenerado OUTPUT;
        
        INSERT INTO RecetasDetalle (ID, ID_RECETA, MEDICAMENTO, DOSIS, FRECUENCIA, DURACION)
        VALUES (@IdGenerado, @IdReceta, @Medicamento, @Dosis, @Frecuencia, @Duracion);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: LISTAR DETALLES DE RECETA
CREATE OR ALTER PROCEDURE sp_ListarDetallesReceta
    @IdReceta VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ID,
        MEDICAMENTO,
        DOSIS,
        FRECUENCIA,
        DURACION
    FROM RecetasDetalle
    WHERE ID_RECETA = @IdReceta
    ORDER BY ID;
END
GO

-- SP: ACTUALIZAR DETALLE RECETA
CREATE OR ALTER PROCEDURE sp_ActualizarRecetaDetalle
    @Id VARCHAR(20),
    @Medicamento VARCHAR(200),
    @Dosis VARCHAR(100),
    @Frecuencia VARCHAR(100),
    @Duracion VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE RecetasDetalle
    SET MEDICAMENTO = @Medicamento,
        DOSIS = @Dosis,
        FRECUENCIA = @Frecuencia,
        DURACION = @Duracion
    WHERE ID = @Id;
END
GO

-- SP: ELIMINAR DETALLE RECETA
CREATE OR ALTER PROCEDURE sp_EliminarRecetaDetalle
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM RecetasDetalle WHERE ID = @Id;
END
GO

PRINT '  ? SPs CRUD Recetas Medicas creados';
GO
