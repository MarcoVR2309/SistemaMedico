<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="SistemaMedico.vista.Index" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="UTF-8" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <title>Clínica Aguirre - Sistema Médico</title>
    <link href="../styles/main.css" rel="stylesheet" type="text/css" />
    
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"/>
    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap" rel="stylesheet"/>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Navbar -->
        <nav class="navbar">
            <div class="container">
                <div class="navbar-brand">
                    <i class="fas fa-heartbeat logo-icon"></i>
                    <span class="logo-text">Clínica Aguirre</span>
                </div>
                <ul class="navbar-menu">
                    <li><a href="#staff-medico">Staff Médico</a></li>
                    <li><a href="#nosotros">Nosotros</a></li>
                    <li><a href="#servicios">Servicios</a></li>
                    <li><a href="#pacientes" class="btn-primary">Pacientes</a></li>
                </ul>
                <div class="navbar-contact">
                    <span><i class="fas fa-phone"></i> 01-234-5678</span>
                    <span><i class="fas fa-phone"></i> 01-987-6543</span>
                </div>
            </div>
        </nav>

        <!-- Hero Section -->
        <section class="hero-section">
            <div class="container hero-container">
                <div class="hero-content">
                    <span class="hero-badge">QUEREMOS VERTE BIEN</span>
                    <h1 class="hero-title">
                        Tu salud en las <span class="highlight">mejores manos</span>
                    </h1>
                    <p class="hero-description">
                        Especialistas con amplia experiencia y la mejor tecnología en cirugías 
                        para el cuidado integral de tu salud y bienestar.
                    </p>
                    <asp:Button ID="btnRegistro" runat="server" Text="Regístrate Aquí" CssClass="btn-hero" OnClick="btnRegistro_Click" />
                </div>
                <div class="hero-image">
                    <img src="../vista/images/medical_team.png" alt="Equipo Médico" />
                </div>
            </div>
        </section>

        <!-- Services Section -->
        <section class="services-section">
            <div class="container">
                <h2 class="section-title">¿En qué podemos ayudarte hoy?</h2>
                <div class="services-grid">
                    <!-- Pacientes -->
                    <div class="service-card">
                        <div class="service-icon icon-blue">
                            <i class="fas fa-bed"></i>
                        </div>
                        <h3 class="service-title">Pacientes</h3>
                        <p class="service-description">
                            Atención integral en bienestar y salud de tu familia
                        </p>
                        <asp:Button ID="btnAcceder" runat="server" Text="Acceder" CssClass="btn-service btn-teal" OnClick="btnAcceder_Click" />
                    </div>

                    <!-- Médicos -->
                    <div class="service-card">
                        <div class="service-icon icon-purple">
                            <i class="fas fa-flask"></i>
                        </div>
                        <h3 class="service-title">Médicos</h3>
                        <p class="service-description">
                            Portal profesional para gestión de consultas médicas
                        </p>
                        <asp:Button ID="btnPortalMedico" runat="server" Text="Portal Médico" CssClass="btn-service btn-purple" OnClick="btnPortalMedico_Click" />
                    </div>

                    <!-- Servicios -->
                    <div class="service-card">
                        <div class="service-icon icon-teal">
                            <i class="fas fa-briefcase-medical"></i>
                        </div>
                        <h3 class="service-title">Servicios</h3>
                        <p class="service-description">
                            Conoce nuestros servicios médicos especializados
                        </p>
                        <asp:Button ID="btnVerServicios" runat="server" Text="Ver Servicios" CssClass="btn-service btn-teal" OnClick="btnVerServicios_Click" />
                    </div>

                    <!-- Staff Médico -->
                    <div class="service-card">
                        <div class="service-icon icon-green">
                            <i class="fas fa-user-md"></i>
                        </div>
                        <h3 class="service-title">Staff Médico</h3>
                        <p class="service-description">
                            Conoce a nuestros doctores especialistas
                        </p>
                        <asp:Button ID="btnVerMedicos" runat="server" Text="Ver Médicos" CssClass="btn-service btn-teal" OnClick="btnVerMedicos_Click" />
                    </div>
                </div>
            </div>
        </section>

        <!-- Footer -->
        <footer class="footer">
            <div class="container">
                <div class="footer-content">
                    <div class="footer-brand">
                        <i class="fas fa-heartbeat logo-icon"></i>
                        <span class="logo-text">Clínica Aguirre</span>
                    </div>
                    <p class="footer-text">© 2025 Clínica Aguirre. Todos los derechos reservados.</p>
                    <div class="footer-contact">
                        <span><i class="fas fa-phone"></i> 01-234-5678</span>
                        <span><i class="fas fa-phone"></i> 01-987-6543</span>
                        <span><i class="fas fa-envelope"></i> info@clinicaaguirre.com</span>
                    </div>
                </div>
            </div>
        </footer>
    </form>
</body>
</html>
