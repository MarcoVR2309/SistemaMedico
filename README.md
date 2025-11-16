# 🏥 Sistema Médico - Gestión de Citas

[![Build and Deploy](https://github.com/MarcoVR2309/SistemaMedico/actions/workflows/main_sistemaclinica-webapp.yml/badge.svg)](https://github.com/MarcoVR2309/SistemaMedico/actions)
[![Azure](https://img.shields.io/badge/Azure-Deployed-blue)](https://sistemaclinica-webapp-ecbfudhb4g7d7fu.brazilsouth-01.azurewebsites.net)

Sistema de gestión de citas médicas desarrollado en **ASP.NET Web Forms** y **C#** para la **Clínica Aguirre**. 

El sistema permite a los pacientes iniciar sesión, ver su perfil, y gestionar la reserva de citas médicas, interactuando con una base de datos en **Microsoft Azure**.

---

## 🚀 Despliegue Automático

Este proyecto cuenta con **CI/CD automático** configurado con **GitHub Actions**. 

- **🌐 URL de producción:** https://sistemaclinica-webapp-ecbfudhb4g7d7fu.brazilsouth-01.azurewebsites.net
- **🔄 Despliegue:** Automático al hacer push a `main`
- **⏱️ Tiempo:** 3-5 minutos por despliegue

📖 **[Ver guía completa de despliegue](docs/DEPLOYMENT.md)**

---

## 🛠️ Tecnologías

- **Backend:** ASP.NET Web Forms 4.7.2
- **Lenguaje:** C# 7.3
- **Base de datos:** Azure SQL Database
- **Hosting:** Azure App Service (Windows)
- **CI/CD:** GitHub Actions
- **Frontend:** HTML5, CSS3, JavaScript

---

## 📁 Estructura del Proyecto

```
SistemaMedico/
├── .github/workflows/         # Configuración de GitHub Actions
├── SistemaMedico/
│   ├── vista/                # Páginas ASPX (UI)
│   ├── datos/                # Capa de acceso a datos (DAO)
│   ├── modelo/               # Modelos de datos
│   ├── styles/               # Hojas de estilo CSS
│   ├── scripts/              # Archivos JavaScript
│   └── Web.config            # Configuración de la aplicación
├── docs/                     # Documentación
└── README.md
```

---

## 🔧 Configuración Local

### **Prerrequisitos**

- Visual Studio 2019 o superior
- .NET Framework 4.7.2
- SQL Server Management Studio (opcional)
- Git

### **Instalación**

1. **Clonar el repositorio:**
```bash
git clone https://github.com/MarcoVR2309/SistemaMedico.git
cd SistemaMedico
```

2. **Abrir en Visual Studio:**
```bash
# Abre la solución
SistemaMedico/SistemaMedico.sln
```

3. **Restaurar paquetes NuGet:**
```bash
nuget restore
```

4. **Configurar cadena de conexión:**
   - Edita `Web.config`
   - Actualiza la cadena de conexión `RedClinicas` con tus credenciales

5. **Ejecutar el proyecto:**
   - Presiona `F5` en Visual Studio
   - O haz click en el botón **IIS Express**

---

## 🔄 Flujo de Trabajo

### **Desarrollo**

```bash
# 1. Crear una rama para tu feature
git checkout -b feature/mi-nueva-funcionalidad

# 2. Hacer tus cambios
# ... código ...

# 3. Commit
git add .
git commit -m "Add: Nueva funcionalidad"

# 4. Push a tu rama
git push origin feature/mi-nueva-funcionalidad

# 5. Crear Pull Request en GitHub
# 6. Después del review, merge a main
# 7. El despliegue se ejecuta automáticamente ✨
```

### **Despliegue Automático**

Cada push a `main` desencadena:
1. ✅ Compilación automática
2. ✅ Pruebas (si están configuradas)
3. ✅ Despliegue a Azure
4. ✅ Notificación del resultado

---

## 📊 Base de Datos

### **Servidor Azure SQL:**
- **Host:** `clinica-aguirre-pe.database.windows.net`
- **Base de datos:** `RedCLinicas`
- **Autenticación:** SQL Server

📖 **[Ver guía de conexión a Azure SQL](docs/ConectarSSMSaAzure.md)**

---

## 🔒 Seguridad

- ✅ Autenticación con Managed Identity
- ✅ Conexión HTTPS obligatoria
- ✅ Secretos gestionados por Azure
- ✅ Firewall configurado en Azure SQL

---

## 🤝 Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

---

## 📝 Licencia

Este proyecto es propiedad de **Clínica Aguirre** - Todos los derechos reservados.

---

## 👥 Equipo de Desarrollo

- **Desarrolladores:** Equipo UPN
- **Universidad:** Universidad Privada del Norte
- **Proyecto:** Sistema de Gestión Médica

---

## 📞 Contacto

Para soporte o consultas:
- **Email:** info@clinicaaguirre.com
- **Teléfono:** 01-234-5678 / 01-987-6543

---

## 📚 Documentación Adicional

- [Guía de Despliegue](docs/DEPLOYMENT.md)
- [Conexión a Azure SQL](docs/ConectarSSMSaAzure.md)
- [Scripts de Base de Datos](SistemaMedico/docs/)

---

**Última actualización:** Noviembre 2025 | **Versión:** 1.0.0
