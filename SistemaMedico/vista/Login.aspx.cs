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

        public Login()
        {
            usuariosDAO = new UsuariosDAO();
            pacientesDAO = new PacientesDAO();
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
                MostrarError("Por favor, ingrese su correo y contrase�a.");
                return;
            }

            try
            {
                Usuarios usuario = null;
                bool credencialesValidas = usuariosDAO.ValidarCredenciales(email, password, out usuario);

                if (credencialesValidas && usuario != null)
                {
                    Session["UsuarioId"] = usuario.Id;
                    Session["UsuarioNombre"] = usuario.Nom + " " + usuario.Ape;
                    Session["UsuarioEmail"] = usuario.Email;
                    Session["IdRol"] = usuario.IdRol;

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

                    RedirigirSegunRol();
                }
                else
                {
                    MostrarError("Correo o contrase�a incorrectos. Por favor, intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al iniciar sesi�n: " + ex.Message);
            }
        }

        private string ObtenerNombreRol(string idRol)
        {
            if (idRol.StartsWith("R"))
            {
                if (idRol == "R0000001") return "Paciente";
                if (idRol == "R0000002") return "Doctor";
                if (idRol == "R0000003") return "Administrador";
            }
            return "Paciente";
        }

        private void RedirigirSegunRol()
        {
            string rol = Session["UsuarioRol"]?.ToString();

            switch (rol)
            {
                case "Paciente":
                    Response.Redirect("~/vista/Paciente/Dashboard.aspx");
                    break;
                    
                case "Doctor":
                    Response.Redirect("~/vista/Medico/Dashboard.aspx");
                    break;
                    
                case "Administrador":
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
