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
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js"></script>
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
                        <p class="content-subtitle">
    Citas programadas para hoy: <asp:Label ID="lblFechaHoy" runat="server"></asp:Label>
</p>
                    </div>
                    <div class="header-actions">
                         <%-- <a href="#" id="btnAbrirModalCita" class="btn-service btn-purple">+ Agendar Cita</a> --%>
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

<div id="modalVerFicha" class="modal-backdrop">
    <div class="modal-content" style="max-width: 800px;">

        <div id="areaImprimibleFicha" style="background-color: #fff;"> <div class="modal-header" style="background-color: var(--light-blue); border-bottom: 2px solid var(--primary-blue);">
                <div style="display: flex; align-items: center; gap: 15px;">
                    <div class="logo-circle" style="background: white; padding: 10px; border-radius: 50%; width: 50px; height: 50px; display: flex; align-items: center; justify-content: center; color: var(--primary-blue);">
                        <i class="fas fa-heartbeat fa-2x"></i>
                    </div>
                    <div>
                        <h3 class="section-title-panel" style="margin: 0; font-size: 1.4rem;">Ficha Médica Digital</h3>
                        <small style="color: var(--gray-text);">Clínica Aguirre - Informe de Consulta</small>
                    </div>
                </div>
                </div>

            <div class="modal-body" style="padding: 2rem 3rem;">

                <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 2rem; margin-bottom: 2rem; padding-bottom: 2rem; border-bottom: 1px dashed #CCC;">

                    <div>
                        <h4 style="color: var(--primary-purple); margin-bottom: 1rem; text-transform: uppercase; font-size: 0.9rem; letter-spacing: 1px;">Paciente</h4>
                        <div class="info-row" style="margin-bottom: 8px;">
                            <strong>Nombre:</strong> <asp:Label ID="lblFichaPaciente" runat="server" Text="-" />
                        </div>
                        <div class="info-row" style="margin-bottom: 8px;">
                            <strong>DNI:</strong> <asp:Label ID="lblFichaDNI" runat="server" Text="-" />
                        </div>
                        <div class="info-row" style="margin-bottom: 8px;">
                            <strong>Teléfono:</strong> <asp:Label ID="lblFichaTelefono" runat="server" Text="-" />
                        </div>
                        <div class="info-row">
                            <strong>Peso:</strong> <asp:Label ID="lblFichaPeso" runat="server" Text="-" /> kg
                        </div>
                    </div>

                    <div>
                        <h4 style="color: var(--primary-teal); margin-bottom: 1rem; text-transform: uppercase; font-size: 0.9rem; letter-spacing: 1px;">Detalles de la Cita</h4>
                        <div class="info-row" style="margin-bottom: 8px;">
                            <strong>Fecha:</strong> <asp:Label ID="lblFichaFecha" runat="server" Text="-" />
                        </div>
                        <div class="info-row" style="margin-bottom: 8px;">
                            <strong>Sede:</strong> <asp:Label ID="lblFichaSede" runat="server" Text="-" />
                        </div>
                        <div class="info-row" style="margin-bottom: 8px;">
                            <strong>Especialidad:</strong> <asp:Label ID="lblFichaEspecialidad" runat="server" Text="-" />
                        </div>
                        <div class="info-row">
                            <strong>Estado:</strong> <asp:Label ID="lblFichaEstado" runat="server" Text="-" Font-Bold="true" />
                        </div>
                    </div>
                </div>

                <div style="background-color: #F9FAFB; padding: 1.5rem; border-radius: 12px; border: 1px solid #E5E7EB;">
                    <h4 style="color: var(--dark-text); margin-bottom: 1.5rem; border-bottom: 2px solid var(--primary-blue); display: inline-block; padding-bottom: 5px;">Resumen Clínico</h4>

                    <div style="margin-bottom: 1.5rem;">
                        <strong style="display: block; color: var(--gray-text); font-size: 0.9rem; margin-bottom: 5px;">Motivo de Consulta:</strong>
                        <asp:Label ID="lblFichaMotivo" runat="server" Text="-" Style="display: block; font-style: italic;" />
                    </div>

                    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 20px;">
                        <div>
                            <strong style="color: var(--primary-blue);">Diagnóstico:</strong>
                            <p style="margin-top: 5px; line-height: 1.6;">
                                <asp:Label ID="lblFichaDiagnostico" runat="server" Text="-" />
                            </p>
                        </div>
                        <div>
                            <strong style="color: var(--primary-teal);">Tratamiento:</strong>
                            <p style="margin-top: 5px; line-height: 1.6;">
                                <asp:Label ID="lblFichaTratamiento" runat="server" Text="-" />
                            </p>
                        </div>
                    </div>

                    <div style="margin-top: 1.5rem;">
                        <strong>Observaciones:</strong>
                        <p style="margin-top: 5px;">
                            <asp:Label ID="lblFichaObservaciones" runat="server" Text="-" />
                        </p>
                    </div>
                </div>
                
                <div style="margin-top: 3rem; text-align: center; color: #999; font-size: 0.8rem; border-top: 1px solid #eee; padding-top: 10px;">
                    <p>Este documento es un reporte médico oficial generado por el Sistema de Clínica Aguirre.</p>
                    <small>ID Cita: <asp:Label ID="lblFichaId" runat="server" /></small>
                </div>

            </div>
        </div>
        <span class="modal-close" id="spanCerrarModalFicha" style="position: absolute; top: 20px; right: 25px; z-index: 10;">&times;</span>

        <div class="modal-footer" style="justify-content: flex-end; align-items: center; gap: 10px;">
            <button type="button" class="btn-service-outline btn-teal" onclick="descargarFichaPDF()">
                <i class="fas fa-file-pdf"></i> Descargar PDF
            </button>

            <button type="button" id="btnCerrarFichaInferior" class="btn-service btn-purple">Cerrar</button>
        </div>

    </div>
</div>

                    </section>
                </div>

                <div id="panel-horario" class="content-panel">
    <div class="schedule-container">
        <div class="schedule-header">
            <h2 class="section-title-panel">Agenda Semanal</h2>
            <div class="schedule-controls">
                <span class="current-week-label">Semana Actual</span>
            </div>
        </div>
        <div class="schedule-grid-wrapper">
            <div class="schedule-placeholder">
                <i class="far fa-calendar-alt fa-3x" style="color: var(--gray-text); margin-bottom: 1rem;"></i>
                <p>La visualización del horario se cargará aquí próximamente.</p>
            </div>
        </div>
    </div>
</div>

                <div id="panel-pacientes" class="content-panel">
                    <section class="placeholder-container">
                        <h2 class="section-title-panel">Gestión de Pacientes</h2>
                        <p>Aquí se mostrará un buscador y una lista de pacientes para acceder al historial clínico (RF09).</p>
                        </section>
                </div>
                



                <div id="panel-perfil" class="content-panel">
    
    <div class="profile-container">
        
        <div class="profile-header-card">
            <div class="profile-avatar">
                <i class="fas fa-user-md"></i>
            </div>
            <div class="profile-title-info">
                <h2 class="profile-name-display"><asp:Label ID="lblPerfilNombreCompleto" runat="server" Text="Cargando..." /></h2>
                <span class="profile-role-badge">Médico Especialista</span>
            </div>
            <div class="profile-header-actions">
                 <asp:Button ID="btnEditarPerfil" runat="server" Text="Editar Datos" CssClass="btn-service-outline btn-purple" OnClientClick="return false;" />
            </div>
        </div>

        <div class="profile-info-grid">
            
            <div class="info-card">
                <h3 class="info-card-title"><i class="far fa-id-card"></i> Información Personal</h3>
                
                <div class="detail-row">
                    <span class="detail-label">Correo Electrónico</span>
                    <asp:Label ID="lblPerfilEmail" runat="server" CssClass="detail-value" />
                </div>
                <div class="detail-row">
                    <span class="detail-label">Teléfono de Contacto</span>
                    <asp:Label ID="lblPerfilTelefono" runat="server" CssClass="detail-value" />
                </div>
            </div>

            <div class="info-card">
                <h3 class="info-card-title"><i class="fas fa-briefcase-medical"></i> Datos Profesionales</h3>
                
                <div class="detail-row">
                    <span class="detail-label">Especialidad</span>
                    <asp:Label ID="lblPerfilEspecialidad" runat="server" CssClass="detail-value" />
                </div>
                <div class="detail-row">
                    <span class="detail-label">N° Colegiatura (CMP)</span>
                    <asp:Label ID="lblPerfilCMP" runat="server" CssClass="detail-value" />
                </div>
                <div class="detail-row">
                    <span class="detail-label">Años de Experiencia</span>
                    <asp:Label ID="lblPerfilExperiencia" runat="server" CssClass="detail-value" />
                </div>
                 <div class="detail-row">
                    <span class="detail-label">ID Sistema</span>
                    <asp:Label ID="lblPerfilID" runat="server" CssClass="detail-value code-text" />
                </div>
            </div>

        </div>
    </div>
</div>
                



                </main>
        </div>
    </form>
    
    
</body>
</html>