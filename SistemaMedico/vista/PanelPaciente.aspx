<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanelPaciente.aspx.cs" Inherits="SistemaMedico.vista.PanelPaciente" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Panel del Paciente - MediTech</title>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" rel="stylesheet" />
    <link href="../styles/panelpaciente.css" rel="stylesheet" />
    <script src="../scripts/panelpaciente.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
        
        <asp:HiddenField ID="hdnActiveTab" runat="server" Value="misCitas" ClientIDMode="Static" />

        <!-- Barra de Navegación Superior -->
        <div class="navbar">
            <a href="PanelPaciente.aspx" class="navbar-brand"><i class="fas fa-notes-medical"></i> Clínica Aguirre</a>
            <div class="navbar-user-info">
                <span class="navbar-user">Bienvenido, <asp:Label ID="lblUserName" runat="server" Text="[Usuario]"></asp:Label></span>
                <asp:Button ID="btnLogout" runat="server" Text="Cerrar Sesión" OnClick="btnLogout_Click" CssClass="navbar-logout" />
            </div>
        </div>

        <!-- Contenedor Principal -->
        <div class="main-container">
            
            <!-- Mensaje de Estado Global -->
            <asp:Label ID="lblMensajeGlobal" runat="server" CssClass="status-message" EnableViewState="false" Visible="false"></asp:Label>

            <!-- Menú de Navegación de Pestañas -->
            <div class="tab-menu">
                <!-- El 'onclick' llama a la función en tu archivo .js -->
                <a id="btn_misCitas" class="tab-button active" onclick="changeTab('misCitas')"><i class="fas fa-calendar-check"></i>Mis Citas</a>
                <a id="btn_nuevaCita" class="tab-button" onclick="changeTab('nuevaCita')"><i class="fas fa-calendar-plus"></i>Nueva Cita</a>
                <a id="btn_miPerfil" class="tab-button" onclick="changeTab('miPerfil')"><i class="fas fa-user-circle"></i>Mi Perfil</a>
            </div>

            <!-- Contenido de las Pestañas -->
            <div class="tab-content">
                
                <!-- Pestaña 1: MIS CITAS PENDIENTES -->
                <div id="misCitas" class="content-section active">
                    <h2>Mis Citas Programadas</h2>
                    <asp:GridView ID="gvCitas" runat="server" AutoGenerateColumns="False" CssClass="gridview-style">
                        <Columns>
                            <asp:BoundField DataField="FechaCita" HeaderText="Fecha" DataFormatString="{0:d}" />
                            <asp:BoundField DataField="HoraCita" HeaderText="Hora" DataFormatString="{0:t}" />
                            <asp:BoundField DataField="NombreDoctor" HeaderText="Doctor" />
                            <asp:BoundField DataField="NombreEspecialidad" HeaderText="Especialidad" />
                            <asp:BoundField DataField="Sede" HeaderText="Sede" />
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
                        </Columns>
                         <EmptyDataTemplate>
                            <div class="no-citas">
                                <i class="far fa-calendar-times"></i>
                                <p>No tienes citas programadas.</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>

                <!-- Pestaña 2: SOLICITAR NUEVA CITA -->
                <div id="nuevaCita" class="content-section">
                    <h2>Programar Nueva Cita</h2>
                    <p style="margin-bottom: 25px;">Selecciona la especialidad, médico y horario que prefieras.</p>
                    <div class="profile-grid">
                        <div class="form-group">
                            <label for="<%= ddlEspecialidades.ClientID %>">1. Especialidad *</label>
                            <asp:DropDownList ID="ddlEspecialidades" runat="server" DataTextField="NomEsp" DataValueField="Id" 
                                AutoPostBack="True" OnSelectedIndexChanged="ddlEspecialidades_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="<%= ddlDoctores.ClientID %>">2. Doctor *</label>
                            <asp:DropDownList ID="ddlDoctores" runat="server" DataTextField="NombreCompleto" DataValueField="Id" 
                                AutoPostBack="True" OnSelectedIndexChanged="ddlDoctores_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="profile-grid">
                         <div class="form-group">
                            <label for="<%= txtFechaCita.ClientID %>">3. Fecha *</label>
                            <asp:TextBox ID="txtFechaCita" runat="server" TextMode="Date" OnTextChanged="txtFechaCita_TextChanged" AutoPostBack="True"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="<%= ddlHoras.ClientID %>">4. Hora Disponible *</label>
                            <asp:DropDownList ID="ddlHoras" runat="server">
                                <asp:ListItem Text="-- Seleccione hora --" Value=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <asp:Button ID="btnReservar" runat="server" Text="Programar Cita" CssClass="btn-primary" OnClick="btnReservar_Click" />
                </div>

                <!-- Pestaña 3: MI PERFIL -->
                <div id="miPerfil" class="content-section">
                    <h2>Información Personal</h2>
                    <div class="profile-header">
                        <div class="profile-avatar"><asp:Label ID="lblAvatarInitial" runat="server" Text="P"></asp:Label></div>
                        <div class="profile-info">
                            <h3><asp:Label ID="lblProfileUserName" runat="server" Text="[Usuario]"></asp:Label></h3>
                            <p>Paciente</p>
                        </div>
                    </div>
                    <div class="profile-grid">
                        <div class="form-group profile-field">
                            <label>Nombre Completo</label>
                            <span><asp:Label ID="txtProfileNombre" runat="server"></asp:Label></span>
                        </div>
                        <div class="form-group profile-field">
                            <label>Correo Electrónico</label>
                            <span><asp:Label ID="txtProfileEmail" runat="server"></asp:Label></span>
                        </div>
                        <div class="form-group profile-field">
                            <label>Teléfono</label>
                            <span><asp:Label ID="txtProfileTelefono" runat="server"></asp:Label></span>
                        </div>
                        <div class="form-group profile-field">
                            <label>Fecha de Nacimiento</label>
                            <span><asp:Label ID="txtProfileFechaNac" runat="server"></asp:Label></span>
                        </div>
                        <div class="form-group profile-field">
                            <label>ID de Usuario</label>
                            <span><asp:Label ID="txtProfileUserId" runat="server"></asp:Label></span>
                        </div>
                         <div class="form-group profile-field">
                            <label>DNI</label>
                            <span><asp:Label ID="txtProfileDNI" runat="server"></asp:Label></span>
                        </div>
                    </div>
                    <div class="profile-actions" style="margin-top: 20px; text-align: right;">
                        <asp:Button ID="btnEditProfile" runat="server" Text="Editar Contraseña" CssClass="btn-secondary" Visible="true" />
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>