# ?? Guía de Despliegue Automático con GitHub Actions

Este proyecto está configurado con **CI/CD automático** que despliega la aplicación a Azure App Service cada vez que se hace push a la rama `main`.

## ?? Configuración Actual

### **Servicio de Hosting**
- **Plataforma:** Azure App Service (Windows)
- **Nombre:** `SistemaClinica-webapp`
- **URL:** https://sistemaclinica-webapp-ecbfudhb4g7d7fu.brazilsouth-01.azurewebsites.net
- **Región:** Brazil South
- **Plan:** Free Tier / Basic

### **Base de Datos**
- **Servidor:** `clinica-aguirre-pe.database.windows.net`
- **Base de Datos:** `RedCLinicas`
- **Autenticación:** SQL Server Authentication

---

## ?? Flujo de Despliegue Automático

Cada vez que haces `push` a la rama `main`, GitHub Actions automáticamente:

```
1. ?? Detecta cambios en la rama main
   ?
2. ??? Compila el proyecto ASP.NET
   ?
3. ?? Restaura paquetes NuGet
   ?
4. ?? Construye la solución en modo Release
   ?
5. ?? Sube el artefacto compilado
   ?
6. ?? Se autentica en Azure (Managed Identity)
   ?
7. ?? Despliega a Azure App Service
   ?
8. ? Aplicación actualizada en 3-5 minutos
```

---

## ?? Cómo Hacer un Despliegue

### **Método 1: Push normal (Recomendado)**

```bash
# 1. Haz tus cambios en el código
# 2. Agrega los archivos modificados
git add .

# 3. Haz commit con un mensaje descriptivo
git commit -m "Descripción de los cambios"

# 4. Sube a GitHub (esto activará el despliegue automático)
git push origin main
```

### **Método 2: Despliegue manual**

Si quieres desplegar sin hacer cambios:

1. Ve a tu repositorio en GitHub
2. Click en **Actions** ? **Build and deploy ASP app to Azure Web App**
3. Click en **Run workflow**
4. Selecciona la rama `main`
5. Click en **Run workflow**

---

## ?? Monitorear el Despliegue

### **En GitHub:**
1. Ve a: https://github.com/MarcoVR2309/SistemaMedico/actions
2. Verás el estado del workflow:
   - ?? **Amarillo** = En progreso
   - ? **Verde** = Exitoso
   - ? **Rojo** = Falló

3. Click en el workflow para ver los logs detallados

### **En Azure Portal:**
1. Ve a: https://portal.azure.com
2. Busca tu App Service: `SistemaClinica-webapp`
3. En el menú lateral: **Deployment Center** ? **Logs**
4. Verás el historial de despliegues

---

## ??? Archivo de Workflow

El archivo de configuración está en:
```
.github/workflows/main_sistemaclinica-webapp.yml
```

### **Características principales:**

- **Trigger:** Push a rama `main` o ejecución manual
- **Build:** Windows Server, MSBuild, NuGet
- **Deploy:** Azure Web Apps Deploy con Managed Identity
- **Autenticación:** OIDC (OpenID Connect) - Sin contraseñas expuestas

---

## ?? Configuración de Secretos

Los siguientes secretos están configurados automáticamente en GitHub:

| Secreto | Descripción |
|---------|-------------|
| `AZUREAPPSERVICE_CLIENTID_*` | ID del cliente para autenticación |
| `AZUREAPPSERVICE_TENANTID_*` | ID del tenant de Azure |
| `AZUREAPPSERVICE_SUBSCRIPTIONID_*` | ID de la suscripción |

?? **No modifiques estos secretos manualmente.** Azure los gestiona automáticamente.

---

## ?? Solución de Problemas

### **Error: Build Failed**

```bash
# Verifica que la solución compile localmente
msbuild SistemaMedico/SistemaMedico.csproj /p:Configuration=Release
```

Si compila localmente pero falla en GitHub Actions:
1. Verifica que todos los paquetes NuGet estén en el repositorio
2. Revisa los logs del workflow en GitHub Actions

### **Error: Deploy Failed**

1. Ve a Azure Portal ? Tu App Service ? **Diagnose and solve problems**
2. Revisa los logs en **Deployment Center** ? **Logs**
3. Verifica que la cadena de conexión esté configurada correctamente

### **Error: Cannot connect to database**

1. Ve a Azure Portal ? Tu SQL Server
2. **Networking** ? **Firewall rules**
3. Asegúrate de que "Allow Azure services" esté habilitado

---

## ?? Seguridad

### **Mejores Prácticas Implementadas:**

? **Managed Identity** - Sin contraseñas en el código
? **OIDC Authentication** - Tokens de corta duración
? **Secretos en GitHub Secrets** - No expuestos en el repositorio
? **HTTPS Only** - Toda la comunicación encriptada

### **Mejoras Recomendadas:**

1. **Eliminar contraseña del Web.config:**
   - Usar Azure Key Vault para almacenar credenciales
   - Configurar la cadena de conexión en Azure Portal

2. **Configurar Environments en GitHub:**
   - Agregar aprobaciones manuales para producción
   - Separar ambientes de staging y producción

---

## ?? Métricas de Despliegue

### **Tiempo promedio de despliegue:** 3-5 minutos

| Fase | Tiempo |
|------|--------|
| Build | ~1-2 min |
| Upload Artifact | ~30 seg |
| Deploy | ~1-2 min |
| **Total** | **~3-5 min** |

---

## ?? Soporte

Si encuentras problemas con el despliegue:

1. **Revisa los logs en GitHub Actions**
2. **Verifica el estado en Azure Portal**
3. **Contacta al equipo de desarrollo**

---

## ?? Referencias

- [Azure Web Apps Deploy Action](https://github.com/Azure/webapps-deploy)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)

---

**Última actualización:** 14 de noviembre de 2025  
**Configurado por:** Equipo de Desarrollo - Clínica Aguirre
