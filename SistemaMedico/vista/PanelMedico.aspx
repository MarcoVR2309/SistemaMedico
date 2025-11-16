<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanelMedico.aspx.cs" Inherits="SistemaMedico.vista.PanelMedico" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Dashboard Médico - Clínica Aguirre</title>
    
    <link href="../styles/main.css" rel="stylesheet" type="text/css" />
    <link href="../styles/panelmedico.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet"/>
    <script src="../scripts/panelmedico.js"></script>

</head>
<body>
    <form id="form1" runat="server">
        
        <div class="dashboard-layout">
            
            <aside class="dashboard-sidebar">
                <div class="sidebar-header">
                    <div class="navbar-brand">
                        <i class="fas fa-heartbeat logo-icon"></i>
                        <span class="logo-text">Clínica Aguirre</span>
                    </div>
                </div>
                
                <div class="sidebar-profile">
                    <i class="fas fa-user-md profile-icon"></i>
                    <div class="profile-info">
                        <span class="profile-welcome">Bienvenido,</span>
                        <asp:Label ID="lblDoctorName" runat="server" Text="Dr. Juan Pérez" CssClass="profile-name"></asp:Label>
                    </div>
                </div>

                <nav class="sidebar-nav">
                    <ul>
                        <li class="nav-item active">
                            <a href="#" data-target="panel-dashboard">
                                <i class="fas fa-tachometer-alt"></i> Dashboard
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="#" data-target="panel-horario">
                                <i class="fas fa-calendar-alt"></i> Mi Horario
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="#" data-target="panel-pacientes">
                                <i class="fas fa-users"></i> Pacientes
                            </a>
                        </li>
                    </ul>
                </nav>

                <div class="sidebar-footer">
                    <ul>
                         <li class="nav-item">
                            <a href="#" data-target="panel-perfil">
                                <i class="fas fa-user-circle"></i> Mi Perfil
                            </a>
                        </li>
                        <li class="nav-item">
                            <asp:LinkButton ID="lnkCerrarSesion" runat="server" OnClick="lnkCerrarSesion_Click">
                                <i class="fas fa-sign-out-alt"></i> Cerrar Sesión
                            </asp:LinkButton>
                        </li>
                    </ul>
                </div>
            </aside>

            <main class="dashboard-content">
                
                <header class="content-header">
                     <div class="header-main">
                        <h1 class="content-title">Dashboard</h1>
                        <p class="content-subtitle">Citas programadas para hoy: 12 de Noviembre, 2025</p>
                    </div>
                    <div class="header-actions">
                         <a href="#" id="btnAbrirModalCita" class="btn-service btn-purple">+ Agendar Cita</a>
                    </div>
                </header>

                <div id="modalAgendarCita" class="modal-backdrop">
    <div class="modal-content">
        
        <div class="modal-header">
            <h3 class="section-title-panel">Agendar Nueva Cita</h3>
            <span class="modal-close" id="spanCerrarModal">&times;</span>
        </div>

        <div class="modal-body">
            
            <div class="form-grid">
                <div class="form-group">
                    <label>Paciente</label>
                    <asp:DropDownList ID="ddlPacienteModal" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
                <div class="form-group">
                    <label>Sede</label>
                    <asp:DropDownList ID="ddlSedeModal" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>

                <div class="form-group">
                    <label>Fecha</label>
                    <asp:TextBox ID="txtFechaModal" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>Hora</label>
                    <asp:DropDownList ID="ddlHoraModal" runat="server" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>

            <div class="form-group">
                <label>Motivo de la Consulta</label>
                <asp:TextBox ID="txtMotivoModal" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
            
            <asp:Label ID="lblModalMensaje" runat="server" CssClass="modal-mensaje" EnableViewState="false"></asp:Label>

        </div>

        <div class="modal-footer">
            <button type="button" id="btnCancelarModal" class="btn-service-outline btn-purple">Cancelar</button>
            <asp:Button ID="btnGuardarCita" runat="server" Text="Guardar Cita" CssClass="btn-service btn-teal" OnClick="btnGuardarCita_Click" />
        </div>

    </div>
</div>

                <div id="modalFinalizarConsulta" class="modal-backdrop">
    <div class="modal-content">
        
        <div class="modal-header">
            <h3 class="section-title-panel">Registrar Consulta Médica</h3>
            <span class="modal-close" id="spanCerrarModalConsulta">&times;</span>
        </div>

        <div class="modal-body">
            
            <asp:HiddenField ID="hfConsultaIdCita" runat="server" />
            <asp:HiddenField ID="hfConsultaIdConsulta" runat="server" />

            <div class="form-group">
                <label>Síntomas</label>
                <asp:TextBox ID="txtSintomas" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
            <div class="form-group">
                <label>Diagnóstico</label>
                <asp:TextBox ID="txtDiagnostico" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
            <div class="form-group">
                <label>Tratamiento</label>
                <asp:TextBox ID="txtTratamiento" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
            <div class="form-group">
                <label>Observaciones</label>
                <asp:TextBox ID="txtObservaciones" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
            </div>
            
            <asp:Label ID="lblModalConsultaMensaje" runat="server" CssClass="modal-mensaje" EnableViewState="false"></asp:Label>
        </div>

        <div class="modal-footer">
            <button type="button" id="btnCancelarModalConsulta" class="btn-service-outline btn-purple">Cancelar</button>
            <asp:Button ID="btnGuardarConsulta" runat="server" Text="Finalizar y Guardar" CssClass="btn-service btn-teal" OnClick="btnGuardarConsulta_Click" 
                OnClientClick="return handleGuardarCita(this);" />
        </div>
    </div>
</div>

                <div id="panel-dashboard" class="content-panel active">
                    <section class="citas-container">
                        <h2 class="section-title-panel">Citas del Día</h2>
                        
                        <div class="citas-list">
    
    <asp:Repeater ID="repeaterCitas" runat="server" OnItemCommand="repeaterCitas_ItemCommand">
        <ItemTemplate>
            <div class="cita-card">
                
                <div class="cita-time">
                    <i class="far fa-clock"></i>
                    <span><%# Eval("HoraCompleta", "{0:hh:mm tt}") %></span>
                </div>
                
                <div class="cita-patient-info">
                    <h3 class="patient-name"><%# Eval("NombresPaciente") %> <%# Eval("ApellidosPaciente") %></h3>
                </div>
                
                <div class='cita-status <%# Eval("ESTADO").ToString() == "P" ? "status-pendiente" : "status-confirmada" %>'>
                    <i class='<%# Eval("ESTADO").ToString() == "P" ? "fas fa-exclamation-circle" : "fas fa-check-circle" %>'></i>
                    <%# Eval("ESTADO").ToString() == "P" ? "Pendiente" : "Confirmada" %>
                    </div>
                
                <div class="cita-actions">
    
    <asp:Button ID="btnAccionCita" runat="server"
        
        Text='<%# Eval("ESTADO").ToString() == "I" ? "Finalizar Consulta" : (Eval("ESTADO").ToString() == "F" ? "Finalizada" : "Iniciar Consulta") %>'
        
        CssClass='<%# Eval("ESTADO").ToString() == "I" ? "btn-service btn-purple" : (Eval("ESTADO").ToString() == "F" ? "btn-service-outline btn-teal" : "btn-service btn-teal") %>'
        
        CommandName='<%# Eval("ESTADO").ToString() == "I" ? "AbrirModalFinalizar" : "Iniciar" %>'
        
        CommandArgument='<%# Eval("IdCita") %>'
        
        Enabled='<%# Eval("ESTADO").ToString() != "F" %>'
        />
        
    <asp:Button ID="btnVerFicha" runat="server" 
        Text="Ver Ficha" 
        CssClass="btn-service-outline btn-purple" 
        CommandName="VerFicha" 
        CommandArgument='<%# Eval("IdCita") %>' />
</div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Label ID="lblNoCitas" runat="server" Text="No hay citas programadas para hoy." CssClass="no-citas-mensaje" Visible="false" />
    
</div>
                    </section>
                </div>

                <div id="panel-horario" class="content-panel">
                    <section class="placeholder-container">
                        <h2 class="section-title-panel">Mi Horario</h2>
                        <p>Aquí se mostrará el módulo para gestionar los horarios de atención (RF12).</p>
                        </section>
                </div>

                <div id="panel-pacientes" class="content-panel">
                    <section class="placeholder-container">
                        <h2 class="section-title-panel">Gestión de Pacientes</h2>
                        <p>Aquí se mostrará un buscador y una lista de pacientes para acceder al historial clínico (RF09).</p>
                        </section>
                </div>
                
                <div id="panel-perfil" class="content-panel">
                    <section class="placeholder-container">
                        <h2 class="section-title-panel">Mi Perfil</h2>
                        <p>Aquí se mostrará el formulario para editar datos personales y contraseña (RF03).</p>
                        </section>
                </div>
                
                </main>
        </div>
    </form>
    
    
</body>
</html>