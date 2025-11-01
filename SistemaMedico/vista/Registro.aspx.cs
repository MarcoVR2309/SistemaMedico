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
    public partial class Registro : System.Web.UI.Page
    {
        private UsuariosDAO usuariosDAO;
        private PacientesDAO pacientesDAO;
        private RolesDAO rolesDAO;

        public Registro()
        {
            usuariosDAO = new UsuariosDAO();
            pacientesDAO = new PacientesDAO();
            rolesDAO = new RolesDAO();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioId"] != null)
                {
                    Response.Redirect("~/vista/Index.aspx");
                }
            }
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string tipoDocumento = ddlTipoDocumento.SelectedValue;
            string numeroDocumento = txtNumeroDocumento.Text.Trim();
            string genero = ddlGenero.SelectedValue;
            string email = txtEmail.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string sedePref = txtSedePref.Text.Trim();
            string fechaNacStr = txtFechaNacimiento.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmarPassword = txtConfirmarPassword.Text.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || 
                string.IsNullOrEmpty(numeroDocumento) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fechaNacStr))
            {
                MostrarError("Por favor, complete todos los campos obligatorios.");
                return;
            }

            if (password != confirmarPassword)
            {
                MostrarError("Las contraseñas no coinciden. Por favor, verifique.");
                return;
            }

            if (password.Length < 6)
            {
                MostrarError("La contraseña debe tener al menos 6 caracteres.");
                return;
            }

            try
            {
                Usuarios usuarioExistente = usuariosDAO.ObtenerPorEmail(email);
                if (usuarioExistente != null)
                {
                    MostrarError("Ya existe una cuenta con este correo electrónico.");
                    return;
                }

                DateTime fechaNac = DateTime.Parse(fechaNacStr);

                string idRolPaciente = rolesDAO.ObtenerIdRolPorNombre("Paciente");
                if (string.IsNullOrEmpty(idRolPaciente))
                {
                    MostrarError("Error: No se encontró el rol de Paciente en el sistema.");
                    return;
                }

                Usuarios nuevoUsuario = new Usuarios
                {
                    IdRol = idRolPaciente,
                    TipoDoc = tipoDocumento,
                    NumDoc = numeroDocumento,
                    Nom = nombre,
                    Ape = apellido,
                    Gen = genero[0],
                    Email = email,
                    Tel = string.IsNullOrEmpty(telefono) ? null : telefono,
                    FecNac = fechaNac,
                    SedePref = string.IsNullOrEmpty(sedePref) ? null : sedePref,
                    PswdHash = usuariosDAO.HashPassword(password),
                    Activo = true
                };

                string idUsuarioGenerado = usuariosDAO.Insertar(nuevoUsuario);

                if (string.IsNullOrEmpty(idUsuarioGenerado))
                {
                    MostrarError("Error al crear la cuenta. Por favor, intente nuevamente.");
                    return;
                }

                Pacientes nuevoPaciente = new Pacientes
                {
                    IdUsu = idUsuarioGenerado
                };

                string idPacienteGenerado = pacientesDAO.Insertar(nuevoPaciente);

                Session["UsuarioId"] = idUsuarioGenerado;
                Session["PacienteId"] = idPacienteGenerado;
                Session["UsuarioNombre"] = nombre + " " + apellido;
                Session["UsuarioEmail"] = email;
                Session["UsuarioRol"] = "Paciente";
                Session["EsNuevo"] = true;

                Response.Redirect("~/vista/Paciente/Dashboard.aspx");
            }
            catch (Exception ex)
            {
                MostrarError("Error al crear la cuenta: " + ex.Message);
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
