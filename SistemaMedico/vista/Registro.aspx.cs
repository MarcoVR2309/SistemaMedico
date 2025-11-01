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
        private SedesDAO sedesDAO;

        public Registro()
        {
            usuariosDAO = new UsuariosDAO();
            pacientesDAO = new PacientesDAO();
            rolesDAO = new RolesDAO();
            sedesDAO = new SedesDAO();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioId"] != null)
                {
                    Response.Redirect("~/vista/Index.aspx");
                }
                CargarSedes();
            }
        }

        private void CargarSedes()
        {
            try
            {
                List<Sedes> listaSedes = sedesDAO.ListarSedes();
                DDLSedePref.Items.Clear();
                DDLSedePref.Items.Add(new ListItem("-- Seleccionar sede (opcional) --", ""));

                foreach (Sedes sede in listaSedes)
                {
                    DDLSedePref.Items.Add(new ListItem(sede.NomSede, sede.Id));
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar las sedes: " + ex.Message);
            }
        }

        private bool ValidarSede(string idSede)
        {
            if (string.IsNullOrEmpty(idSede))
            {
                return true;
            }
            try
            {
                Sedes sede = sedesDAO.ObtenerPorId(idSede);
                return sede != null;
            }
            catch (Exception ex)
            {
                MostrarError("Error al validar la sede: " + ex.Message);
                return false;
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
            string sedePref = DDLSedePref.SelectedValue;
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

            if (!ValidarSede(sedePref))
            {
                MostrarError("La sede preferida seleccionada no es válida.");
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

                Response.Redirect("~/vista/Login.aspx");
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
