<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="SistemaMedico.vista.Registro" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Crear Cuenta - Clínica Aguirre</title>
    <link href="../styles/registro.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <div class="registro-container">
            <div class="registro-header">
                <div class="registro-icon">
                    <i class="fas fa-user-plus"></i>
                </div>
                <h1 class="registro-title">Crear Cuenta</h1>
                <p class="registro-subtitle">Regístrate en la Clinica Aguirre</p>
            </div>

            <asp:Panel ID="pnlError" runat="server" CssClass="error-message" Visible="false">
                <asp:Literal ID="litError" runat="server"></asp:Literal>
            </asp:Panel>

            <div class="info-box">
                <i class="fas fa-info-circle"></i>
                <p>Podrás agendar citas y gestionar tu historial médico</p>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Nombre *</label>
                    <asp:TextBox 
                        ID="txtNombre" 
                        runat="server" 
                        CssClass="form-input" 
                        placeholder="Juan"
                        required="required">
                    </asp:TextBox>
                </div>

                <div class="form-group">
                    <label class="form-label">Apellido *</label>
                    <asp:TextBox 
                        ID="txtApellido" 
                        runat="server" 
                        CssClass="form-input" 
                        placeholder="Pérez"
                        required="required">
                    </asp:TextBox>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Tipo de Documento *</label>
                    <asp:DropDownList ID="ddlTipoDocumento" runat="server" CssClass="form-select">
                        <asp:ListItem Value="DNI" Selected="True">DNI</asp:ListItem>
                        <asp:ListItem Value="CE">Carnet de Extranjería</asp:ListItem>
                        <asp:ListItem Value="Pasaporte">Pasaporte</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group">
                    <label class="form-label">Número de Documento *</label>
                    <asp:TextBox 
                        ID="txtNumeroDocumento" 
                        runat="server" 
                        CssClass="form-input" 
                        placeholder="12345678"
                        required="required">
                    </asp:TextBox>
                </div>
            </div>

            <div class="form-group">
                <label class="form-label">Género *</label>
                <asp:DropDownList ID="ddlGenero" runat="server" CssClass="form-select">
                    <asp:ListItem Value="M">Masculino</asp:ListItem>
                    <asp:ListItem Value="F">Femenino</asp:ListItem>
                    <asp:ListItem Value="O" Selected="True">Otro</asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="form-group">
                <label class="form-label">Correo Electrónico *</label>
                <asp:TextBox 
                    ID="txtEmail" 
                    runat="server" 
                    CssClass="form-input" 
                    placeholder="tu@email.com"
                    TextMode="Email"
                    required="required">
                </asp:TextBox>
            </div>

            <div class="form-group">
                <label class="form-label">Teléfono</label>
                <asp:TextBox 
                    ID="txtTelefono" 
                    runat="server" 
                    CssClass="form-input" 
                    placeholder="987654321"
                    TextMode="Phone">
                </asp:TextBox>
            </div>

            <div class="form-group">
                <label class="form-label">Fecha de Nacimiento *</label>
                <asp:TextBox 
                    ID="txtFechaNacimiento" 
                    runat="server" 
                    CssClass="form-input" 
                    TextMode="Date"
                    required="required">
                </asp:TextBox>
            </div>

            <div class="form-group">
                <label class="form-label">Sede Preferida</label>
                <asp:DropDownList 
                    ID="DDLSedePref"
                    runat="server"
                    CssClass="form-input"
                    placeholder="Sede Preferida (Obligatorio)">
                </asp:DropDownList>
                <small class="form-help">Puedes dejarlo vacío o ingresar el nombre de tu sede preferida</small>
            </div>

            <div class="form-group">
                <label class="form-label">Contraseña *</label>
                <asp:TextBox 
                    ID="txtPassword" 
                    runat="server" 
                    CssClass="form-input" 
                    placeholder="••••••••"
                    TextMode="Password"
                    required="required">
                </asp:TextBox>
                <small class="form-help">Mínimo 6 caracteres</small>
            </div>

            <div class="form-group">
                <label class="form-label">Confirmar Contraseña *</label>
                <asp:TextBox 
                    ID="txtConfirmarPassword" 
                    runat="server" 
                    CssClass="form-input" 
                    placeholder="••••••••"
                    TextMode="Password"
                    required="required">
                </asp:TextBox>
            </div>

            <asp:Button 
                ID="btnRegistrar" 
                runat="server" 
                Text="Crear Cuenta" 
                CssClass="btn-registro" 
                OnClick="btnRegistrar_Click" />

            <div class="login-link">
                ¿Ya tienes cuenta? 
                <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/vista/Login.aspx">
                    Inicia sesión aquí
                </asp:HyperLink>
            </div>

            <div class="back-link">
                <asp:HyperLink ID="lnkVolver" runat="server" NavigateUrl="~/vista/Index.aspx">
                    ← Volver al inicio
                </asp:HyperLink>
            </div>
        </div>
    </form>
</body>
</html>
