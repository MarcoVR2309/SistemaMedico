-- =========================================================================
-- SCRIPT DE SECUENCIAS Y GENERADORES DE IDs - RED CLÍNICAS
-- Base de datos: RedCLinicas (Azure SQL Database)
-- Fecha: 15/11/2025
-- =========================================================================

USE RedCLinicas;
GO

PRINT 'Creando secuencias en RedCLinicas...';
GO

-- =========================================================================
-- SECUENCIAS PARA GENERACIÓN AUTOMÁTICA DE IDs
-- =========================================================================

-- SECUENCIA PARA USUARIOS (U0000001, U0000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_Usuarios')
BEGIN
    CREATE SEQUENCE seq_Usuarios START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_Usuarios creada';
END
GO

-- SECUENCIA PARA PACIENTES (P0000001, P0000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_Pacientes')
BEGIN
    CREATE SEQUENCE seq_Pacientes START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_Pacientes creada';
END
GO

-- SECUENCIA PARA DOCTORES (D0000001, D0000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_Doctores')
BEGIN
    CREATE SEQUENCE seq_Doctores START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_Doctores creada';
END
GO

-- SECUENCIA PARA ADMINISTRADORES (A0000001, A0000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_Administradores')
BEGIN
    CREATE SEQUENCE seq_Administradores START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_Administradores creada';
END
GO

-- SECUENCIA PARA CITAS (C0000001, C0000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_Citas')
BEGIN
    CREATE SEQUENCE seq_Citas START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_Citas creada';
END
GO

-- SECUENCIA PARA CONSULTAS MEDICAS (CM000001, CM000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_ConsultasMedicas')
BEGIN
    CREATE SEQUENCE seq_ConsultasMedicas START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_ConsultasMedicas creada';
END
GO

-- SECUENCIA PARA RECETAS MEDICAS (RM000001, RM000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_RecetasMedicas')
BEGIN
    CREATE SEQUENCE seq_RecetasMedicas START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_RecetasMedicas creada';
END
GO

-- SECUENCIA PARA RECETAS DETALLE (RD000001, RD000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_RecetasDetalle')
BEGIN
    CREATE SEQUENCE seq_RecetasDetalle START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_RecetasDetalle creada';
END
GO

-- SECUENCIA PARA PAGOS (PG000001, PG000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_Pagos')
BEGIN
    CREATE SEQUENCE seq_Pagos START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_Pagos creada';
END
GO

-- SECUENCIA PARA HORARIOS DOCTOR (HD000001, HD000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_HorariosDoctor')
BEGIN
    CREATE SEQUENCE seq_HorariosDoctor START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_HorariosDoctor creada';
END
GO

-- SECUENCIA PARA ROLES (R0000001, R0000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_Roles')
BEGIN
    CREATE SEQUENCE seq_Roles START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_Roles creada';
END
GO

-- SECUENCIA PARA SEDES (S0000001, S0000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_Sedes')
BEGIN
    CREATE SEQUENCE seq_Sedes START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_Sedes creada';
END
GO

-- SECUENCIA PARA ESPECIALIDADES (E0000001, E0000002, ...)
IF NOT EXISTS (SELECT * FROM sys.sequences WHERE name = 'seq_Especialidades')
BEGIN
    CREATE SEQUENCE seq_Especialidades START WITH 1 INCREMENT BY 1;
    PRINT 'Secuencia seq_Especialidades creada';
END
GO

PRINT '13 secuencias creadas correctamente';
GO

-- =========================================================================
-- STORED PROCEDURES GENERADORES DE IDs
-- =========================================================================

PRINT 'Creando Stored Procedures generadores...';
GO

-- SP: GENERAR ID USUARIO (U0000001)
CREATE OR ALTER PROCEDURE GenerarIdUsuario
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_Usuarios;
    SET @nuevoId = 'U' + RIGHT('0000000' + CAST(@numero AS VARCHAR(7)), 7);
END
GO

-- SP: GENERAR ID PACIENTE (P0000001)
CREATE OR ALTER PROCEDURE GenerarIdPaciente
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_Pacientes;
    SET @nuevoId = 'P' + RIGHT('0000000' + CAST(@numero AS VARCHAR(7)), 7);
END
GO

-- SP: GENERAR ID DOCTOR (D0000001)
CREATE OR ALTER PROCEDURE GenerarIdDoctor
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_Doctores;
    SET @nuevoId = 'D' + RIGHT('0000000' + CAST(@numero AS VARCHAR(7)), 7);
END
GO

-- SP: GENERAR ID ADMINISTRADOR (A0000001)
CREATE OR ALTER PROCEDURE sp_GenerarIdAdministrador
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_Administradores;
    SET @nuevoId = 'A' + RIGHT('0000000' + CAST(@numero AS VARCHAR(7)), 7);
END
GO

-- SP: GENERAR ID CITA (C0000001)
CREATE OR ALTER PROCEDURE GenerarIdCita
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_Citas;
    SET @nuevoId = 'C' + RIGHT('0000000' + CAST(@numero AS VARCHAR(7)), 7);
END
GO

-- SP: GENERAR ID CONSULTA MEDICA (CM000001)
CREATE OR ALTER PROCEDURE GenerarIdConsultaMedica
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_ConsultasMedicas;
    SET @nuevoId = 'CM' + RIGHT('000000' + CAST(@numero AS VARCHAR(6)), 6);
END
GO

-- SP: GENERAR ID RECETA MEDICA (RM000001)
CREATE OR ALTER PROCEDURE GenerarIdRecetaMedica
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_RecetasMedicas;
    SET @nuevoId = 'RM' + RIGHT('000000' + CAST(@numero AS VARCHAR(6)), 6);
END
GO

-- SP: GENERAR ID RECETA DETALLE (RD000001)
CREATE OR ALTER PROCEDURE GenerarIdRecetaDetalle
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_RecetasDetalle;
    SET @nuevoId = 'RD' + RIGHT('000000' + CAST(@numero AS VARCHAR(6)), 6);
END
GO

-- SP: GENERAR ID PAGO (PG000001)
CREATE OR ALTER PROCEDURE GenerarIdPago
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_Pagos;
    SET @nuevoId = 'PG' + RIGHT('000000' + CAST(@numero AS VARCHAR(6)), 6);
END
GO

-- SP: GENERAR ID HORARIO DOCTOR (HD000001)
CREATE OR ALTER PROCEDURE GenerarIdHorarioDoctor
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_HorariosDoctor;
    SET @nuevoId = 'HD' + RIGHT('000000' + CAST(@numero AS VARCHAR(6)), 6);
END
GO

-- SP: GENERAR ID ROL (R0000001)
CREATE OR ALTER PROCEDURE GenerarIdRol
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_Roles;
    SET @nuevoId = 'R' + RIGHT('0000000' + CAST(@numero AS VARCHAR(7)), 7);
END
GO

-- SP: GENERAR ID SEDE (S0000001)
CREATE OR ALTER PROCEDURE GenerarIdSede
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_Sedes;
    SET @nuevoId = 'S' + RIGHT('0000000' + CAST(@numero AS VARCHAR(7)), 7);
END
GO

-- SP: GENERAR ID ESPECIALIDAD (E0000001)
CREATE OR ALTER PROCEDURE GenerarIdEspecialidad
    @nuevoId VARCHAR(20) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @numero INT;
    SET @numero = NEXT VALUE FOR seq_Especialidades;
    SET @nuevoId = 'E' + RIGHT('0000000' + CAST(@numero AS VARCHAR(7)), 7);
END
GO

PRINT '13 Stored Procedures generadores creados correctamente';
PRINT 'Sistema de IDs personalizados completado';
GO
