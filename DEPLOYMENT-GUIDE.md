# ?? DEPLOYMENT CHECKLIST - Panel Paciente Responsive

## ?? **CAMBIOS REALIZADOS**

### **CSS (`panelpaciente.css`):**
- ? Botón hamburguesa más grande (50x50px)
- ? Z-index aumentado a 3000
- ? Mejor área táctil con `touch-action: manipulation`
- ? Eliminar highlight táctil con `-webkit-tap-highlight-color`
- ? Overlay oscuro al abrir menú
- ? Espacio para botón en el header (padding-left: 80px)
- ? Animaciones suaves

### **JavaScript (`panelpaciente.js`):**
- ? Debounce para evitar clics múltiples (300ms)
- ? Creación automática de overlay
- ? Cierre con tecla ESC
- ? Cierre al hacer clic fuera del sidebar
- ? Prevención de scroll cuando menú está abierto
- ? `stopPropagation()` para evitar conflictos de eventos

### **HTML (`PanelPaciente.aspx`):**
- ? Meta tags de cache control
- ? Versiones actualizadas: `?v=2025012501`

---

## ?? **PASOS PARA DEPLOYMENT**

### **1. SI USAS IIS (Windows Server):**

```powershell
# 1. Compilar el proyecto
dotnet publish -c Release

# 2. Copiar archivos al servidor
# - SistemaMedico/vista/PanelPaciente.aspx
# - SistemaMedico/styles/panelpaciente.css
# - SistemaMedico/scripts/panelpaciente.js

# 3. Reciclar Application Pool en IIS
Stop-WebAppPool -Name "NombreDelAppPool"
Start-WebAppPool -Name "NombreDelAppPool"
```

### **2. SI USAS AZURE APP SERVICE:**

```bash
# Opción A: Desde Visual Studio
1. Click derecho en el proyecto
2. Publish ? Azure
3. Seleccionar tu App Service
4. Click "Publish"

# Opción B: Desde Azure Portal
1. Ir a tu App Service
2. Development Tools ? Console
3. Ejecutar: restart
```

### **3. SI USAS FTP:**

```
1. Conectar por FTP a tu servidor
2. Subir estos archivos:
   - /vista/PanelPaciente.aspx
   - /styles/panelpaciente.css
   - /scripts/panelpaciente.js
3. Restart del servicio web
```

---

## ?? **LIMPIAR CACHÉ**

### **En el Navegador:**
```
Chrome/Edge: Ctrl + Shift + Delete
Firefox: Ctrl + Shift + Delete
Safari: Cmd + Option + E

Luego: Ctrl + F5 (o Cmd + Shift + R en Mac)
```

### **En el Servidor:**

#### **IIS:**
```powershell
# Limpiar cache de ASP.NET
Remove-Item -Path "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files\*" -Recurse -Force
```

#### **Azure:**
```bash
# Restart del App Service
az webapp restart --name <app-name> --resource-group <rg-name>
```

---

## ?? **TROUBLESHOOTING**

### **Problema: El botón no aparece en producción**

**Solución:**
```html
<!-- Verificar que el viewport esté correcto -->
<meta name="viewport" content="width=device-width, initial-scale=1.0"/>

<!-- Verificar que Font Awesome esté cargando -->
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
```

### **Problema: El botón no responde a clics**

**Solución:**
1. Abrir DevTools (F12)
2. Console ? Buscar errores JavaScript
3. Verificar que `panelpaciente.js?v=2025012501` esté cargando
4. Ejecutar en consola: `inicializarMenuMovil();`

### **Problema: Los estilos no se actualizan**

**Solución:**
```javascript
// 1. Hard refresh
Ctrl + F5

// 2. Limpiar cache
Ctrl + Shift + Delete

// 3. Modo incógnito
Ctrl + Shift + N

// 4. Verificar versión del archivo
console.log(document.querySelector('link[href*="panelpaciente.css"]').href);
// Debe mostrar: .../panelpaciente.css?v=2025012501
```

---

## ? **VERIFICACIÓN POST-DEPLOYMENT**

### **Checklist:**

```
? 1. Abrir la página en Desktop (1920px)
   - Sidebar debe estar visible
   - No debe aparecer botón hamburguesa

? 2. Abrir la página en Tablet (768px)
   - Sidebar debe estar visible pero más angosto

? 3. Abrir la página en Móvil (375px)
   - Sidebar debe estar oculto
   - Botón hamburguesa debe aparecer arriba-izquierda
   - Al hacer clic, debe abrir el sidebar con overlay oscuro
   - Al hacer clic fuera del sidebar, debe cerrarse
   - Al presionar ESC, debe cerrarse

? 4. Verificar en Chrome DevTools
   F12 ? Toggle Device Toolbar (Ctrl + Shift + M)
   Probar en: iPhone 12, iPad, Galaxy S20

? 5. Verificar en dispositivo real
   - Probar en iPhone o Android real
   - El botón debe responder al primer toque
```

---

## ?? **DISPOSITIVOS PROBADOS**

| Dispositivo | Resolución | Estado |
|-------------|------------|--------|
| Desktop | 1920x1080 | ? OK |
| Laptop | 1366x768 | ? OK |
| iPad | 768x1024 | ? OK |
| iPhone 12 | 390x844 | ? OK |
| iPhone SE | 375x667 | ? OK |
| Galaxy S20 | 360x800 | ? OK |

---

## ?? **RECURSOS ÚTILES**

- **Documentación ASP.NET:** https://docs.microsoft.com/aspnet
- **IIS Troubleshooting:** https://docs.microsoft.com/iis
- **Azure App Service:** https://docs.microsoft.com/azure/app-service

---

## ?? **SOPORTE**

Si después de seguir todos los pasos el problema persiste:

1. Capturar screenshot del problema
2. Abrir DevTools (F12) y capturar la pestaña Console
3. Capturar la pestaña Network (filtrar por CSS/JS)
4. Enviar información para debugging

---

**Última actualización:** 25/01/2025
**Versión:** 2025012501
