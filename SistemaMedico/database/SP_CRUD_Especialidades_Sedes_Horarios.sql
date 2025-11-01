-- ================================================================================
-- STORED PROCEDURES CRUD - ESPECIALIDADES, SEDES, HORARIOS
-- Base de Datos: RedCLinicas
-- ================================================================================

USE RedCLinicas;
GO

PRINT 'Creando Stored Procedures CRUD para Especialidades, Sedes y Horarios...';
GO

-- ????????????????????????????????????????????????????????????????????????????
-- ESPECIALIDADES - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR ESPECIALIDAD
CREATE OR ALTER PROCEDURE sp_InsertarEspecialidad
    @Nombre VARCHAR(150),
    @Descripcion VARCHAR(500) = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdEspecialidad @IdGenerado OUTPUT;
        
        INSERT INTO Especialidades (ID, NOM_ESP, DESCRIP)
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

-- SP: OBTENER ESPECIALIDAD POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerEspecialidadPorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID, NOM_ESP, DESCRIP
    FROM Especialidades
    WHERE ID = @Id;
END
GO

-- SP: LISTAR ESPECIALIDADES
CREATE OR ALTER PROCEDURE sp_ListarEspecialidades
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID, NOM_ESP, DESCRIP
    FROM Especialidades
    ORDER BY NOM_ESP;
END
GO

-- SP: ACTUALIZAR ESPECIALIDAD
CREATE OR ALTER PROCEDURE sp_ActualizarEspecialidad
    @Id VARCHAR(20),
    @Nombre VARCHAR(150),
    @Descripcion VARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Especialidades
    SET NOM_ESP = @Nombre,
        DESCRIP = @Descripcion
    WHERE ID = @Id;
END
GO

-- ????????????????????????????????????????????????????????????????????????????
-- SEDES - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR SEDE
CREATE OR ALTER PROCEDURE sp_InsertarSede
    @Nombre VARCHAR(200),
    @Direccion VARCHAR(300) = NULL,
    @Telefono VARCHAR(20) = NULL,
    @Email VARCHAR(150) = NULL,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdSede @IdGenerado OUTPUT;
        
        INSERT INTO Sedes (ID, NOM_SEDE, DIRECCION, TELEFONO, EMAIL)
        VALUES (@IdGenerado, @Nombre, @Direccion, @Telefono, @Email);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: OBTENER SEDE POR ID
CREATE OR ALTER PROCEDURE sp_ObtenerSedePorId
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID, NOM_SEDE, DIRECCION, TELEFONO, EMAIL
    FROM Sedes
    WHERE ID = @Id;
END
GO

-- SP: LISTAR SEDES
CREATE OR ALTER PROCEDURE sp_ListarSedes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID, NOM_SEDE, DIRECCION, TELEFONO, EMAIL
    FROM Sedes
    ORDER BY NOM_SEDE;
END
GO

-- SP: ACTUALIZAR SEDE
CREATE OR ALTER PROCEDURE sp_ActualizarSede
    @Id VARCHAR(20),
    @Nombre VARCHAR(200),
    @Direccion VARCHAR(300) = NULL,
    @Telefono VARCHAR(20) = NULL,
    @Email VARCHAR(150) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Sedes
    SET NOM_SEDE = @Nombre,
        DIRECCION = @Direccion,
        TELEFONO = @Telefono,
        EMAIL = @Email
    WHERE ID = @Id;
END
GO

-- ????????????????????????????????????????????????????????????????????????????
-- HORARIOS DOCTOR - CRUD
-- ????????????????????????????????????????????????????????????????????????????

-- SP: INSERTAR HORARIO DOCTOR
CREATE OR ALTER PROCEDURE sp_InsertarHorarioDoctor
    @IdDoctor VARCHAR(20),
    @DiaSemana VARCHAR(20),
    @HoraInicio TIME,
    @HoraFin TIME,
    @IdGenerado VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        EXEC GenerarIdHorarioDoctor @IdGenerado OUTPUT;
        
        INSERT INTO HorariosDoctor (ID, ID_DOC, DIA_SEMANA, HORA_INICIO, HORA_FIN)
        VALUES (@IdGenerado, @IdDoctor, @DiaSemana, @HoraInicio, @HoraFin);
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- SP: LISTAR HORARIOS POR DOCTOR
CREATE OR ALTER PROCEDURE sp_ListarHorariosPorDoctor
    @IdDoctor VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ID,
        DIA_SEMANA,
        HORA_INICIO,
        HORA_FIN
    FROM HorariosDoctor
    WHERE ID_DOC = @IdDoctor
    ORDER BY 
        CASE DIA_SEMANA
            WHEN 'Lunes' THEN 1
            WHEN 'Martes' THEN 2
            WHEN 'Miércoles' THEN 3
            WHEN 'Jueves' THEN 4
            WHEN 'Viernes' THEN 5
            WHEN 'Sábado' THEN 6
            WHEN 'Domingo' THEN 7
        END,
        HORA_INICIO;
END
GO

-- SP: ACTUALIZAR HORARIO DOCTOR
CREATE OR ALTER PROCEDURE sp_ActualizarHorarioDoctor
    @Id VARCHAR(20),
    @DiaSemana VARCHAR(20),
    @HoraInicio TIME,
    @HoraFin TIME
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE HorariosDoctor
    SET DIA_SEMANA = @DiaSemana,
        HORA_INICIO = @HoraInicio,
        HORA_FIN = @HoraFin
    WHERE ID = @Id;
END
GO

-- SP: ELIMINAR HORARIO DOCTOR
CREATE OR ALTER PROCEDURE sp_EliminarHorarioDoctor
    @Id VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM HorariosDoctor WHERE ID = @Id;
END
GO

PRINT '  ? SPs CRUD Especialidades, Sedes y Horarios creados';
GO
