-- =========================================================================
-- SCRIPT DE INSERCIÓN DE DATOS INICIALES - RED CLÍNICAS
-- Base de datos: RedCLinicas (Azure SQL Database)
-- Fecha: 15/11/2025
-- Descripción: Inserta datos base necesarios para el funcionamiento del sistema
-- =========================================================================

USE RedCLinicas;
GO

PRINT '=== INICIANDO INSERCIÓN DE DATOS BASE ===';
GO

-- =========================================================================
-- DECLARACIÓN DE VARIABLES PARA ALMACENAR LOS IDs ÚNICOS
-- =========================================================================
DECLARE @ID_ROL_ADM VARCHAR(20), @ID_ROL_MED VARCHAR(20), @ID_ROL_PAC VARCHAR(20);
DECLARE @ID_SEDE_CEN VARCHAR(20), @ID_SEDE_NOR VARCHAR(20), @ID_SEDE_SUR VARCHAR(20);
DECLARE @ID_ESP_CARDIO VARCHAR(20), @ID_ESP_PEDIA VARCHAR(20), @ID_ESP_DERMATO VARCHAR(20);
DECLARE @ID_ESP_TRAUMA VARCHAR(20), @ID_ESP_NEURO VARCHAR(20);
DECLARE @ID_USU_ADM VARCHAR(20), @ID_ADM VARCHAR(20);
DECLARE @ID_USU_DOC1 VARCHAR(20), @ID_DOC1 VARCHAR(20);
DECLARE @ID_USU_DOC2 VARCHAR(20), @ID_DOC2 VARCHAR(20);

-- =========================================================================
-- GENERAR TODOS LOS IDs DE CATÁLOGO Y USUARIOS
-- =========================================================================
PRINT '--- 1. Generando todos los IDs de catálogo... ---';

-- ROLES
EXEC GenerarIdRol @nuevoId = @ID_ROL_ADM OUTPUT;
EXEC GenerarIdRol @nuevoId = @ID_ROL_MED OUTPUT;
EXEC GenerarIdRol @nuevoId = @ID_ROL_PAC OUTPUT;

PRINT 'IDs de Roles generados:';
PRINT '  Administrador: ' + @ID_ROL_ADM;
PRINT '  Médico: ' + @ID_ROL_MED;
PRINT '  Paciente: ' + @ID_ROL_PAC;

-- ESPECIALIDADES
EXEC GenerarIdEspecialidad @nuevoId = @ID_ESP_CARDIO OUTPUT;
EXEC GenerarIdEspecialidad @nuevoId = @ID_ESP_PEDIA OUTPUT;
EXEC GenerarIdEspecialidad @nuevoId = @ID_ESP_DERMATO OUTPUT;
EXEC GenerarIdEspecialidad @nuevoId = @ID_ESP_TRAUMA OUTPUT;
EXEC GenerarIdEspecialidad @nuevoId = @ID_ESP_NEURO OUTPUT;

PRINT 'IDs de Especialidades generados';

-- SEDES
EXEC GenerarIdSede @nuevoId = @ID_SEDE_CEN OUTPUT;
EXEC GenerarIdSede @nuevoId = @ID_SEDE_NOR OUTPUT;
EXEC GenerarIdSede @nuevoId = @ID_SEDE_SUR OUTPUT;

PRINT 'IDs de Sedes generados';

-- USUARIOS
EXEC GenerarIdUsuario @nuevoId = @ID_USU_ADM OUTPUT;
EXEC GenerarIdUsuario @nuevoId = @ID_USU_DOC1 OUTPUT;
EXEC GenerarIdUsuario @nuevoId = @ID_USU_DOC2 OUTPUT;

-- REGISTROS ASOCIADOS
EXEC sp_GenerarIdAdministrador @nuevoId = @ID_ADM OUTPUT;
EXEC GenerarIdDoctor @nuevoId = @ID_DOC1 OUTPUT;
EXEC GenerarIdDoctor @nuevoId = @ID_DOC2 OUTPUT;

PRINT 'IDs de Usuarios y registros asociados generados';
GO

-- =========================================================================
-- INSERTAR DATOS EN TABLAS DE CATÁLOGO
-- =========================================================================
PRINT '--- 2. Insertando datos en tablas de catálogo... ---';

-- ROLES 
INSERT INTO Roles (ID, NOM_ROL)
VALUES 
    (@ID_ROL_ADM, 'Administrador'),
    (@ID_ROL_MED, 'Médico'),
    (@ID_ROL_PAC, 'Paciente');

PRINT '? Roles insertados (3)';

-- ESPECIALIDADES 
INSERT INTO Especialidades (ID, NOM_ESP, DES_ESP)
VALUES 
    (@ID_ESP_CARDIO, 'Cardiología', 'Especialidad dedicada al estudio, diagnóstico y tratamiento del corazón'),
    (@ID_ESP_PEDIA, 'Pediatría', 'Especialidad médica dedicada al cuidado de la salud infantil'),
    (@ID_ESP_DERMATO, 'Dermatología', 'Especialidad que trata enfermedades de la piel, cabello y uñas'),
    (@ID_ESP_TRAUMA, 'Traumatología', 'Especialidad que trata lesiones del sistema musculoesquelético'),
    (@ID_ESP_NEURO, 'Neurología', 'Especialidad que trata trastornos del sistema nervioso');

PRINT '? Especialidades insertadas (5)';

-- SEDES 
INSERT INTO Sedes (ID, NOM_SEDE, DIR_SEDE)
VALUES
    (@ID_SEDE_CEN, 'Sede Central', 'Av. Principal #1000, Lima Centro'),
    (@ID_SEDE_NOR, 'Sede Norte', 'Calle Girasoles #50, Los Olivos'),
    (@ID_SEDE_SUR, 'Sede Sur', 'Jr. Primavera #201, Miraflores');

PRINT '? Sedes insertadas (3)';

-- =========================================================================
-- INSERTAR USUARIOS Y REGISTROS RELACIONADOS
-- =========================================================================
PRINT '--- 3. Insertando usuarios del sistema... ---';

-- USUARIO ADMINISTRADOR
INSERT INTO Usuarios (ID, ID_ROL, TIPO_DOC, NUM_DOC, APE, NOM, GEN, EMAIL, PSWDHASH, SEDE_PREF, Activo)
VALUES (
    @ID_USU_ADM,
    @ID_ROL_ADM,
    'DNI',
    '87654321', 
    'Torres Ruiz',
    'Elena María',
    'F',
    'elena.torres@clinicaaguirre.com',
    CAST('AdminPassword2025!' AS VARBINARY(256)), -- En producción: usar hash real
    @ID_SEDE_CEN,
    1
);

-- REGISTRO EN TABLA ADMINISTRADORES
INSERT INTO Administradores (ID, ID_USU, NIVEL_ACCESO)
VALUES (@ID_ADM, @ID_USU_ADM, 3); -- Nivel 3 = Super Admin

PRINT '? Usuario Administrador creado: ' + @ID_USU_ADM;

-- USUARIO MÉDICO 1 - CARDIÓLOGO
INSERT INTO Usuarios (ID, ID_ROL, TIPO_DOC, NUM_DOC, APE, NOM, GEN, FEC_NAC, EMAIL, TEL, PSWDHASH, SEDE_PREF, Activo)
VALUES (
    @ID_USU_DOC1,
    @ID_ROL_MED,
    'DNI',
    '12345678', 
    'Mendoza Pérez',
    'Javier Alberto',
    'M',
    '1985-05-20',
    'javier.mendoza@clinicaaguirre.com',
    '987654321',
    CAST('DocPassword2025!' AS VARBINARY(256)), -- En producción: usar hash real
    @ID_SEDE_NOR,
    1
);

-- REGISTRO EN TABLA DOCTORES
INSERT INTO Doctores (ID, ID_USU, ID_ESP, COD_MED, DES_PRO, EXPE)
VALUES (
    @ID_DOC1,
    @ID_USU_DOC1,
    @ID_ESP_CARDIO,
    'CMP-12345',
    'Cardiólogo especialista en enfermedades cardiovasculares con más de 10 años de experiencia',
    10
);

PRINT '? Usuario Médico 1 (Cardiólogo) creado: ' + @ID_USU_DOC1;

-- USUARIO MÉDICO 2 - PEDIATRA
INSERT INTO Usuarios (ID, ID_ROL, TIPO_DOC, NUM_DOC, APE, NOM, GEN, FEC_NAC, EMAIL, TEL, PSWDHASH, SEDE_PREF, Activo)
VALUES (
    @ID_USU_DOC2,
    @ID_ROL_MED,
    'DNI',
    '23456789', 
    'Ramírez Castro',
    'Ana Lucía',
    'F',
    '1990-08-15',
    'ana.ramirez@clinicaaguirre.com',
    '987654322',
    CAST('DocPassword2025!' AS VARBINARY(256)), -- En producción: usar hash real
    @ID_SEDE_CEN,
    1
);

-- REGISTRO EN TABLA DOCTORES
INSERT INTO Doctores (ID, ID_USU, ID_ESP, COD_MED, DES_PRO, EXPE)
VALUES (
    @ID_DOC2,
    @ID_USU_DOC2,
    @ID_ESP_PEDIA,
    'CMP-23456',
    'Pediatra especialista en el cuidado integral de niños y adolescentes',
    6
);

PRINT '? Usuario Médico 2 (Pediatra) creado: ' + @ID_USU_DOC2;

-- =========================================================================
-- INSERTAR HORARIOS DE LOS DOCTORES
-- =========================================================================
PRINT '--- 4. Insertando horarios de doctores... ---';

DECLARE @ID_HOR1 VARCHAR(20), @ID_HOR2 VARCHAR(20), @ID_HOR3 VARCHAR(20), @ID_HOR4 VARCHAR(20);

-- Generar IDs para horarios
EXEC GenerarIdHorarioDoctor @nuevoId = @ID_HOR1 OUTPUT;
EXEC GenerarIdHorarioDoctor @nuevoId = @ID_HOR2 OUTPUT;
EXEC GenerarIdHorarioDoctor @nuevoId = @ID_HOR3 OUTPUT;
EXEC GenerarIdHorarioDoctor @nuevoId = @ID_HOR4 OUTPUT;

-- Horarios Doctor 1 (Cardiólogo) - Sede Norte
INSERT INTO HorariosDoctor (ID, ID_DOC, ID_SEDE, DIA_SEM, HORA, HORA_FIN)
VALUES 
    (@ID_HOR1, @ID_DOC1, @ID_SEDE_NOR, 1, '09:00:00', '13:00:00'), -- Lunes
    (@ID_HOR2, @ID_DOC1, @ID_SEDE_NOR, 3, '14:00:00', '18:00:00'); -- Miércoles

-- Horarios Doctor 2 (Pediatra) - Sede Central
INSERT INTO HorariosDoctor (ID, ID_DOC, ID_SEDE, DIA_SEM, HORA, HORA_FIN)
VALUES 
    (@ID_HOR3, @ID_DOC2, @ID_SEDE_CEN, 2, '08:00:00', '12:00:00'), -- Martes
    (@ID_HOR4, @ID_DOC2, @ID_SEDE_CEN, 4, '15:00:00', '19:00:00'); -- Jueves

PRINT '? Horarios de doctores insertados (4)';

-- =========================================================================
-- RESUMEN DE INSERCIÓN
-- =========================================================================
PRINT '';
PRINT '=== RESUMEN DE DATOS INSERTADOS ===';
PRINT 'Roles: 3 (Administrador, Médico, Paciente)';
PRINT 'Especialidades: 5 (Cardiología, Pediatría, Dermatología, Traumatología, Neurología)';
PRINT 'Sedes: 3 (Central, Norte, Sur)';
PRINT 'Usuarios: 3 (1 Admin + 2 Médicos)';
PRINT 'Administradores: 1';
PRINT 'Doctores: 2';
PRINT 'Horarios: 4';
PRINT '';
PRINT '=== CREDENCIALES DE ACCESO (SOLO DESARROLLO) ===';
PRINT 'ADMINISTRADOR:';
PRINT '  Email: elena.torres@clinicaaguirre.com';
PRINT '  Password: AdminPassword2025!';
PRINT '';
PRINT 'MÉDICO 1 (Cardiólogo):';
PRINT '  Email: javier.mendoza@clinicaaguirre.com';
PRINT '  Password: DocPassword2025!';
PRINT '';
PRINT 'MÉDICO 2 (Pediatra):';
PRINT '  Email: ana.ramirez@clinicaaguirre.com';
PRINT '  Password: DocPassword2025!';
PRINT '';
PRINT '?? IMPORTANTE: En producción, las contraseñas deben estar hasheadas con algoritmos seguros (BCrypt, Argon2, etc.)';
PRINT '=== INSERCIÓN DE DATOS BASE COMPLETADA EXITOSAMENTE ===';
GO
