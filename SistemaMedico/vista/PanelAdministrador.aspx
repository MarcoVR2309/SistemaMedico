<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PanelAdministrador.aspx.cs" Inherits="SistemaMedico.vista.PanelAdministrador" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Panel Administrador - Sistema Médico</title>
    
    <!-- Font Awesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
    
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: #f5f7fa;
            min-height: 100vh;
        }

        /* LAYOUT CON SIDEBAR */
        .dashboard-layout {
            display: flex;
            min-height: 100vh;
        }

        /* SIDEBAR */
        .dashboard-sidebar {
            width: 280px;
            background: linear-gradient(180deg, #667eea 0%, #764ba2 100%);
            color: white;
            display: flex;
            flex-direction: column;
            box-shadow: 4px 0 10px rgba(0,0,0,0.1);
            position: fixed;
            height: 100vh;
            overflow-y: auto;
            z-index: 100;
        }

        .sidebar-header {
            padding: 25px 20px;
            border-bottom: 1px solid rgba(255,255,255,0.1);
        }

        .navbar-brand {
            display: flex;
            align-items: center;
            gap: 12px;
            color: white;
            text-decoration: none;
        }

        .logo-icon {
            font-size: 32px;
            color: #ffd700;
        }

        .logo-text {
            font-size: 20px;
            font-weight: 700;
            letter-spacing: 0.5px;
        }

        .sidebar-profile {
            padding: 20px;
            background: rgba(0,0,0,0.1);
            display: flex;
            align-items: center;
            gap: 15px;
        }

        .profile-icon {
            font-size: 40px;
            color: white;
        }

        .profile-info {
            flex: 1;
        }

        .profile-welcome {
            font-size: 12px;
            opacity: 0.8;
            display: block;
        }

        .profile-name {
            font-size: 16px;
            font-weight: 600;
            display: block;
            margin-top: 2px;
        }

        /* NAVIGATION */
        .sidebar-nav {
            flex: 1;
            padding: 20px 0;
        }

        .sidebar-nav ul {
            list-style: none;
        }

        .nav-item {
            margin: 5px 0;
        }

        .nav-item a,
        .nav-item .nav-link {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 14px 25px;
            color: white;
            text-decoration: none;
            transition: all 0.3s;
            border-left: 3px solid transparent;
            cursor: pointer;
            background: transparent;
            border: none;
            width: 100%;
            text-align: left;
            font-size: 14px;
        }

        .nav-item a:hover,
        .nav-item .nav-link:hover,
        .nav-item.active a,
        .nav-item.active .nav-link {
            background: rgba(255,255,255,0.15);
            border-left-color: #ffd700;
        }

        .nav-item i {
            font-size: 18px;
            width: 20px;
        }

        .sidebar-footer {
            padding: 20px 0;
            border-top: 1px solid rgba(255,255,255,0.1);
        }

        /* MAIN CONTENT */
        .dashboard-content {
            margin-left: 280px;
            flex: 1;
            background: #f5f7fa;
            min-height: 100vh;
        }

        .content-header {
            background: white;
            padding: 25px 40px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            display: flex;
            justify-content: space-between;
            align-items: center;
            flex-wrap: wrap;
            gap: 20px;
        }

        .header-main h1 {
            font-size: 28px;
            color: #2c3e50;
            margin-bottom: 5px;
        }

        .header-main p {
            color: #7f8c8d;
            font-size: 14px;
        }

        .btn-create {
            background: #667eea;
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            transition: all 0.3s;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .btn-create:hover {
            background: #5568d3;
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }

        /* CONTENT AREA */
        .content {
            padding: 40px;
        }

        .content-panel {
            display: none;
        }

        .content-panel.active {
            display: block;
        }

        /* STATISTICS CARDS */
        .stats-container {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .stat-card {
            background: white;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            transition: transform 0.3s;
            border-left: 4px solid #667eea;
        }

        .stat-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 5px 20px rgba(0,0,0,0.1);
        }

        .stat-card .label {
            font-size: 14px;
            color: #7f8c8d;
            margin-bottom: 10px;
            font-weight: 500;
        }

        .stat-card .value {
            font-size: 32px;
            font-weight: bold;
            color: #2c3e50;
        }

        .stat-card.green {
            border-left-color: #11998e;
        }

        .stat-card.blue {
            border-left-color: #4facfe;
        }

        .stat-card.orange {
            border-left-color: #fa709a;
        }

        /* SEARCH AND FILTERS */
        .filters-container {
            background: white;
            padding: 25px;
            border-radius: 12px;
            margin-bottom: 30px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            display: grid;
            grid-template-columns: 2fr 1fr auto;
            gap: 15px;
            align-items: end;
        }

        .form-group {
            display: flex;
            flex-direction: column;
        }

        .form-group label {
            font-weight: 600;
            color: #333;
            margin-bottom: 8px;
            font-size: 14px;
        }

        .form-control {
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 14px;
            transition: border 0.3s;
            width: 100%;
        }

        .form-control:focus {
            outline: none;
            border-color: #667eea;
        }

        .btn-search {
            background: #667eea;
            color: white;
            border: none;
            padding: 12px 30px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            transition: all 0.3s;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .btn-search:hover {
            background: #5568d3;
        }

        /* TABLE */
        .table-container {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            overflow: hidden;
        }

        .users-table {
            width: 100%;
            border-collapse: collapse;
            background: white;
        }

        .users-table thead {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }

        .users-table th {
            padding: 15px;
            text-align: left;
            font-weight: 600;
            font-size: 14px;
        }

        .users-table td {
            padding: 15px;
            border-bottom: 1px solid #f0f0f0;
            font-size: 14px;
        }

        .users-table tbody tr {
            transition: background 0.3s;
        }

        .users-table tbody tr:hover {
            background: #f8f9fa;
        }

        /* BADGES */
        .badge {
            display: inline-block;
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
        }

        .badge-success {
            background: #d4edda;
            color: #155724;
        }

        .badge-danger {
            background: #f8d7da;
            color: #721c24;
        }

        .badge-paciente {
            background: #e3f2fd;
            color: #1565c0;
        }

        .badge-medico {
            background: #f3e5f5;
            color: #6a1b9a;
        }

        .badge-admin {
            background: #fff3e0;
            color: #e65100;
        }

        /* ACTION BUTTONS */
        .btn-action {
            padding: 8px 12px;
            border: none;
            border-radius: 6px;
            cursor: pointer;
            font-size: 12px;
            transition: all 0.3s;
            margin: 0 3px;
        }

        .btn-edit {
            background: #4facfe;
            color: white;
        }

        .btn-edit:hover {
            background: #3f9de8;
        }

        .btn-delete {
            background: #fa709a;
            color: white;
        }

        .btn-delete:hover {
            background: #e85d88;
        }

        /* MODAL STYLES */
        .modal-backdrop {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.6);
            z-index: 1000;
            justify-content: center;
            align-items: center;
            animation: fadeIn 0.3s ease;
        }

        .modal-backdrop.show {
            display: flex;
        }

        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }

        .modal-container-custom {
            background: white;
            border-radius: 16px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            max-width: 700px;
            width: 90%;
            max-height: 90vh;
            overflow-y: auto;
            animation: slideDown 0.3s ease-out;
        }

        @keyframes slideDown {
            from {
                opacity: 0;
                transform: translateY(-30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .modal-header-custom {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 25px 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-radius: 16px 16px 0 0;
        }

        .modal-header-custom h2 {
            font-size: 24px;
            display: flex;
            align-items: center;
            gap: 10px;
            margin: 0;
        }

        .btn-close-modal {
            background: rgba(255,255,255,0.2);
            color: white;
            border: 2px solid white;
            width: 35px;
            height: 35px;
            border-radius: 50%;
            cursor: pointer;
            font-size: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: all 0.3s;
            padding: 0;
        }

        .btn-close-modal:hover {
            background: white;
            color: #667eea;
            transform: rotate(90deg);
        }

        .modal-body-custom {
            padding: 30px;
        }

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-bottom: 20px;
        }

        .form-group.full-width {
            grid-column: 1 / -1;
        }

        .required {
            color: #e74c3c;
        }

        .form-control:disabled {
            background: #f5f5f5;
            cursor: not-allowed;
        }

        select.form-control {
            cursor: pointer;
        }

        .section-divider {
            margin: 30px 0 20px 0;
            padding-bottom: 10px;
            border-bottom: 2px solid #667eea;
            color: #667eea;
            font-weight: 600;
            font-size: 16px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .dynamic-fields {
            display: none;
        }

        .dynamic-fields.show {
            display: block;
        }

        .alert {
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            display: none;
        }

        .alert.show {
            display: block;
        }

        .alert-danger {
            background: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }

        .alert-success {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }

        .modal-footer-custom {
            padding: 20px 30px;
            background: #f8f9fa;
            display: flex;
            justify-content: flex-end;
            gap: 15px;
            border-radius: 0 0 16px 16px;
        }

        .btn {
            padding: 12px 30px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            transition: all 0.3s;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .btn-secondary {
            background: #6c757d;
            color: white;
        }

        .btn-secondary:hover {
            background: #5a6268;
        }

        .btn-primary {
            background: #667eea;
            color: white;
        }

        .btn-primary:hover {
            background: #5568d3;
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }

        .btn:disabled {
            opacity: 0.6;
            cursor: not-allowed;
        }

        .info-text {
            font-size: 12px;
            color: #666;
            margin-top: 5px;
            font-style: italic;
        }

        .placeholder-container {
            background: white;
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            text-align: center;
        }

        .placeholder-container h2 {
            color: #2c3e50;
            margin-bottom: 15px;
        }

        .placeholder-container p {
            color: #7f8c8d;
            font-size: 16px;
        }

        /* RESPONSIVE */
        @media (max-width: 1024px) {
            .dashboard-sidebar {
                width: 240px;
            }

            .dashboard-content {
                margin-left: 240px;
            }

            .filters-container {
                grid-template-columns: 1fr;
            }

            .stats-container {
                grid-template-columns: repeat(2, 1fr);
            }
        }

        @media (max-width: 768px) {
            .dashboard-sidebar {
                width: 100%;
                position: relative;
                height: auto;
            }

            .dashboard-content {
                margin-left: 0;
            }

            .content {
                padding: 20px;
            }

            .content-header {
                padding: 20px;
            }

            .stats-container {
                grid-template-columns: 1fr;
            }

            .form-row {
                grid-template-columns: 1fr;
            }

            .modal-container-custom {
                width: 95%;
                max-height: 95vh;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
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
                    <i class="fas fa-user-shield profile-icon"></i>
                    <div class="profile-info">
                        <span class="profile-welcome">Administrador</span>
                        <asp:Label ID="lblNombreAdmin" runat="server" Text="Admin" CssClass="profile-name"></asp:Label>
                    </div>
                </div>

                <nav class="sidebar-nav">
                    <ul>
                        <li class="nav-item active">
                            <a href="#" data-target="panel-usuarios">
                                <i class="fas fa-users"></i> Gestión de Cuentas
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="#" data-target="panel-especialidades">
                                <i class="fas fa-stethoscope"></i> Especialidades
                            </a>
                        </li>
                        <li class="nav-item">
                            <a href="#" data-target="panel-sedes">
                                <i class="fas fa-building"></i> Sedes
                            </a>
                        </li>
                    </ul>
                </nav>

                <div class="sidebar-footer">
                    <ul>
                        <li class="nav-item">
                            <asp:LinkButton ID="btnCerrarSesion" runat="server" OnClick="btnCerrarSesion_Click" CssClass="nav-link">
                                <i class="fas fa-sign-out-alt"></i> Cerrar Sesión
                            </asp:LinkButton>
                        </li>
                    </ul>
                </div>
            </aside>

            <!-- MAIN CONTENT -->
            <main class="dashboard-content">
                
                <!-- PANEL USUARIOS (GESTIÓN DE CUENTAS) -->
                <div id="panel-usuarios" class="content-panel active">
                    <header class="content-header">
                        <div class="header-main">
                            <h1>Gestión de Cuentas</h1>
                            <p>Recursos Humanos - Administración de usuarios del sistema</p>
                        </div>
                        <div class="header-actions">
                            <button type="button" class="btn-create" onclick="abrirModalCrearUsuario()">
                                <i class="fas fa-user-plus"></i> Crear Usuario
                            </button>
                        </div>
                    </header>

                    <div class="content">
                        <!-- STATISTICS -->
                        <div class="stats-container">
                            <div class="stat-card">
                                <div class="label">Total Usuarios</div>
                                <div class="value"><asp:Label ID="lblTotalUsuarios" runat="server" Text="0"></asp:Label></div>
                            </div>
                            <div class="stat-card green">
                                <div class="label">Doctores</div>
                                <div class="value"><asp:Label ID="lblDoctores" runat="server" Text="0"></asp:Label></div>
                            </div>
                            <div class="stat-card blue">
                                <div class="label">Pacientes</div>
                                <div class="value"><asp:Label ID="lblPacientes" runat="server" Text="0"></asp:Label></div>
                            </div>
                            <div class="stat-card orange">
                                <div class="label">Activos</div>
                                <div class="value"><asp:Label ID="lblActivos" runat="server" Text="0"></asp:Label></div>
                            </div>
                        </div>

                        <!-- FILTERS -->
                        <div class="filters-container">
                            <div class="form-group">
                                <label><i class="fas fa-search"></i> Buscar por nombre o email</label>
                                <asp:TextBox ID="txtBusqueda" runat="server" CssClass="form-control" placeholder="Buscar por nombre, apellido o email..."></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label><i class="fas fa-filter"></i> Filtrar por Rol</label>
                                <asp:DropDownList ID="ddlFiltroRol" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="" Text="Todos los roles"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn-search" OnClick="btnBuscar_Click" />
                        </div>

                        <!-- USERS TABLE -->
                        <div class="table-container">
                            <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False" CssClass="users-table" 
                                          OnRowCommand="gvUsuarios_RowCommand" DataKeyNames="ID">
                                <Columns>
                                    <asp:BoundField DataField="NOM" HeaderText="Usuario" />
                                    <asp:BoundField DataField="APE" HeaderText="Apellido" />
                                    <asp:BoundField DataField="EMAIL" HeaderText="Email" />
                                    <asp:TemplateField HeaderText="Rol">
                                        <ItemTemplate>
                                            <span class='badge badge-<%# GetRolClass(Eval("Rol").ToString()) %>'>
                                                <%# Eval("Rol") %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TEL" HeaderText="Teléfono" />
                                    <asp:TemplateField HeaderText="Estado">
                                        <ItemTemplate>
                                            <span class='badge badge-<%# Convert.ToBoolean(Eval("Activo")) ? "success" : "danger" %>'>
                                                <%# Convert.ToBoolean(Eval("Activo")) ? "Activo" : "Inactivo" %>
                                            </span>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="FechaRegistro" HeaderText="Fecha Registro" DataFormatString="{0:dd/MM/yyyy}" />
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <asp:Button ID="btnEditar" runat="server" Text="Editar" CssClass="btn-action btn-edit" 
                                                        CommandName="Editar" CommandArgument='<%# Eval("ID") %>' />
                                            <asp:Button ID="btnEliminar" runat="server" Text="Desactivar" CssClass="btn-action btn-delete" 
                                                        CommandName="Desactivar" CommandArgument='<%# Eval("ID") %>' 
                                                        OnClientClick="return confirm('¿Está seguro de desactivar este usuario?');" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div style="text-align: center; padding: 40px; color: #999;">
                                        <i class="fas fa-users" style="font-size: 48px; margin-bottom: 15px;"></i>
                                        <p>No se encontraron usuarios</p>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </div>

                <!-- PANEL ESPECIALIDADES -->
                <div id="panel-especialidades" class="content-panel">
                    <header class="content-header">
                        <div class="header-main">
                            <h1>Administrar Especialidades Médicas</h1>
                            <p>Gestiona las especialidades disponibles en la clínica</p>
                        </div>
                        <div class="header-actions">
                            <button type="button" class="btn-create" onclick="abrirModalEspecialidad()">
                                <i class="fas fa-plus"></i> Nueva Especialidad
                            </button>
                        </div>
                    </header>

                    <div class="content">
                        <!-- STATISTICS -->
                        <div class="stats-container">
                            <div class="stat-card">
                                <div class="label">Total Especialidades</div>
                                <div class="value"><asp:Label ID="lblTotalEspecialidades" runat="server" Text="0"></asp:Label></div>
                            </div>
                            <div class="stat-card green">
                                <div class="label">Especialidades Activas</div>
                                <div class="value"><asp:Label ID="lblEspecialidadesActivas" runat="server" Text="0"></asp:Label></div>
                            </div>
                            <div class="stat-card blue">
                                <div class="label">Total Doctores</div>
                                <div class="value"><asp:Label ID="lblTotalDoctores" runat="server" Text="0"></asp:Label></div>
                            </div>
                        </div>

                        <!-- ESPECIALIDADES TABLE -->
                        <div class="table-container">
                            <asp:GridView ID="gvEspecialidades" runat="server" AutoGenerateColumns="False" CssClass="users-table" 
                                          OnRowCommand="gvEspecialidades_RowCommand" DataKeyNames="ID">
                                <Columns>
                                    <asp:BoundField DataField="NombreEspecialidad" HeaderText="Especialidad" />
                                    <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                                    <asp:BoundField DataField="TotalDoctores" HeaderText="Total Doctores" />
                                    <asp:BoundField DataField="DoctoresActivos" HeaderText="Doctores Activos" />
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <asp:Button ID="btnEditarEsp" runat="server" Text="Editar" CssClass="btn-action btn-edit" 
                                                        CommandName="Editar" CommandArgument='<%# Eval("ID") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div style="text-align: center; padding: 40px; color: #999;">
                                        <i class="fas fa-stethoscope" style="font-size: 48px; margin-bottom: 15px;"></i>
                                        <p>No se encontraron especialidades</p>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </div>

                <!-- PANEL SEDES -->
                <div id="panel-sedes" class="content-panel">
                    <header class="content-header">
                        <div class="header-main">
                            <h1>Gestión de Sedes</h1>
                            <p>Administra las ubicaciones físicas de la clínica</p>
                        </div>
                        <div class="header-actions">
                            <button type="button" class="btn-create" onclick="abrirModalSede()">
                                <i class="fas fa-plus"></i> Nueva Sede
                            </button>
                        </div>
                    </header>

                    <div class="content">
                        <!-- STATISTICS -->
                        <div class="stats-container">
                            <div class="stat-card">
                                <div class="label">Total Sedes</div>
                                <div class="value"><asp:Label ID="lblTotalSedes" runat="server" Text="0"></asp:Label></div>
                            </div>
                            <div class="stat-card green">
                                <div class="label">Sedes Activas</div>
                                <div class="value"><asp:Label ID="lblSedesActivas" runat="server" Text="0"></asp:Label></div>
                            </div>
                        </div>

                        <!-- SEDES TABLE -->
                        <div class="table-container">
                            <asp:GridView ID="gvSedes" runat="server" AutoGenerateColumns="False" CssClass="users-table" 
                                          OnRowCommand="gvSedes_RowCommand" DataKeyNames="ID">
                                <Columns>
                                    <asp:BoundField DataField="NombreSede" HeaderText="Sede" />
                                    <asp:BoundField DataField="Direccion" HeaderText="Dirección" />
                                    <asp:BoundField DataField="UsuariosPrefieren" HeaderText="Usuarios Prefieren" />
                                    <asp:BoundField DataField="TotalCitas" HeaderText="Total Citas" />
                                    <asp:TemplateField HeaderText="Acciones">
                                        <ItemTemplate>
                                            <asp:Button ID="btnEditarSede" runat="server" Text="Editar" CssClass="btn-action btn-edit" 
                                                        CommandName="Editar" CommandArgument='<%# Eval("ID") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div style="text-align: center; padding: 40px; color: #999;">
                                        <i class="fas fa-building" style="font-size: 48px; margin-bottom: 15px;"></i>
                                        <p>No se encontraron sedes</p>
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </div>

            </main>
        </div>

        <!-- MODAL CREAR USUARIO -->
        <div id="modalCrearUsuario" class="modal-backdrop">
            <div class="modal-container-custom">

                <!-- HEADER -->
                <div class="modal-header-custom">
                    <h2><i class="fas fa-user-plus"></i> Crear Nuevo Usuario</h2>
                    <button type="button" class="btn-close-modal" onclick="cerrarModalCrearUsuario()">×</button>
                </div>

                <!-- BODY -->
                <div class="modal-body-custom">
                    <!-- MENSAJE DE ERROR/ÉXITO -->
                    <asp:Panel ID="pnlMensaje" runat="server" CssClass="alert" Visible="false">
                        <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                    </asp:Panel>

                    <!-- INFORMACIÓN BÁSICA -->
                    <div class="section-divider">
                        <i class="fas fa-user"></i> Información Básica
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label>Nombre <span class="required">*</span></label>
                            <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Ej: Juan" MaxLength="100"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>Apellido <span class="required">*</span></label>
                            <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" placeholder="Ej: Pérez García" MaxLength="100"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label>Tipo de Documento <span class="required">*</span></label>
                            <asp:DropDownList ID="ddlTipoDoc" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Seleccione...</asp:ListItem>
                                <asp:ListItem Value="DNI">DNI</asp:ListItem>
                                <asp:ListItem Value="CE">Carnet de Extranjería</asp:ListItem>
                                <asp:ListItem Value="PASAPORTE">Pasaporte</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>Número de Documento <span class="required">*</span></label>
                            <asp:TextBox ID="txtNumDoc" runat="server" CssClass="form-control" placeholder="Ej: 12345678" MaxLength="20"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label>Género <span class="required">*</span></label>
                            <asp:DropDownList ID="ddlGenero" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Seleccione...</asp:ListItem>
                                <asp:ListItem Value="M">Masculino</asp:ListItem>
                                <asp:ListItem Value="F">Femenino</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>Fecha de Nacimiento <span class="required">*</span></label>
                            <asp:TextBox ID="txtFechaNac" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                    </div>

                    <!-- INFORMACIÓN DE CONTACTO -->
                    <div class="section-divider">
                        <i class="fas fa-envelope"></i> Información de Contacto
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label>Email <span class="required">*</span></label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="ejemplo@correo.com" TextMode="Email" MaxLength="150"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label>Teléfono <span class="required">*</span></label>
                            <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" placeholder="987654321" MaxLength="20"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label>Sede Preferida</label>
                            <asp:DropDownList ID="ddlSede" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Seleccione...</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>Contraseña <span class="required">*</span></label>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Mínimo 6 caracteres"></asp:TextBox>
                            <span class="info-text">La contraseña debe tener al menos 6 caracteres</span>
                        </div>
                    </div>

                    <!-- ROL Y DATOS ESPECÍFICOS -->
                    <div class="section-divider">
                        <i class="fas fa-user-tag"></i> Rol y Permisos
                    </div>

                    <div class="form-row">
                        <div class="form-group full-width">
                            <label>Rol <span class="required">*</span></label>
                            <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-control" onchange="cambiarRol()">
                                <asp:ListItem Value="">Seleccione un rol...</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <!-- CAMPOS PARA MÉDICO -->
                    <asp:Panel ID="pnlMedico" runat="server" CssClass="dynamic-fields">
                        <div class="section-divider">
                            <i class="fas fa-stethoscope"></i> Información Profesional (Médico)
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label>Especialidad <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlEspecialidad" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Seleccione...</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label>Número de Colegiatura <span class="required">*</span></label>
                                <asp:TextBox ID="txtNumColegiatura" runat="server" CssClass="form-control" placeholder="Ej: CMP-12345" MaxLength="50"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label>Años de Experiencia <span class="required">*</span></label>
                                <asp:TextBox ID="txtExperiencia" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ej: 5" min="0" max="50"></asp:TextBox>
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- CAMPOS PARA ADMINISTRADOR -->
                    <asp:Panel ID="pnlAdministrador" runat="server" CssClass="dynamic-fields">
                        <div class="section-divider">
                            <i class="fas fa-user-shield"></i> Permisos de Administrador
                        </div>

                        <div class="form-row">
                            <div class="form-group full-width">
                                <label>Nivel de Acceso <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlNivelAcceso" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="1" Selected="True">Nivel 1 - Básico</asp:ListItem>
                                    <asp:ListItem Value="2">Nivel 2 - Intermedio</asp:ListItem>
                                    <asp:ListItem Value="3">Nivel 3 - Avanzado (Super Admin)</asp:ListItem>
                                </asp:DropDownList>
                                <span class="info-text">
                                    Nivel 1: Lectura y gestión básica | 
                                    Nivel 2: Gestión completa de usuarios | 
                                    Nivel 3: Acceso total al sistema
                                </span>
                            </div>
                        </div>
                    </asp:Panel>

                    <!-- CAMPOS PARA PACIENTE (Opcional) -->
                    <asp:Panel ID="pnlPaciente" runat="server" CssClass="dynamic-fields">
                        <div class="section-divider">
                            <i class="fas fa-user-injured"></i> Información Adicional (Opcional)
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label>Peso (kg)</label>
                                <asp:TextBox ID="txtPeso" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ej: 70" min="0" max="300" step="0.1"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>Edad</label>
                                <asp:TextBox ID="txtEdad" runat="server" CssClass="form-control" TextMode="Number" placeholder="Ej: 30" min="0" max="120"></asp:TextBox>
                            </div>
                        </div>
                    </asp:Panel>
                </div>

                <!-- MODAL CREAR ESPECIALIDAD -->
                <div id="modalCrearEspecialidad" class="modal-backdrop">
                    <div class="modal-container-custom">
                        <!-- HEADER -->
                        <div class="modal-header-custom">
                            <h2><i class="fas fa-plus"></i> Nueva Especialidad Médica</h2>
                            <button type="button" class="btn-close-modal" onclick="cerrarModalCrearEspecialidad()">×</button>
                        </div>

                        <!-- BODY -->
                        <div class="modal-body-custom">
                            <!-- MENSAJE DE ERROR/ÉXITO -->
                            <asp:Panel ID="pnlMensajeEspecialidad" runat="server" CssClass="alert" Visible="false">
                                <asp:Label ID="lblMensajeEspecialidad" runat="server"></asp:Label>
                            </asp:Panel>

                            <!-- INFORMACIÓN BÁSICA -->
                            <div class="section-divider">
                                <i class="fas fa-stethoscope"></i> Datos de la Especialidad
                            </div>

                            <div class="form-row">
                                <div class="form-group full-width">
                                    <label>Nombre de la Especialidad <span class="required">*</span></label>
                                    <asp:TextBox ID="txtNombreEspecialidad" runat="server" CssClass="form-control" placeholder="Ej: Cardiología" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>

                            <div class="form-row">
                                <div class="form-group full-width">
                                    <label>Descripción de la Especialidad</label>
                                    <asp:TextBox ID="txtDescripcionEspecialidad" runat="server" CssClass="form-control" placeholder="Descripción breve" TextMode="MultiLine" Rows="3" MaxLength="250"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <!-- FOOTER -->
                        <div class="modal-footer-custom">
                            <button type="button" class="btn btn-secondary" onclick="cerrarModalCrearEspecialidad()">Cancelar</button>
                            <asp:Button ID="btnCrearEspecialidad" runat="server" Text="Crear Especialidad" CssClass="btn btn-primary" OnClick="btnCrearEspecialidad_Click" />
                        </div>
                    </div>
                </div>

                <!-- MODAL CREAR SEDE -->
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
                                    <asp:TextBox ID="txtNombreSede" runat="server" CssClass="form-control" placeholder="Ej: Sede Principal" MaxLength="200"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label>Teléfono</label>
                                    <asp:TextBox ID="txtTelefonoSede" runat="server" CssClass="form-control" placeholder="987654321" MaxLength="20"></asp:TextBox>
                                </div>
                            </div>

                            <div class="form-group">
                                <label>Dirección</label>
                                <asp:TextBox ID="txtDireccionSede" runat="server" CssClass="form-control" placeholder="Av. Principal 123" MaxLength="300"></asp:TextBox>
                            </div>

                            <div class="form-group">
                                <label>Email</label>
                                <asp:TextBox ID="txtEmailSede" runat="server" CssClass="form-control" TextMode="Email" placeholder="sede@clinica.com" MaxLength="150"></asp:TextBox>
                            </div>
                        </div>

                        <div class="modal-footer-custom">
                            <button type="button" class="btn btn-secondary" onclick="cerrarModalSede()">Cancelar</button>
                            <asp:Button ID="btnCrearSede" runat="server" Text="Crear Sede" CssClass="btn btn-primary" OnClick="btnCrearSede_Click" />
                        </div>
                    </div>
                </div>
            </form>

    <script type="text/javascript">
        // Navegación entre paneles
        document.addEventListener('DOMContentLoaded', function() {
            const navItems = document.querySelectorAll('.sidebar-nav .nav-item a');
            const panels = document.querySelectorAll('.content-panel');

            navItems.forEach(item => {
                item.addEventListener('click', function(e) {
                    e.preventDefault();
                    
                    // Remover clase active de todos los items
                    navItems.forEach(nav => nav.parentElement.classList.remove('active'));
                    
                    // Agregar clase active al item actual
                    this.parentElement.classList.add('active');
                    
                    // Ocultar todos los paneles
                    panels.forEach(panel => panel.classList.remove('active'));
                    
                    // Mostrar el panel correspondiente
                    const targetId = this.getAttribute('data-target');
                    const targetPanel = document.getElementById(targetId);
                    if (targetPanel) {
                        targetPanel.classList.add('active');
                    }
                });
            });
        });

        // Funciones para modal de usuarios
        function abrirModalCrearUsuario() {
            document.getElementById('modalCrearUsuario').classList.add('show');
            document.body.style.overflow = 'hidden';
        }

        function cerrarModalCrearUsuario() {
            document.getElementById('modalCrearUsuario').classList.remove('show');
            document.body.style.overflow = 'auto';
        }

        // Funciones para modal de especialidades
        function abrirModalEspecialidad() {
            document.getElementById('modalCrearEspecialidad').classList.add('show');
            document.body.style.overflow = 'hidden';
        }

        function cerrarModalCrearEspecialidad() {
            document.getElementById('modalCrearEspecialidad').classList.remove('show');
            document.body.style.overflow = 'auto';
        }

        // Funciones para modal de sedes
        function abrirModalSede() {
            document.getElementById('modalSede').classList.add('show');
            document.body.style.overflow = 'hidden';
        }

        function cerrarModalSede() {
            document.getElementById('modalSede').classList.remove('show');
            document.body.style.overflow = 'auto';
        }

        // Cerrar modales al hacer clic fuera
        window.onclick = function(event) {
            if (event.target.classList.contains('modal-backdrop')) {
                event.target.classList.remove('show');
                document.body.style.overflow = 'auto';
            }
        }

        // Evitar que los modales se cierren al hacer clic dentro
        document.addEventListener('DOMContentLoaded', function() {
            var modalContents = document.querySelectorAll('.modal-container-custom');
            modalContents.forEach(function(content) {
                content.onclick = function(event) {
                    event.stopPropagation();
                }
            });
        });
    </script>
</body>
</html>
