<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CrearUsuario.aspx.cs" Inherits="SistemaMedico.vista.CrearUsuario" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Crear Usuario - Sistema M�dico</title>
    
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
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .modal-container {
            background: white;
            border-radius: 16px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            max-width: 700px;
            width: 100%;
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

        .modal-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 25px 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-radius: 16px 16px 0 0;
        }

        .modal-header h2 {
            font-size: 24px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .btn-close {
            background: rgba(255,255,255,0.2);
            color: white;
            border: 2px solid white;
            width: 35px;
            height: 35px;
            border-radius: 50%;
            cursor: pointer;
            font-size: 18px;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: all 0.3s;
        }

        .btn-close:hover {
            background: white;
            color: #667eea;
            transform: rotate(90deg);
        }

        .modal-body {
            padding: 30px;
        }

        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-bottom: 20px;
        }

        .form-group {
            display: flex;
            flex-direction: column;
        }

        .form-group.full-width {
            grid-column: 1 / -1;
        }

        .form-group label {
            font-weight: 600;
            color: #333;
            margin-bottom: 8px;
            font-size: 14px;
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .required {
            color: #e74c3c;
        }

        .form-control {
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 14px;
            transition: border 0.3s;
            width: 100%;
            font-family: inherit;
        }

        .form-control:focus {
            outline: none;
            border-color: #667eea;
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

        .modal-footer {
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

        /* RESPONSIVE */
        @media (max-width: 768px) {
            .modal-container {
                max-height: 95vh;
            }

            .form-row {
                grid-template-columns: 1fr;
            }

            .modal-header h2 {
                font-size: 20px;
            }

            .modal-body {
                padding: 20px;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="modal-container">
            <!-- HEADER -->
            <div class="modal-header">
                <h2><i class="fas fa-user-plus"></i> Crear Nuevo Usuario</h2>
                <asp:Button ID="btnCerrar" runat="server" CssClass="btn-close" Text="�" OnClick="btnCerrar_Click" />
            </div>

            <!-- BODY -->
            <div class="modal-body">
                <!-- MENSAJE DE ERROR/�XITO -->
                <asp:Panel ID="pnlMensaje" runat="server" CssClass="alert" Visible="false">
                    <asp:Label ID="lblMensaje" runat="server"></asp:Label>
                </asp:Panel>

                <!-- INFORMACI�N B�SICA -->
                <div class="section-divider">
                    <i class="fas fa-user"></i> Informaci�n B�sica
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label>Nombre <span class="required">*</span></label>
                        <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Ej: Juan" MaxLength="100"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Apellido <span class="required">*</span></label>
                        <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" placeholder="Ej: P�rez Garc�a" MaxLength="100"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label>Tipo de Documento <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlTipoDoc" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Seleccione...</asp:ListItem>
                            <asp:ListItem Value="DNI">DNI</asp:ListItem>
                            <asp:ListItem Value="CE">Carnet de Extranjer�a</asp:ListItem>
                            <asp:ListItem Value="PASAPORTE">Pasaporte</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label>N�mero de Documento <span class="required">*</span></label>
                        <asp:TextBox ID="txtNumDoc" runat="server" CssClass="form-control" placeholder="Ej: 12345678" MaxLength="20"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label>G�nero <span class="required">*</span></label>
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

                <!-- INFORMACI�N DE CONTACTO -->
                <div class="section-divider">
                    <i class="fas fa-envelope"></i> Informaci�n de Contacto
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label>Email <span class="required">*</span></label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="ejemplo@correo.com" TextMode="Email" MaxLength="150"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Tel�fono <span class="required">*</span></label>
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
                        <label>Contrase�a <span class="required">*</span></label>
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="M�nimo 6 caracteres"></asp:TextBox>
                        <span class="info-text">La contrase�a debe tener al menos 6 caracteres</span>
                    </div>
                </div>

                <!-- ROL Y DATOS ESPEC�FICOS -->
                <div class="section-divider">
                    <i class="fas fa-user-tag"></i> Rol y Permisos
                </div>

                <div class="form-row">
                    <div class="form-group full-width">
                        <label>Rol <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlRol" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRol_SelectedIndexChanged">
                            <asp:ListItem Value="">Seleccione un rol...</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <!-- CAMPOS PARA M�DICO -->
                <asp:Panel ID="pnlMedico" runat="server" CssClass="dynamic-fields">
                    <div class="section-divider">
                        <i class="fas fa-stethoscope"></i> Informaci�n Profesional (M�dico)
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label>Especialidad <span class="required">*</span></label>
                            <asp:DropDownList ID="ddlEspecialidad" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Seleccione...</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label>N�mero de Colegiatura <span class="required">*</span></label>
                            <asp:TextBox ID="txtNumColegiatura" runat="server" CssClass="form-control" placeholder="Ej: CMP-12345" MaxLength="50"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-group">
                            <label>A�os de Experiencia <span class="required">*</span></label>
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
                                <asp:ListItem Value="1" Selected="True">Nivel 1 - B�sico</asp:ListItem>
                                <asp:ListItem Value="2">Nivel 2 - Intermedio</asp:ListItem>
                                <asp:ListItem Value="3">Nivel 3 - Avanzado (Super Admin)</asp:ListItem>
                            </asp:DropDownList>
                            <span class="info-text">
                                Nivel 1: Lectura y gesti�n b�sica | 
                                Nivel 2: Gesti�n completa de usuarios | 
                                Nivel 3: Acceso total al sistema
                            </span>
                        </div>
                    </div>
                </asp:Panel>

                <!-- CAMPOS PARA PACIENTE (Opcional) -->
                <asp:Panel ID="pnlPaciente" runat="server" CssClass="dynamic-fields">
                    <div class="section-divider">
                        <i class="fas fa-user-injured"></i> Informaci�n Adicional (Opcional)
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

            <!-- FOOTER -->
            <div class="modal-footer">
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" />
                <asp:Button ID="btnCrear" runat="server" Text="Crear Usuario" CssClass="btn btn-primary" OnClick="btnCrear_Click" />
            </div>
        </div>
    </form>
</body>
</html>
