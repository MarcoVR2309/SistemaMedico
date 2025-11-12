-- ================================================================================
-- Script: Admin_CrearUsuarioAdmin.sql
-- Propósito: Crear un usuario administrador por defecto en la base RedCLinicas
-- Cómo usar:
--   1. Conectarse a la base de datos RedCLinicas (Azure SQL / SQL Server)
--   2. Ejecutar este script completo
--   3. Confirmar el ID generado y usar las credenciales indicadas
-- ================================================================================

-- Cambiar si la base tiene otro nombre
USE RedCLinicas;
GO

PRINT 'Creando usuario administrador por defecto...';
GO

DECLARE @IdRolAdmin VARCHAR(20);
DECLARE @IdGenerado VARCHAR(20);

-- 1. Buscar el ID del rol Administrador
SELECT @IdRolAdmin = ID
FROM Roles
WHERE NOM_ROL = 'Administrador';

IF @IdRolAdmin IS NULL
BEGIN
    THROW 50010, 'No existe un rol con nombre Administrador. Crear el rol antes de ejecutar este script.', 1;
END;

-- 2. Validar si ya existe un usuario con el mismo email
IF EXISTS (SELECT 1 FROM Usuarios WHERE EMAIL = 'admin@meditech.com')
BEGIN
    PRINT 'Ya existe un usuario con el correo admin@meditech.com. No se creó un registro nuevo.';
    SELECT ID, EMAIL, ID_ROL, FechaCreacion FROM Usuarios WHERE EMAIL = 'admin@meditech.com';
    RETURN;
END;

-- 3. Crear el usuario usando el SP oficial
EXEC sp_InsertarUsuario
    @IdRol         = @IdRolAdmin,
    @TipoDoc       = 'DNI',
    @NumDoc        = '00000000',
    @Apellido      = 'Principal',
    @Nombre        = 'Admin',
    @Genero        = 'M',
    @FechaNac      = '1990-01-01',
    @Email         = 'admin@meditech.com',
    @Telefono      = '987654321',
    @PasswordHash  = HASHBYTES('SHA2_256', 'Admin123!'),
    @SedePref      = NULL,
    @IdGenerado    = @IdGenerado OUTPUT;

PRINT 'Usuario administrador creado correctamente.';
SELECT
    @IdGenerado AS IdUsuario,
    'admin@meditech.com' AS Email,
    'Admin123!' AS PasswordTemporal;
GO

PRINT 'Inicia sesión en la aplicación con las credenciales anteriores y cambia la contraseña después.';
GO

