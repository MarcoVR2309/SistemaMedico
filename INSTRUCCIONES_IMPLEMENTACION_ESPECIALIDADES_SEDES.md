# ?? INSTRUCCIONES PASO A PASO - Implementación Especialidades y Sedes

## ? Estado Actual
- ? Código C# backend: **COMPLETO Y FUNCIONANDO**
- ? Paneles HTML: **IMPLEMENTADOS**
- ? Modales HTML: **MAL UBICADOS** (están dentro del modal de usuarios)
- ? JavaScript: **FALTA FUNCIÓN cambiarRol()**
- ? Designer.cs: **FALTA ACTUALIZAR**

---

## ?? PARTE 1: ARREGLAR ESTRUCTURA HTML DEL ARCHIVO .ASPX

### Problema Actual
Los modales de Especialidades y Sedes están **DENTRO** del modal de Usuarios. Esto causará errores.

### Solución

#### PASO 1.1: Localiza en el archivo `PanelAdministrador.aspx` el cierre del modal de usuarios

Busca esta línea (aproximadamente línea 1200):
```html
</asp:Panel>
</div>

<!-- FOOTER -->
<div class="modal-footer-custom">
    <button type="button" class="btn btn-secondary" onclick="cerrarModalCrearUsuario()">Cancelar</button>
    <asp:Button ID="btnCrearUsuario" runat="server" Text="Crear Usuario" CssClass="btn btn-primary" OnClick="btnCrearUsuario_Click" />
</div>
```

#### PASO 1.2: DESPUÉS del `</div>` que cierra el modal de usuarios, agrega:

```html
</div>
</div>

<!-- MODAL ESPECIALIDAD -->
<div id="modalEspecialidad" class="modal-backdrop">
    <div class="modal-container-custom" style="max-width: 600px;">
        <div class="modal-header-custom">
            <h2><i class="fas fa-stethoscope"></i> Nueva Especialidad</h2>
            <button type="button" class="btn-close-modal" onclick="cerrarModalEspecialidad()">×</button>
        </div>

        <div class="modal-body-custom">
            <asp:Panel ID="pnlMensajeEsp" runat="server" CssClass="alert" Visible="false">
                <asp:Label ID="lblMensajeEsp" runat="server"></asp:Label>
            </asp:Panel>

            <div class="form-group">
                <label>Nombre de la Especialidad <span class="required">*</span></label>
                <asp:TextBox ID="txtNombreEsp" runat="server" CssClass="form-control" 
                             placeholder="Ej: Cardiología" MaxLength="150"></asp:TextBox>
            </div>

            <div class="form-group">
                <label>Descripción</label>
                <asp:TextBox ID="txtDescripcionEsp" runat="server" CssClass="form-control" 
                             TextMode="MultiLine" Rows="4" 
                             placeholder="Descripción de la especialidad..." MaxLength="500"></asp:TextBox>
            </div>
        </div>

        <div class="modal-footer-custom">
            <button type="button" class="btn btn-secondary" onclick="cerrarModalEspecialidad()">Cancelar</button>
            <asp:Button ID="btnCrearEspecialidad" runat="server" Text="Crear Especialidad" 
                        CssClass="btn btn-primary" OnClick="btnCrearEspecialidad_Click" />
        </div>
    </div>
</div>

<!-- MODAL SEDE -->
<div id="modalSede" class="modal-backdrop">
    <div class="modal-container-custom" style="max-width: 700px;">
        <div class="modal-header-custom">
            <h2><i class="fas fa-building"></i> Nueva Sede</h2>
            <button type="button" class="btn-close-modal" onclick="cerrarModalSede()">×</button>
        </div>

        <div class="modal-body-custom">
            <asp:Panel ID="pnlMensajeSede" runat="server" CssClass="alert" Visible="false">
                <asp:Label ID="lblMensajeSede" runat="server"></asp:Label>
            </asp:Panel>

            <div class="form-row">
                <div class="form-group">
                    <label>Nombre de la Sede <span class="required">*</span></label>
                    <asp:TextBox ID="txtNombreSede" runat="server" CssClass="form-control" 
                                 placeholder="Ej: Sede Principal" MaxLength="200"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Teléfono</label>
                    <asp:TextBox ID="txtTelefonoSede" runat="server" CssClass="form-control" 
                                 placeholder="987654321" MaxLength="20"></asp:TextBox>
                </div>
            </div>

            <div class="form-group">
                <label>Dirección</label>
                <asp:TextBox ID="txtDireccionSede" runat="server" CssClass="form-control" 
                             placeholder="Av. Principal 123" MaxLength="300"></asp:TextBox>
            </div>

            <div class="form-group">
                <label>Email</label>
                <asp:TextBox ID="txtEmailSede" runat="server" CssClass="form-control" 
                             TextMode="Email" placeholder="sede@clinica.com" MaxLength="150"></asp:TextBox>
            </div>
        </div>

        <div class="modal-footer-custom">
            <button type="button" class="btn btn-secondary" onclick="cerrarModalSede()">Cancelar</button>
            <asp:Button ID="btnCrearSede" runat="server" Text="Crear Sede" 
                        CssClass="btn btn-primary" OnClick="btnCrearSede_Click" />
        </div>
    </div>
</div>
```

---

## ?? PARTE 2: ACTUALIZAR JAVASCRIPT

#### PASO 2.1: Localiza el script al final del archivo

Busca donde dice:
```javascript
function cerrarModalCrearUsuario() {
```

#### PASO 2.2: AGREGA la función `cambiarRol()` que falta:

```javascript
function cambiarRol() {
    var ddlRol = document.getElementById('<%= ddlRol.ClientID %>');
    var selectedValue = ddlRol.value;

    // Ocultar todos los paneles
    document.getElementById('<%= pnlMedico.ClientID %>').classList.remove('show');
    document.getElementById('<%= pnlAdministrador.ClientID %>').classList.remove('show');
    document.getElementById('<%= pnlPaciente.ClientID %>').classList.remove('show');

    // Mostrar el panel correspondiente
    if (selectedValue === 'R0000002') { // Médico
        document.getElementById('<%= pnlMedico.ClientID %>').classList.add('show');
    } else if (selectedValue === 'R0000001') { // Administrador
        document.getElementById('<%= pnlAdministrador.ClientID %>').classList.add('show');
    } else if (selectedValue === 'R0000003') { // Paciente
        document.getElementById('<%= pnlPaciente.ClientID %>').classList.add('show');
    }
}
```

---

## ?? PARTE 3: ACTUALIZAR DESIGNER.CS

Abre `PanelAdministrador.aspx.designer.cs` y AGREGA estos controles al final (antes del último `}`):

```csharp
/// <summary>
/// gvEspecialidades control.
/// </summary>
protected global::System.Web.UI.WebControls.GridView gvEspecialidades;

/// <summary>
/// lblTotalEspecialidades control.
/// </summary>
protected global::System.Web.UI.WebControls.Label lblTotalEspecialidades;

/// <summary>
/// pnlMensajeEsp control.
/// </summary>
protected global::System.Web.UI.WebControls.Panel pnlMensajeEsp;

/// <summary>
/// lblMensajeEsp control.
/// </summary>
protected global::System.Web.UI.WebControls.Label lblMensajeEsp;

/// <summary>
/// txtNombreEsp control.
/// </summary>
protected global::System.Web.UI.WebControls.TextBox txtNombreEsp;

/// <summary>
/// txtDescripcionEsp control.
/// </summary>
protected global::System.Web.UI.WebControls.TextBox txtDescripcionEsp;

/// <summary>
/// btnCrearEspecialidad control.
/// </summary>
protected global::System.Web.UI.WebControls.Button btnCrearEspecialidad;

/// <summary>
/// gvSedes control.
/// </summary>
protected global::System.Web.UI.WebControls.GridView gvSedes;

/// <summary>
/// lblTotalSedes control.
/// </summary>
protected global::System.Web.UI.WebControls.Label lblTotalSedes;

/// <summary>
/// pnlMensajeSede control.
/// </summary>
protected global::System.Web.UI.WebControls.Panel pnlMensajeSede;

/// <summary>
/// lblMensajeSede control.
/// </summary>
protected global::System.Web.UI.WebControls.Label lblMensajeSede;

/// <summary>
/// txtNombreSede control.
/// </summary>
protected global::System.Web.UI.WebControls.TextBox txtNombreSede;

/// <summary>
/// txtDireccionSede control.
/// </summary>
protected global::System.Web.UI.WebControls.TextBox txtDireccionSede;

/// <summary>
/// txtTelefonoSede control.
/// </summary>
protected global::System.Web.UI.WebControls.TextBox txtTelefonoSede;

/// <summary>
/// txtEmailSede control.
/// </summary>
protected global::System.Web.UI.WebControls.TextBox txtEmailSede;

/// <summary>
/// btnCrearSede control.
/// </summary>
protected global::System.Web.UI.WebControls.Button btnCrearSede;
```

---

## ? VERIFICACIÓN FINAL

1. **Compilar el proyecto**: `Ctrl + Shift + B`
2. **Verificar que no hay errores**
3. **Ejecutar el proyecto**
4. **Probar:**
   - ? Navegar al panel de Especialidades
   - ? Abrir modal "Nueva Especialidad"
   - ? Crear una especialidad
   - ? Verificar que aparece en la tabla
   - ? Navegar al panel de Sedes
   - ? Abrir modal "Nueva Sede"
   - ? Crear una sede
   - ? Verificar que aparece en la tabla

---

## ?? TROUBLESHOOTING

### Error: "El nombre 'gvEspecialidades' no existe"
**Solución**: Actualizar el `designer.cs` con los controles (Parte 3)

### Los modales no se abren
**Solución**: Verificar que las funciones JavaScript estén correctamente agregadas

### La tabla está vacía
**Solución**: 
1. Verificar que existan registros en la base de datos
2. Revisar los stored procedures `sp_ListarEspecialidades` y `sp_ListarSedes`

### Error al crear especialidad/sede
**Solución**: 
1. Verificar la conexión a la base de datos
2. Revisar los logs de errores en `Debug.WriteLine`

---

## ?? ESTRUCTURA DE ARCHIVOS MODIFICADOS

```
? PanelAdministrador.aspx.cs      - Backend (YA LISTO)
??  PanelAdministrador.aspx         - Frontend (ARREGLAR MODALES)
??  PanelAdministrador.aspx.designer.cs - Designer (AGREGAR CONTROLES)
```

---

¿Necesitas ayuda con algún paso específico? ¡Házmelo saber!
