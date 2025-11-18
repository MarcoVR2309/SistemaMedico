using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaMedico.datos;
using SistemaMedico.modelo;

namespace SistemaMedico.vista
{
    public partial class Login : System.Web.UI.Page
    {
        private UsuariosDAO usuariosDAO;
        private PacientesDAO pacientesDAO;
        private DoctoresDAO doctoresDAO;


        public Login()
        {
            usuariosDAO = new UsuariosDAO();
            pacientesDAO = new PacientesDAO();
            doctoresDAO = new DoctoresDAO(); 
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioId"] != null)
                {
                    RedirigirSegunRol();
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MostrarError("Por favor, ingrese su correo y contraseña.");
                return;
            }

            try
            {
                Usuarios usuario = null;
                bool credencialesValidas = usuariosDAO.ValidarCredenciales(email, password, out usuario);

                if (credencialesValidas && usuario != null)
                {

                    Session["UsuarioId"] = usuario.Id;
                    Session["UsuarioNombreCompleto"] = usuario.Nom + " " + usuario.Ape;
                    Session["UsuarioEmail"] = usuario.Email;
                    Session["IdRol"] = usuario.IdRol;
                    Session["UsuarioDNI"] = usuario.NumDoc;
                    Session["UsuarioTelefono"] = usuario.Tel;
                    Session["UsuarioFechaNac"] = usuario.FecNac;

                    string nombreRol = ObtenerNombreRol(usuario.IdRol);
                    Session["UsuarioRol"] = nombreRol;

                    if (nombreRol == "Paciente")
                    {
                        Pacientes paciente = pacientesDAO.ObtenerPorIdUsuario(usuario.Id);
                        if (paciente != null)
                        {
                            Session["PacienteId"] = paciente.Id;
                        }
                    }
                    else if (nombreRol == "Doctor")
                    {
                        // Llama al método que acabamos de crear
                        Doctores doctor = doctoresDAO.ObtenerDoctorPorIdUsuario(usuario.Id);
                        if (doctor != null)
                        {
                            // ¡ESTA ES LA LÍNEA CLAVE!
                            Session["DoctorId"] = doctor.Id; // Guardamos "D0000001", etc.
                        }
                        else
                        {
                            // Si es un Doctor pero no tiene registro en la tabla Doctores
                            MostrarError("Error: La cuenta de doctor no está configurada.");
                            Session.Clear(); // Limpia la sesión por seguridad
                            return; // Detiene el login
                        }
                    }
                    RedirigirSegunRol();
                }
                else
                {
                    MostrarError("Correo o contraseña incorrectos. Por favor, intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al iniciar sesión: " + ex.Message);
            }
        }

        private string ObtenerNombreRol(string idRol)
        {
            if (idRol.StartsWith("R"))
            {
                if (idRol == "R0000001") return "Administrador";
                if (idRol == "R0000002") return "Doctor";
                if (idRol == "R0000003") return "Paciente";
            }
            return "Paciente"; // fallback
        }

        private void RedirigirSegunRol()
        {
            string rol = Session["UsuarioRol"]?.ToString().Trim().ToLower();

            switch (rol)
            {
                case "paciente":
                    Response.Redirect("~/vista/PanelPaciente.aspx");
                    break;

                case "doctor":
                    Response.Redirect("~/vista/PanelMedico.aspx");
                    break;

                case "administrador":
                    Response.Redirect("~/vista/Admin/Dashboard.aspx");
                    break;

                default:
                    Session.Clear();
                    MostrarError("No se pudo determinar el tipo de usuario.");
                    break;
            }
        }

        private void MostrarError(string mensaje)
        {
            litError.Text = "<i class='fas fa-exclamation-circle'></i> " + mensaje;
            pnlError.Visible = true;
            pnlError.CssClass = "error-message show";
        }
    }
}