<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestionUsuarios.aspx.cs" Inherits="SistemaMedico.vista.admin.GestionUsuarios" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Gestión de Usuarios - Admin</title>
    <link href="../../styles/admin-usuarios.css" rel="stylesheet" />
    <script src="https://kit.fontawesome.com/64d58efce2.js" crossorigin="anonymous"></script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div class="admin-container">
            <div class="admin-header">
                <div>
                    <h1>Gestión de Cuentas</h1>
                    <span>Recursos Humanos · Administración de usuarios del sistema</span>
                </div>
                <asp:Button ID="btnMostrarCrear" runat="server" CssClass="primary-button" Text="Crear Usuario" OnClick="btnMostrarCrear_Click" UseSubmitBehavior="false" />
            </div>

            <asp:Panel ID="pnlNotificacion" runat="server" Visible="false" CssClass="filters-card" Style="border-left: 4px solid #3155ff;">
                <asp:Literal ID="litNotificacion" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="filters-card">
                <div class="filters-grid">
                    <div class="form-control">
                        <label for="<%= txtBusqueda.ClientID %>">Buscar usuario</label>
                        <asp:TextBox ID="txtBusqueda" runat="server" placeholder="Buscar por nombre, correo o documento"></asp:TextBox>
                    </div>
                    <div class="form-control">
                        <label for="<%= ddlRolFiltro.ClientID %>">Filtrar por rol</label>
                        <asp:DropDownList ID="ddlRolFiltro" runat="server"></asp:DropDownList>
                    </div>
                    <div class="form-control">
                        <label for="<%= ddlEstadoFiltro.ClientID %>">Estado</label>
                        <asp:DropDownList ID="ddlEstadoFiltro" runat="server">
                            <asp:ListItem Text="Todos" Value=""></asp:ListItem>
                            <asp:ListItem Text="Activos" Value="ACTIVO"></asp:ListItem>
                            <asp:ListItem Text="Inactivos" Value="INACTIVO"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnBuscar" runat="server" Text="Filtrar" CssClass="primary-button" OnClick="btnBuscar_Click" />
                        <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar" CssClass="outline-button" OnClick="btnLimpiar_Click" CausesValidation="false" />
                    </div>
                </div>
            </div>

            <div class="stats-grid">
                <div class="stat-card">
                    <span>Total usuarios</span>
                    <h3><asp:Label ID="lblTotalUsuarios" runat="server" Text="0"></asp:Label></h3>
                </div>
                <div class="stat-card">
                    <span>Doctores</span>
                    <h3><asp:Label ID="lblTotalDoctores" runat="server" Text="0"></asp:Label></h3>
                </div>
                <div class="stat-card">
                    <span>Pacientes</span>
                    <h3><asp:Label ID="lblTotalPacientes" runat="server" Text="0"></asp:Label></h3>
                </div>
                <div class="stat-card">
                    <span>Activos</span>
                    <h3><asp:Label ID="lblTotalActivos" runat="server" Text="0"></asp:Label></h3>
                </div>
            </div>

            <div class="grid-card">
                <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" CssClass="users-grid"
                    OnRowCommand="gvUsuarios_RowCommand" DataKeyNames="Id" EmptyDataText="No se encontraron usuarios con los criterios seleccionados.">
                    <Columns>
                        <asp:TemplateField HeaderText="Usuario">
                            <ItemTemplate>
                                <div>
                                    <strong><%# Eval("NombreCompleto") %></strong><br />
                                    <span style="font-size:12px;color:#6c757d;"><%# Eval("Email") %></span>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rol">
                            <ItemTemplate>
                                <span class='badge <%# ObtenerClaseRol(Eval("Rol").ToString()) %>'><%# Eval("Rol") %></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Teléfono">
                            <ItemTemplate>
                                <%# string.IsNullOrEmpty(Eval("Telefono").ToString()) ? "-" : Eval("Telefono") %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Estado">
                            <ItemTemplate>
                                <span class='badge <%# (bool)Eval("Activo") ? "estado-activo" : "estado-inactivo" %>'>
                                    <%# (bool)Eval("Activo") ? "Activo" : "Inactivo" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Especialidad / Rol">
                            <ItemTemplate>
                                <%# Eval("NombreEspecialidad") != null && Eval("NombreEspecialidad").ToString() != "" ? Eval("NombreEspecialidad") : "-" %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Acciones">
                            <ItemTemplate>
                                <div class="actions-group">
                                    <asp:LinkButton ID="btnEditar" runat="server" CssClass="icon-button" CommandName="Editar" CommandArgument='<%# Eval("Id") %>'
                                        ToolTip="Editar">
                                        <i class="fas fa-pen"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnEstado" runat="server" CssClass='<%# ((bool)Eval("Activo")) ? "icon-button danger" : "icon-button success" %>'
                                        CommandName="ToggleEstado" CommandArgument='<%# Eval("Id") + "|" + Eval("Activo") %>' ToolTip='<%# ((bool)Eval("Activo")) ? "Desactivar" : "Activar" %>'>
                                        <i class='<%# ((bool)Eval("Activo")) ? "fas fa-user-slash" : "fas fa-user-check" %>'></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnReset" runat="server" CssClass="icon-button" CommandName="ResetPassword" CommandArgument='<%# Eval("Id") %>' ToolTip="Resetear contraseña">
                                        <i class="fas fa-undo"></i>
                                    </asp:LinkButton>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Panel ID="pnlEmptyState" runat="server" Visible="false" CssClass="empty-state">
                    <i class="fas fa-user-slash" style="font-size:36px;margin-bottom:12px;display:block;"></i>
                    <span>No se encontraron usuarios para los filtros seleccionados.</span>
                </asp:Panel>
            </div>
        </div>

        <asp:HiddenField ID="hdnUsuarioSeleccionado" runat="server" />
        <asp:HiddenField ID="hdnMostrarModal" runat="server" />
        <asp:HiddenField ID="hdnAccionModal" runat="server" />

        <!-- Modal Crear Usuario -->
        <div id="modalCrearUsuario" class="modal-overlay">
            <div class="modal-card">
                <div class="modal-header">
                    <h3>Crear Usuario</h3>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblErrorCrear" runat="server" CssClass="validation-summary" Visible="false"></asp:Label>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtNombreCrear.ClientID %>">Nombre</label>
                            <asp:TextBox ID="txtNombreCrear" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtApellidoCrear.ClientID %>">Apellido</label>
                            <asp:TextBox ID="txtApellidoCrear" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtEmailCrear.ClientID %>">Email</label>
                            <asp:TextBox ID="txtEmailCrear" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtTelefonoCrear.ClientID %>">Teléfono</label>
                            <asp:TextBox ID="txtTelefonoCrear" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= ddlRolCrear.ClientID %>">Rol</label>
                            <asp:DropDownList ID="ddlRolCrear" runat="server"></asp:DropDownList>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtDocumentoCrear.ClientID %>">Documento (DNI)</label>
                            <asp:TextBox ID="txtDocumentoCrear" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtPasswordCrear.ClientID %>">Contraseña temporal</label>
                            <asp:TextBox ID="txtPasswordCrear" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= ddlGeneroCrear.ClientID %>">Género</label>
                            <asp:DropDownList ID="ddlGeneroCrear" runat="server">
                                <asp:ListItem Text="Seleccionar" Value=""></asp:ListItem>
                                <asp:ListItem Text="Femenino" Value="F"></asp:ListItem>
                                <asp:ListItem Text="Masculino" Value="M"></asp:ListItem>
                                <asp:ListItem Text="Otro" Value="O"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtFechaNacimientoCrear.ClientID %>">Fecha de nacimiento</label>
                            <asp:TextBox ID="txtFechaNacimientoCrear" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= ddlEspecialidadCrear.ClientID %>">Especialidad (Doctores)</label>
                            <asp:DropDownList ID="ddlEspecialidadCrear" runat="server"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtColegiaturaCrear.ClientID %>">Colegiatura (Doctores)</label>
                            <asp:TextBox ID="txtColegiaturaCrear" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtExperienciaCrear.ClientID %>">Años de experiencia (Doctores)</label>
                            <asp:TextBox ID="txtExperienciaCrear" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtPesoPacienteCrear.ClientID %>">Peso (Pacientes)</label>
                            <asp:TextBox ID="txtPesoPacienteCrear" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtFechaReferenciaPacienteCrear.ClientID %>">Fecha referencia (Pacientes)</label>
                            <asp:TextBox ID="txtFechaReferenciaPacienteCrear" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="secondary-button" data-close-modal="modalCrearUsuario">Cancelar</button>
                    <asp:Button ID="btnCrearUsuario" runat="server" Text="Crear Usuario" CssClass="primary-button" OnClick="btnCrearUsuario_Click" />
                </div>
            </div>
        </div>

        <!-- Modal Editar Usuario -->
        <div id="modalEditarUsuario" class="modal-overlay">
            <div class="modal-card">
                <div class="modal-header">
                    <h3>Editar Usuario</h3>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblErrorEditar" runat="server" CssClass="validation-summary" Visible="false"></asp:Label>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtNombreEditar.ClientID %>">Nombre</label>
                            <asp:TextBox ID="txtNombreEditar" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtApellidoEditar.ClientID %>">Apellido</label>
                            <asp:TextBox ID="txtApellidoEditar" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtEmailEditar.ClientID %>">Email</label>
                            <asp:TextBox ID="txtEmailEditar" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtTelefonoEditar.ClientID %>">Teléfono</label>
                            <asp:TextBox ID="txtTelefonoEditar" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= ddlRolEditar.ClientID %>">Rol</label>
                            <asp:DropDownList ID="ddlRolEditar" runat="server"></asp:DropDownList>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtDocumentoEditar.ClientID %>">Documento (DNI)</label>
                            <asp:TextBox ID="txtDocumentoEditar" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= ddlGeneroEditar.ClientID %>">Género</label>
                            <asp:DropDownList ID="ddlGeneroEditar" runat="server">
                                <asp:ListItem Text="Seleccionar" Value=""></asp:ListItem>
                                <asp:ListItem Text="Femenino" Value="F"></asp:ListItem>
                                <asp:ListItem Text="Masculino" Value="M"></asp:ListItem>
                                <asp:ListItem Text="Otro" Value="O"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtFechaNacimientoEditar.ClientID %>">Fecha de nacimiento</label>
                            <asp:TextBox ID="txtFechaNacimientoEditar" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= ddlEspecialidadEditar.ClientID %>">Especialidad (Doctores)</label>
                            <asp:DropDownList ID="ddlEspecialidadEditar" runat="server"></asp:DropDownList>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtColegiaturaEditar.ClientID %>">Colegiatura (Doctores)</label>
                            <asp:TextBox ID="txtColegiaturaEditar" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtExperienciaEditar.ClientID %>">Años de experiencia (Doctores)</label>
                            <asp:TextBox ID="txtExperienciaEditar" runat="server"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= txtPesoPacienteEditar.ClientID %>">Peso (Pacientes)</label>
                            <asp:TextBox ID="txtPesoPacienteEditar" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-two-columns">
                        <div class="form-control">
                            <label for="<%= txtFechaReferenciaPacienteEditar.ClientID %>">Fecha referencia (Pacientes)</label>
                            <asp:TextBox ID="txtFechaReferenciaPacienteEditar" runat="server" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="form-control">
                            <label for="<%= chkActivoEditar.ClientID %>">Estado</label>
                            <asp:CheckBox ID="chkActivoEditar" runat="server" Text="Usuario activo" />
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="secondary-button" data-close-modal="modalEditarUsuario">Cancelar</button>
                    <asp:Button ID="btnGuardarCambios" runat="server" Text="Guardar Cambios" CssClass="primary-button" OnClick="btnGuardarCambios_Click" />
                </div>
            </div>
        </div>

        <!-- Modal Confirmación Genérica -->
        <div id="modalConfirmacion" class="modal-overlay">
            <div class="modal-card">
                <div class="modal-header">
                    <h3 id="tituloModalConfirmacion">Confirmar acción</h3>
                </div>
                <div class="modal-body">
                    <asp:Label ID="lblMensajeConfirmacion" runat="server"></asp:Label>
                </div>
                <div class="modal-footer">
                    <button type="button" class="secondary-button" data-close-modal="modalConfirmacion">Cancelar</button>
                    <asp:Button ID="btnConfirmarAccion" runat="server" Text="Confirmar" CssClass="danger-button" OnClick="btnConfirmarAccion_Click" />
                </div>
            </div>
        </div>

        <script src="../../scripts/admin-usuarios.js"></script>
    </form>
</body>
</html>

