# ?? Database Scripts - Sistema Médico Clínica Aguirre

Conjunto de scripts SQL para la configuración completa de la base de datos **RedCLinicas** en Azure SQL Database.

---

## ?? Estructura de Archivos

Los scripts deben ejecutarse en el siguiente orden:

| #  | Archivo | Descripción | Tiempo Aprox. |
|----|---------|-------------|---------------|
| 00 | `00_MASTER_SETUP.sql` | **Script maestro** - Ejecuta todos los demás en orden | 5-10 min |
| 01 | `01_CREATE_TABLES.sql` | Creación de todas las tablas del sistema | 2-3 min |
| 02 | `02_CREATE_SEQUENCES.sql` | Secuencias y generadores de IDs automáticos | 1-2 min |
| 03 | `03_INSERT_INITIAL_DATA.sql` | Datos iniciales (roles, sedes, usuarios base) | 1 min |

---

## ?? Guía de Uso

### **Opción 1: Ejecución Automática (Recomendada)**

Ejecuta el script maestro que se encarga de todo:

```sql
-- En SSMS, conectado a Azure SQL Database
USE RedCLinicas;
GO

-- Ejecutar el script maestro
:r 00_MASTER_SETUP.sql
```

### **Opción 2: Ejecución Manual**

Si prefieres ejecutar paso por paso:

```sql
-- 1. Crear tablas
:r 01_CREATE_TABLES.sql

-- 2. Crear secuencias y generadores
:r 02_CREATE_SEQUENCES.sql

-- 3. Insertar datos iniciales
:r 03_INSERT_INITIAL_DATA.sql
```

---

## ?? Conexión a Azure SQL Database

### **Datos de Conexión:**

```
Servidor: clinica-aguirre-pe.database.windows.net
Base de datos: RedCLinicas
Autenticación: SQL Server Authentication
Usuario: adminClinica
Contraseña: [Consultar con el equipo]
```

### **Desde SSMS:**

1. Abrir SQL Server Management Studio
2. Conectar con:
   - Tipo de servidor: **Motor de base de datos**
   - Nombre del servidor: `clinica-aguirre-pe.database.windows.net`
   - Autenticación: **Autenticación de SQL Server**
   - Usuario: `adminClinica`
   - Contraseña: `ClinicaAguirre2025`
3. Opciones >> Propiedades de conexión
   - Conectarse a la base de datos: `RedCLinicas`
   - Cifrar conexión: **Activado**

?? **Guía detallada:** Ver [`docs/ConectarSSMSaAzure.md`](../docs/ConectarSSMSaAzure.md)

---

## ?? Estructura de la Base de Datos

### **Tablas Principales**

```
Catálogos:
??? Roles              (Tipos de usuario: Admin, Médico, Paciente)
??? Sedes              (Sucursales de la clínica)
??? Especialidades     (Especialidades médicas)

Usuarios:
??? Usuarios           (Tabla base de todos los usuarios)
??? Pacientes          (Datos específicos de pacientes)
??? Doctores           (Datos específicos de médicos)
??? Administradores    (Datos específicos de administradores)

Operaciones:
??? Citas              (Reservas de citas médicas)
??? HorariosDoctor     (Disponibilidad de médicos)
??? ConsultasMedicas   (Registro de consultas)
??? RecetasMedicas     (Recetas emitidas)
??? RecetasDetalle     (Detalle de medicamentos)
??? Pagos              (Registro de transacciones)
```

### **Sistema de IDs Personalizados**

Cada tabla tiene un formato de ID único:

| Tabla | Formato | Ejemplo |
|-------|---------|---------|
| Usuarios | U0000001 | U0000001, U0000002, ... |
| Pacientes | P0000001 | P0000001, P0000002, ... |
| Doctores | D0000001 | D0000001, D0000002, ... |
| Administradores | A0000001 | A0000001, A0000002, ... |
| Citas | C0000001 | C0000001, C0000002, ... |
| ConsultasMedicas | CM000001 | CM000001, CM000002, ... |
| RecetasMedicas | RM000001 | RM000001, RM000002, ... |
| Pagos | PG000001 | PG000001, PG000002, ... |

**Generación automática:**
```sql
-- Ejemplo de uso
DECLARE @nuevoId VARCHAR(20);
EXEC GenerarIdUsuario @nuevoId OUTPUT;
SELECT @nuevoId; -- Retorna: U0000001
```

---

## ?? Usuarios Iniciales

Después de ejecutar los scripts, se crean estos usuarios de prueba:

### **Administrador**
```
Email: elena.torres@clinicaaguirre.com
Password: AdminPassword2025!
Rol: Administrador (Nivel 3 - Super Admin)
Sede: Central
```

### **Médico 1 - Cardiólogo**
```
Email: javier.mendoza@clinicaaguirre.com
Password: DocPassword2025!
Rol: Médico
Especialidad: Cardiología
Código Médico: CMP-12345
Sede: Norte
```

### **Médico 2 - Pediatra**
```
Email: ana.ramirez@clinicaaguirre.com
Password: DocPassword2025!
Rol: Médico
Especialidad: Pediatría
Código Médico: CMP-23456
Sede: Central
```

?? **IMPORTANTE:** Estas contraseñas son solo para desarrollo. En producción deben estar hasheadas.

---

## ?? Stored Procedures CRUD

Además de los scripts de configuración inicial, el sistema incluye SPs para operaciones CRUD:

- `SP_CRUD_Citas.sql` - Operaciones con citas
- `SP_CRUD_ConsultasMedicas.sql` - Gestión de consultas
- `SP_CRUD_Doctores.sql` - Administración de médicos
- `SP_CRUD_Especialidades_Sedes_Horarios.sql` - Catálogos y horarios
- `SP_CRUD_Pagos_Administradores.sql` - Pagos y administradores
- `SP_CRUD_RecetasMedicas.sql` - Gestión de recetas
- `SP_CRUD_Roles.sql` - Administración de roles

---

## ?? Verificación Post-Instalación

Después de ejecutar los scripts, verifica:

### **1. Tablas creadas**
```sql
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

### **2. Secuencias activas**
```sql
SELECT name, current_value 
FROM sys.sequences
ORDER BY name;
```

### **3. Stored Procedures**
```sql
SELECT ROUTINE_NAME 
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_TYPE = 'PROCEDURE'
ORDER BY ROUTINE_NAME;
```

### **4. Datos iniciales**
```sql
-- Roles
SELECT * FROM Roles;

-- Especialidades
SELECT * FROM Especialidades;

-- Sedes
SELECT * FROM Sedes;

-- Usuarios
SELECT U.ID, U.NOM, U.APE, U.EMAIL, R.NOM_ROL
FROM Usuarios U
INNER JOIN Roles R ON U.ID_ROL = R.ID;

-- Doctores con especialidad
SELECT D.COD_MED, U.NOM, U.APE, E.NOM_ESP
FROM Doctores D
INNER JOIN Usuarios U ON D.ID_USU = U.ID
INNER JOIN Especialidades E ON D.ID_ESP = E.ID;
```

---

## ?? Notas Importantes

### **Seguridad**
- ? Nunca subas contraseñas reales al repositorio
- ? Usa variables de entorno o Azure Key Vault en producción
- ? Las contraseñas deben estar hasheadas (BCrypt, Argon2, etc.)

### **Azure SQL Database**
- Firewall debe permitir tu IP para conectar desde SSMS
- Conexiones deben usar TLS/SSL (ya configurado por defecto)
- Verifica que el App Service tenga acceso a la BD

### **Respaldos**
- Azure SQL realiza respaldos automáticos diariamente
- Puedes restaurar a cualquier punto en el tiempo de los últimos 7-35 días
- Para exportar manualmente: SSMS ? Tareas ? Exportar aplicación de capa de datos

---

## ?? Solución de Problemas

### **Error: Login failed**
- Verifica usuario y contraseña
- Confirma que tu IP está en el firewall de Azure

### **Error: Cannot open database**
- Verifica el nombre exacto: `RedCLinicas` (con C mayúscula)
- Selecciona la BD correcta antes de ejecutar scripts

### **Error: Object already exists**
- Los scripts usan `IF NOT EXISTS` para evitar duplicados
- Si necesitas recrear, elimina primero las tablas en orden inverso

### **Error: Secuencias no generan IDs**
- Verifica que las secuencias existan: `SELECT * FROM sys.sequences`
- Reinicia la secuencia si es necesario:
  ```sql
  ALTER SEQUENCE seq_Usuarios RESTART WITH 1;
  ```

---

## ?? Soporte

Para problemas con la base de datos:
1. Revisa los logs de ejecución en SSMS
2. Consulta la documentación en `/docs/`
3. Contacta al equipo de desarrollo

---

**Última actualización:** 15 de noviembre de 2025  
**Versión de BD:** 1.0.0  
**Compatibilidad:** Azure SQL Database
