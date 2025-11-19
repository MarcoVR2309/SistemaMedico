<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanelPaciente.aspx.cs" Inherits="SistemaMedico.vista.PanelPaciente" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Mi Panel - Clínica Aguirre</title>
    
    <link href="../styles/main.css" rel="stylesheet" />
    <link href="../styles/panelmedico.css" rel="stylesheet" />
    <link href="../styles/panelpaciente.css?v=2" rel="stylesheet" />


    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet"/>
    <script src="../Scripts/panelpaciente.js?v=1"></script>
</head>
<body>
<form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

<div class="dashboard-layout">

    <!-- SIDEBAR -->
    <aside class="dashboard-sidebar">

        <div class="sidebar-header">
            <div class="navbar-brand">
                <i class="fas fa-heartbeat logo-icon"></i>
                <span class="logo-text">Clínica Aguirre</span>
            </div>
        </div>
        
        <div class="sidebar-profile">
            <i class="fas fa-user-circle profile-icon"></i>
            <div class="profile-info">
                <span class="profile-welcome">Bienvenido,</span>
                <asp:Label ID="lblUserName" runat="server" Text="Paciente" CssClass="profile-name"></asp:Label>
            </div>
        </div>

        <nav class="sidebar-nav">
            <ul>
                <li class="nav-item active">
                    <a href="javascript:void(0)" data-target="panel-dashboard">
                        <i class="fas fa-calendar-check"></i> Mis Citas
                    </a>
                </li>
                <li class="nav-item">
                    <a href="javascript:void(0)" data-target="panel-perfil">
                        <i class="fas fa-user-circle"></i> Mi Perfil
                    </a>
                </li>
            </ul>
        </nav>

        <div class="sidebar-footer">
            <ul>
                <li class="nav-item">
                    <asp:LinkButton ID="lnkCerrarSesion" runat="server" OnClick="btnLogout_Click">
                        <i class="fas fa-sign-out-alt"></i> Cerrar Sesión
                    </asp:LinkButton>
                </li>
            </ul>
        </div>

    </aside>

    <!-- CONTENIDO PRINCIPAL -->
    <main class="dashboard-content">
        
        <header class="content-header">
            <div class="header-main">
                <h1 class="content-title">Mis Citas Médicas</h1>
                <p class="content-subtitle">Gestiona tus citas de manera fácil y rápida</p>
            </div>
            <div class="header-actions">
                <a href="#" id="btnAbrirModalCita" class="btn-service btn-purple">
                    <i class="fas fa-plus"></i> Agendar Cita
                </a>
            </div>
        </header>

        <!-- MODAL AGENDAR CITA -->
        <div id="modalAgendarCita" class="modal-backdrop">
            <div class="modal-content">
                
                <div class="modal-header">
                    <h3 class="section-title-panel">Agendar Nueva Cita</h3>
                    <span class="modal-close" id="spanCerrarModal">&times;</span>
                </div>

                <div class="modal-body">
                    
                    <div class="form-grid">
                        <div class="form-group">
                            <label>Especialidad *</label>
                            <asp:DropDownList ID="ddlEspecialidades" runat="server" CssClass="form-control"
                                AutoPostBack="True" OnSelectedIndexChanged="ddlEspecialidades_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>Doctor *</label>
                            <asp:DropDownList ID="ddlDoctores" runat="server" CssClass="form-control"
                                AutoPostBack="True" OnSelectedIndexChanged="ddlDoctores_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>

                        <div class="form-group">
                            <label>Fecha *</label>
                            <asp:TextBox ID="txtFechaCita" runat="server" CssClass="form-control" 
                                placeholder="dd/mm/aaaa" 
                                MaxLength="10"
                                AutoPostBack="True" 
                                OnTextChanged="txtFechaCita_TextChanged"></asp:TextBox>
                        </div>

                        <div class="form-group">
                            <label>Hora *</label>
                            <asp:DropDownList ID="ddlHoras" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label>Motivo de la Consulta</label>
                        <asp:TextBox ID="txtMotivo" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="3"
                            placeholder="Ej: Dolor de cabeza, control general, etc."></asp:TextBox>
                    </div>
                    
                    <asp:Label ID="lblModalMensaje" runat="server" CssClass="modal-mensaje"></asp:Label>

                </div>

                <div class="modal-footer">
                    <button type="button" id="btnCancelarModal" class="btn-service-outline btn-purple">
                        Cancelar
                    </button>
                    <asp:Button ID="btnReservar" runat="server" Text="Reservar Cita" 
                        CssClass="btn-service btn-teal" OnClick="btnReservar_Click" />
                </div>

            </div>
        </div>

        <!-- PANEL MIS CITAS -->
        <div id="panel-dashboard" class="content-panel active">
            <section class="citas-container">
                <h2 class="section-title-panel">Mis Citas Programadas</h2>
                
                <div class="citas-list">
                    <asp:Repeater ID="repeaterCitas" runat="server">
                        <ItemTemplate>
                            <div class="cita-card">
                                
                                <div class="cita-time">
                                    <i class="far fa-clock"></i>
                                    <span>
                                        <%# Eval("FechaCita", "{0:dd/MM/yyyy}") %> -
                                        <%# Eval("HoraCita", "{0:hh\\:mm}") %>
                                    </span>
                                </div>
                                
                                <div class="cita-patient-info">
                                    <h3 class="patient-name">Dr. <%# Eval("NombreDoctor") %></h3>
                                    <p class="patient-specialty"><%# Eval("NombreEspecialidad") %></p>
                                    <p class="patient-location"><i class="fas fa-map-marker-alt"></i> <%# Eval("SedeNombre") %></p>
                                </div>
                                
                                <div class='cita-status <%# ObtenerClaseEstado(Eval("Estado").ToString()) %>'>
                                    <i class='<%# ObtenerIconoEstado(Eval("Estado").ToString()) %>'></i>
                                    <%# Eval("Estado") %>
                                </div>
                                
                                <div class="cita-actions">
                                    <button type="button" class="btn-service-outline btn-purple" 
                                        onclick='verDetalleCita("<%# Eval("Id") %>")'>
                                        Ver Detalle
                                    </button>
                                </div>

                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <asp:Label ID="lblNoCitas" runat="server" 
                        Text="No tienes citas programadas." 
                        CssClass="no-citas-mensaje" Visible="false" />
                </div>
            </section>
        </div>

        <!-- PANEL PERFIL -->
        <div id="panel-perfil" class="content-panel">
            <section class="section-block">

                <h2 class="section-title-panel">Mi Perfil</h2>

                <div class="profile-card">

                    <!-- Avatar + info -->
                    <div class="profile-header">
                        <div class="profile-avatar">
                            <asp:Label ID="lblAvatarInitial" runat="server" Text="P"></asp:Label>
                        </div>
                        <div class="profile-header-info">
                            <h3><asp:Label ID="lblProfileUserName" runat="server"></asp:Label></h3>
                            <p>Paciente</p>
                        </div>
                    </div>

                    <hr />

                    <!-- CUADROS BONITOS -->
                    <div class="profile-data-grid">

                        <div class="profile-card-item">
                            <h4><i class="fas fa-user"></i> Nombre Completo</h4>
                            <p><asp:Label ID="txtProfileNombre" runat="server"></asp:Label></p>
                        </div>

                        <div class="profile-card-item">
                            <h4><i class="fas fa-envelope"></i> Correo</h4>
                            <p><asp:Label ID="txtProfileEmail" runat="server"></asp:Label></p>
                        </div>

                        <div class="profile-card-item">
                            <h4><i class="fas fa-phone"></i> Teléfono</h4>
                            <p><asp:Label ID="txtProfileTelefono" runat="server"></asp:Label></p>
                        </div>

                        <div class="profile-card-item">
                            <h4><i class="fas fa-birthday-cake"></i> Fecha de Nacimiento</h4>
                            <p><asp:Label ID="txtProfileFechaNac" runat="server"></asp:Label></p>
                        </div>

                        <div class="profile-card-item">
                            <h4><i class="fas fa-id-card"></i> DNI</h4>
                            <p><asp:Label ID="txtProfileDNI" runat="server"></asp:Label></p>
                        </div>

                        <div class="profile-card-item">
                            <h4><i class="fas fa-hashtag"></i> ID Usuario</h4>
                            <p><asp:Label ID="txtProfileUserId" runat="server"></asp:Label></p>
                        </div>

                    </div>

                </div>

            </section>
        </div>

    </main>

</div>

</form>
</body>
</html>
