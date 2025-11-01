<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SistemaMedico.vista.Login" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Iniciar Sesión - Clínica Aguirre</title>
    <link href="../styles/login.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <!-- Header -->
            <div class="login-header">
                <div class="login-icon">
                    <i class="fas fa-heartbeat"></i>
                </div>
                <h1 class="login-title">Iniciar Sesión</h1>
                <p class="login-subtitle">Accede a tu cuenta de la Clinica Aguirre</p>
            </div>

            <!-- Mensaje de Error -->
            <asp:Panel ID="pnlError" runat="server" CssClass="error-message" Visible="false">
                <asp:Literal ID="litError" runat="server"></asp:Literal>
            </asp:Panel>

            <!-- Formulario de Login -->
            <div class="form-group">
                <label class="form-label">Correo Electrónico</label>
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
                <label class="form-label">Contraseña</label>
                <asp:TextBox 
                    ID="txtPassword" 
                    runat="server" 
                    CssClass="form-input" 
                    placeholder="••••••••"
                    TextMode="Password"
                    required="required">
                </asp:TextBox>
            </div>

            <asp:Button 
                ID="btnLogin" 
                runat="server" 
                Text="Iniciar Sesión" 
                CssClass="btn-login" 
                OnClick="btnLogin_Click" />

            <!-- Link de Registro -->
            <div class="register-link">
                ¿No tienes cuenta? 
                <asp:HyperLink ID="lnkRegistro" runat="server" NavigateUrl="~/vista/Registro.aspx">
                    Regístrate aquí
                </asp:HyperLink>
            </div>

            <!-- Volver al Inicio -->
            <div class="back-link">
                <asp:HyperLink ID="lnkVolver" runat="server" NavigateUrl="~/vista/Index.aspx">
                    ← Volver al inicio
                </asp:HyperLink>
            </div>
        </div>
    </form>
</body>
</html>
