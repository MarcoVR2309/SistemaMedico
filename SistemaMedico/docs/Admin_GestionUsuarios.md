# Módulo Administrador - Gestión de Usuarios

Este documento describe los componentes técnicos y pasos de despliegue del nuevo módulo de **Gestión de Cuentas** para administradores. Incluye los artefactos SQL, lógica en C# y la interfaz Web Forms implementada.

---

## Resumen funcional

- Panel principal con métricas (total usuarios, doctores, pacientes, activos).
- Búsqueda y filtrado por texto, rol y estado.
- CRUD completo de usuarios:
  - Crear usuarios por rol (Administrador, Doctor, Paciente).
  - Editar datos personales y específicos del rol.
  - Activar / desactivar cuentas.
  - Resetear contraseñas con generación de hash SHA-256.
- Soporte para datos extendidos:
  - **Doctor**: especialidad, colegiatura, años de experiencia.
  - **Paciente**: peso y fecha de referencia (edad aproximada).

---

## Componentes SQL

Archivo principal: `database/SP_Admin_GestionUsuarios.sql`

Stored procedures creados:

| Stored Procedure | Descripción |
|------------------|-------------|
| `sp_Admin_ListarUsuarios` | Devuelve métricas globales y listado filtrable de usuarios. |
| `sp_Admin_ObtenerUsuarioDetalle` | Recupera información completa para cargar el formulario de edición. |
| `sp_Admin_CrearUsuario` | Inserta usuario y crea registro en `Doctores` o `Pacientes` según rol. |
| `sp_Admin_ActualizarUsuario` | Actualiza datos base y registros relacionados en una transacción. |
| `sp_Admin_CambiarEstadoUsuario` | Activa o desactiva un usuario (flag `Activo`). |
| `sp_Admin_ResetearPasswordUsuario` | Reemplaza el hash de contraseña con SHA-256. |

Script auxiliar: `database/Admin_CrearUsuarioAdmin.sql`

- Verifica la existencia del rol **Administrador**.
- Evita duplicar el correo `admin@meditech.com`.
- Crea un usuario administrador con contraseña temporal `Admin123!`.

> **Recomendación:** ejecutar ambos archivos en la base **RedCLinicas** antes de publicar el módulo.

---

## Capa de datos en C#

Ubicación: `datos/AdminUsuariosDAO.cs`

Métodos expuestos:

- `ListarUsuarios(...)`
- `ObtenerDetalle(idUsuario)`
- `CrearUsuario(...)`
- `ActualizarUsuario(...)`
- `CambiarEstado(idUsuario, activar)`
- `ResetearPassword(idUsuario, nuevaClave)`

Objetos de transferencia:

- `modelo/AdminUsuarioItem.cs`
- `modelo/AdminUsuarioDetalle.cs`
- `modelo/AdminUsuarioResumen.cs`
- `modelo/AdminUsuariosLista.cs`

Extensión adicional:

- `datos/RolesDAO.cs` ahora expone `ListarRoles()` para poblar filtros y formularios.

---

## Interfaz Web Forms

Archivos clave:

| Archivo | Descripción |
|---------|-------------|
| `vista/admin/GestionUsuarios.aspx` | Vista con cards de métricas, filtros y tabla de usuarios. |
| `vista/admin/GestionUsuarios.aspx.cs` | Code-behind que conecta la UI con `AdminUsuariosDAO`. |
| `vista/admin/GestionUsuarios.aspx.designer.cs` | Declaración de controles. |
| `styles/admin-usuarios.css` | Estilos específicos del módulo siguiendo el diseño de Figma. |
| `scripts/admin-usuarios.js` | Control ligero de modales (abrir/cerrar). |

Ajustes adicionales:

- `vista/Login.aspx.cs`: ahora redirige a `~/vista/admin/GestionUsuarios.aspx` cuando el rol es **Administrador**.

---

## Pasos de despliegue

1. **Restaurar paquetes NuGet** (si se trabaja en un entorno nuevo).  
2. **Ejecutar los stored procedures** en la base:
   ```sql
   :r .\database\SP_Admin_GestionUsuarios.sql
   :r .\database\Admin_CrearUsuarioAdmin.sql
   ```
3. **Crear usuario administrador** (si no existe) usando el script auxiliar.
4. **Publicar la web** o ejecutar desde Visual Studio en IIS Express.
5. **Probar credenciales**:  
   - Email: `admin@meditech.com`  
   - Password temporal: `Admin123!`  
   - Cambiarla tras el primer inicio según política interna.

---

## Pruebas recomendadas

- Listar usuarios en distintos filtros (rol, estado, texto).
- Crear usuarios para cada rol y verificar inserciones relacionadas.
- Editar un usuario doctor → cambiar especialidad y años de experiencia.
- Activar / desactivar usuarios y validar el estado en la base.
- Resetear contraseña y confirmar acceso con la nueva credencial.
- Revisar mensajes y modales de error/éxito en la UI.

---

## Rama de trabajo

- Cambios desarrollados en la rama `aguilar`.
- Se sugiere merge request desde `aguilar` contra `main` después de validar en QA.

---

## Referencias

- Diseño UI: [Figma - Sistema de Gestión de Pacientes](https://www.figma.com/make/9JmtgM4fY8Byqa0i2eND49/Sistema-de-Gestión-de-Pacientes?node-id=0-1)
- Documentación adicional: `docs/ConectarSSMSaAzure.md`

