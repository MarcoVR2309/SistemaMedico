# Conectar SSMS a Azure SQL Database

## Pasos para Conectar

1. **Abrir SQL Server Management Studio (SSMS)**

2. **En la ventana de conexión, configurar:**

```
Tipo de servidor: Motor de base de datos
Nombre del servidor: clinica-aguirre-pe.database.windows.net
Autenticación: Autenticación de SQL Server
Usuario: adminClinica
Contraseña: ClinicaAguirre2025
```

3. **Opciones avanzadas (Click en "Opciones >>")**
   - Pestaña "Propiedades de conexión"
   - Conectarse a la base de datos: RedCLinicas
   - Cifrar conexión: Activado

4. **Click en "Conectar"**

## Ejecutar tu Script

1. Una vez conectado, click en **"Nueva consulta"**
2. Abrir tu archivo `.sql` con SSMS: `Archivo > Abrir > Archivo`
3. Seleccionar tu script de creación
4. Asegurarte que está seleccionada la BD `RedCLinicas` (arriba)
5. Click en **"Ejecutar"** (F5)

## Verificar Tablas Creadas

```sql
-- Ver todas las tablas
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Ver estructura de una tabla específica
EXEC sp_help 'Usuarios';
```

## Errores Comunes

### Error: Login failed
- Verificar usuario y contraseña
- Verificar que tu IP esté en el firewall de Azure

### Error: Firewall blocked
- Ir a Azure Portal > Tu servidor > Networking
- Agregar tu IP actual en "Firewall rules"

### Error: Cannot open database
- Verificar que el nombre de BD sea exactamente: `RedCLinicas` (con C mayúscula)
