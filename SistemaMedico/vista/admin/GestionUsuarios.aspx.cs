using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaMedico.datos;
using SistemaMedico.modelo;

namespace SistemaMedico.vista.admin
{
    public partial class GestionUsuarios : Page
    {
        private readonly AdminUsuariosDAO adminUsuariosDAO = new AdminUsuariosDAO();
        private readonly RolesDAO rolesDAO = new RolesDAO();
        private readonly EspecialidadesDAO especialidadesDAO = new EspecialidadesDAO();

        protected void Page_Load(object sender, EventArgs e)
        {
            // TEMPORAL: Comentado para permitir acceso directo a la vista (solo para pruebas visuales)
            // TODO: Descomentar antes de producción
            /*
            if (!EsAdministrador())
            {
                Response.Redirect("~/vista/Login.aspx");
                return;
            }
            */

            if (!IsPostBack)
            {
                CargarRoles();
                CargarEspecialidadesDropDowns();
                CargarUsuarios();
            }
            else
            {
                // Reabrir modal si se solicitó desde el postback anterior
                if (!string.IsNullOrEmpty(hdnMostrarModal.Value))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModalPostback", $"openModal('{hdnMostrarModal.Value}');", true);
                    hdnMostrarModal.Value = string.Empty;
                }
            }
        }

        private bool EsAdministrador()
        {
            string rol = Session["UsuarioRol"] as string;
            return string.Equals(rol, "Administrador", StringComparison.OrdinalIgnoreCase);
        }

        private void CargarRoles()
        {
            List<Rol> roles = rolesDAO.ListarRoles();

            ddlRolFiltro.Items.Clear();
            ddlRolFiltro.Items.Add(new ListItem("Todos", string.Empty));

            ddlRolCrear.Items.Clear();
            ddlRolCrear.Items.Add(new ListItem("Seleccionar rol", string.Empty));

            ddlRolEditar.Items.Clear();
            ddlRolEditar.Items.Add(new ListItem("Seleccionar rol", string.Empty));

            foreach (Rol rol in roles)
            {
                ddlRolFiltro.Items.Add(new ListItem(rol.NomRol, rol.Id));
                ddlRolCrear.Items.Add(new ListItem(rol.NomRol, rol.Id));
                ddlRolEditar.Items.Add(new ListItem(rol.NomRol, rol.Id));
            }
        }

        private void CargarEspecialidadesDropDowns()
        {
            List<Especialidades> especialidades = new List<Especialidades>();
            try
            {
                especialidades = especialidadesDAO.ListarEspecialidades();
            }
            catch
            {
                // Si no se cargan especialidades, se mantiene la lista vacía
            }

            LlenarDropEspecialidades(ddlEspecialidadCrear, especialidades, true);
            LlenarDropEspecialidades(ddlEspecialidadEditar, especialidades, true);
        }

        private void LlenarDropEspecialidades(DropDownList ddl, List<Especialidades> especialidades, bool incluirOpcionDefault)
        {
            ddl.Items.Clear();
            if (incluirOpcionDefault)
            {
                ddl.Items.Add(new ListItem("Seleccionar especialidad", string.Empty));
            }

            foreach (Especialidades esp in especialidades)
            {
                ddl.Items.Add(new ListItem(esp.NomEsp, esp.Id));
            }
        }

        private void CargarUsuarios()
        {
            try
            {
                AdminUsuariosLista data = adminUsuariosDAO.ListarUsuarios(
                    txtBusqueda.Text.Trim(),
                    ddlRolFiltro.SelectedValue,
                    ddlEstadoFiltro.SelectedValue);

                lblTotalUsuarios.Text = data.Resumen.TotalUsuarios.ToString();
                lblTotalDoctores.Text = data.Resumen.TotalDoctores.ToString();
                lblTotalPacientes.Text = data.Resumen.TotalPacientes.ToString();
                lblTotalActivos.Text = data.Resumen.TotalActivos.ToString();

                if (data.Usuarios.Any())
                {
                    gvUsuarios.DataSource = data.Usuarios;
                    gvUsuarios.DataBind();
                    pnlEmptyState.Visible = false;
                }
                else
                {
                    gvUsuarios.DataSource = null;
                    gvUsuarios.DataBind();
                    pnlEmptyState.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MostrarNotificacion("Error al cargar los usuarios: " + ex.Message, false);
            }
        }

        protected string ObtenerClaseRol(string nombreRol)
        {
            if (string.IsNullOrWhiteSpace(nombreRol))
            {
                return "badge";
            }

            switch (nombreRol.ToLowerInvariant())
            {
                case "doctor":
                    return "badge rol-doctor";
                case "paciente":
                    return "badge rol-paciente";
                case "administrador":
                    return "badge rol-administrador";
                default:
                    return "badge";
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBusqueda.Text = string.Empty;
            ddlRolFiltro.SelectedIndex = 0;
            ddlEstadoFiltro.SelectedIndex = 0;
            CargarUsuarios();
        }

        protected void btnMostrarCrear_Click(object sender, EventArgs e)
        {
            LimpiarFormularioCrear();
            MostrarModal("modalCrearUsuario");
        }

        protected void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            List<string> errores = ValidarFormularioCrear();
            if (errores.Any())
            {
                lblErrorCrear.Text = string.Join("<br/>", errores);
                lblErrorCrear.Visible = true;
                MostrarModal("modalCrearUsuario");
                return;
            }

            try
            {
                string idRol = ddlRolCrear.SelectedValue;
                string rolNombre = ddlRolCrear.SelectedItem.Text;
                DateTime? fechaNac = ParseDate(txtFechaNacimientoCrear.Text);
                decimal? pesoPaciente = ParseDecimal(txtPesoPacienteCrear.Text);
                DateTime? fechaReferenciaPaciente = ParseDate(txtFechaReferenciaPacienteCrear.Text);
                int? experiencia = ParseInt(txtExperienciaCrear.Text);
                string passwordTemporal = string.IsNullOrWhiteSpace(txtPasswordCrear.Text) ? GenerarPasswordTemporal() : txtPasswordCrear.Text;

                var resultado = adminUsuariosDAO.CrearUsuario(
                    idRol,
                    "DNI",
                    txtDocumentoCrear.Text.Trim(),
                    txtApellidoCrear.Text.Trim(),
                    txtNombreCrear.Text.Trim(),
                    ObtenerGeneroSeleccionado(ddlGeneroCrear),
                    fechaNac,
                    txtEmailCrear.Text.Trim(),
                    txtTelefonoCrear.Text.Trim(),
                    passwordTemporal,
                    null,
                    rolNombre.Equals("Doctor", StringComparison.OrdinalIgnoreCase) ? ddlEspecialidadCrear.SelectedValue : null,
                    rolNombre.Equals("Doctor", StringComparison.OrdinalIgnoreCase) ? txtColegiaturaCrear.Text.Trim() : null,
                    rolNombre.Equals("Doctor", StringComparison.OrdinalIgnoreCase) ? experiencia : null,
                    rolNombre.Equals("Paciente", StringComparison.OrdinalIgnoreCase) ? pesoPaciente : null,
                    rolNombre.Equals("Paciente", StringComparison.OrdinalIgnoreCase) ? fechaReferenciaPaciente : null
                );

                string mensaje = "Usuario creado correctamente. ID generado: " + resultado.IdUsuario;
                if (string.IsNullOrWhiteSpace(txtPasswordCrear.Text))
                {
                    mensaje += ". Contraseña temporal: " + passwordTemporal;
                }

                MostrarNotificacion(mensaje, true);
                CerrarModal("modalCrearUsuario");
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                lblErrorCrear.Text = "Error al crear usuario: " + ex.Message;
                lblErrorCrear.Visible = true;
                MostrarModal("modalCrearUsuario");
            }
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string[] argumentos;
            switch (e.CommandName)
            {
                case "Editar":
                    string idEditar = e.CommandArgument.ToString();
                    CargarDatosEdicion(idEditar);
                    break;
                case "ToggleEstado":
                    argumentos = e.CommandArgument.ToString().Split('|');
                    if (argumentos.Length == 2)
                    {
                        string id = argumentos[0];
                        bool estadoActual = bool.Parse(argumentos[1]);
                        hdnUsuarioSeleccionado.Value = id;
                        bool activar = !estadoActual;
                        hdnAccionModal.Value = activar ? "toggle|activar" : "toggle|desactivar";
                        lblMensajeConfirmacion.Text = activar
                            ? "¿Deseas activar la cuenta seleccionada?"
                            : "¿Deseas desactivar la cuenta seleccionada? Podrás reactivarla más adelante.";
                        MostrarModal("modalConfirmacion");
                    }
                    break;
                case "ResetPassword":
                    string idReset = e.CommandArgument.ToString();
                    string passwordTemporal = GenerarPasswordTemporal();
                    hdnUsuarioSeleccionado.Value = idReset;
                    hdnAccionModal.Value = "reset";
                    ViewState["PasswordTemporal"] = passwordTemporal;
                    lblMensajeConfirmacion.Text = $"Se generará una contraseña temporal: <strong>{passwordTemporal}</strong>. ¿Deseas continuar?";
                    MostrarModal("modalConfirmacion");
                    break;
            }
        }

        protected void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hdnUsuarioSeleccionado.Value))
            {
                MostrarNotificacion("No se encontró el usuario a editar.", false);
                return;
            }

            List<string> errores = ValidarFormularioEditar();
            if (errores.Any())
            {
                lblErrorEditar.Text = string.Join("<br/>", errores);
                lblErrorEditar.Visible = true;
                MostrarModal("modalEditarUsuario");
                return;
            }

            try
            {
                string idUsuario = hdnUsuarioSeleccionado.Value;
                string idRol = ddlRolEditar.SelectedValue;
                string rolNombre = ddlRolEditar.SelectedItem.Text;
                DateTime? fechaNac = ParseDate(txtFechaNacimientoEditar.Text);
                decimal? pesoPaciente = ParseDecimal(txtPesoPacienteEditar.Text);
                DateTime? fechaReferencia = ParseDate(txtFechaReferenciaPacienteEditar.Text);
                int? experiencia = ParseInt(txtExperienciaEditar.Text);

                adminUsuariosDAO.ActualizarUsuario(
                    idUsuario,
                    idRol,
                    "DNI",
                    txtDocumentoEditar.Text.Trim(),
                    txtApellidoEditar.Text.Trim(),
                    txtNombreEditar.Text.Trim(),
                    ObtenerGeneroSeleccionado(ddlGeneroEditar),
                    fechaNac,
                    txtEmailEditar.Text.Trim(),
                    txtTelefonoEditar.Text.Trim(),
                    null,
                    rolNombre.Equals("Doctor", StringComparison.OrdinalIgnoreCase) ? ddlEspecialidadEditar.SelectedValue : null,
                    rolNombre.Equals("Doctor", StringComparison.OrdinalIgnoreCase) ? txtColegiaturaEditar.Text.Trim() : null,
                    rolNombre.Equals("Doctor", StringComparison.OrdinalIgnoreCase) ? experiencia : null,
                    rolNombre.Equals("Paciente", StringComparison.OrdinalIgnoreCase) ? pesoPaciente : null,
                    rolNombre.Equals("Paciente", StringComparison.OrdinalIgnoreCase) ? fechaReferencia : null
                );

                if (chkActivoEditar.Checked)
                {
                    adminUsuariosDAO.CambiarEstado(idUsuario, true);
                }
                else
                {
                    adminUsuariosDAO.CambiarEstado(idUsuario, false);
                }

                MostrarNotificacion("Cambios guardados correctamente.", true);
                CerrarModal("modalEditarUsuario");
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                lblErrorEditar.Text = "Error al guardar cambios: " + ex.Message;
                lblErrorEditar.Visible = true;
                MostrarModal("modalEditarUsuario");
            }
        }

        protected void btnConfirmarAccion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hdnUsuarioSeleccionado.Value))
            {
                MostrarNotificacion("No se pudo identificar el usuario seleccionado.", false);
                return;
            }

            try
            {
                if (hdnAccionModal.Value.StartsWith("toggle"))
                {
                    bool activar = hdnAccionModal.Value.Contains("activar");
                    adminUsuariosDAO.CambiarEstado(hdnUsuarioSeleccionado.Value, activar);
                    MostrarNotificacion(activar ? "Usuario activado correctamente." : "Usuario desactivado correctamente.", true);
                }
                else if (hdnAccionModal.Value == "reset")
                {
                    string passwordTemporal = ViewState["PasswordTemporal"] as string ?? GenerarPasswordTemporal();
                    adminUsuariosDAO.ResetearPassword(hdnUsuarioSeleccionado.Value, passwordTemporal);
                    MostrarNotificacion("Contraseña reseteada. Nueva contraseña temporal: " + passwordTemporal, true);
                }

                ViewState["PasswordTemporal"] = null;
                hdnUsuarioSeleccionado.Value = string.Empty;
                hdnAccionModal.Value = string.Empty;
                CargarUsuarios();
                CerrarModal("modalConfirmacion");
            }
            catch (Exception ex)
            {
                MostrarNotificacion("Error al ejecutar la acción: " + ex.Message, false);
                MostrarModal("modalConfirmacion");
            }
        }

        private void CargarDatosEdicion(string idUsuario)
        {
            AdminUsuarioDetalle detalle = adminUsuariosDAO.ObtenerDetalle(idUsuario);
            if (detalle == null)
            {
                MostrarNotificacion("No se encontró información del usuario seleccionado.", false);
                return;
            }

            hdnUsuarioSeleccionado.Value = detalle.Id;

            txtNombreEditar.Text = detalle.Nombre;
            txtApellidoEditar.Text = detalle.Apellido;
            txtEmailEditar.Text = detalle.Email;
            txtTelefonoEditar.Text = detalle.Telefono;
            txtDocumentoEditar.Text = detalle.NumDoc;
            string genero = detalle.Genero == '\0' || detalle.Genero == ' ' ? string.Empty : detalle.Genero.ToString();
            SeleccionarValorDropDown(ddlGeneroEditar, genero);
            txtFechaNacimientoEditar.Text = detalle.FechaNacimiento.HasValue ? detalle.FechaNacimiento.Value.ToString("yyyy-MM-dd") : string.Empty;

            SeleccionarValorDropDown(ddlRolEditar, detalle.IdRol);
            SeleccionarValorDropDown(ddlEspecialidadEditar, detalle.IdEspecialidad);
            txtColegiaturaEditar.Text = detalle.NumeroColegiatura;
            txtExperienciaEditar.Text = detalle.AniosExperiencia.HasValue ? detalle.AniosExperiencia.Value.ToString() : string.Empty;
            txtPesoPacienteEditar.Text = detalle.PesoPaciente.HasValue ? detalle.PesoPaciente.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
            txtFechaReferenciaPacienteEditar.Text = detalle.EdadPaciente.HasValue ? detalle.EdadPaciente.Value.ToString("yyyy-MM-dd") : string.Empty;
            chkActivoEditar.Checked = detalle.Activo;

            lblErrorEditar.Visible = false;
            MostrarModal("modalEditarUsuario");
        }

        private void LimpiarFormularioCrear()
        {
            txtNombreCrear.Text = string.Empty;
            txtApellidoCrear.Text = string.Empty;
            txtEmailCrear.Text = string.Empty;
            txtTelefonoCrear.Text = string.Empty;
            txtDocumentoCrear.Text = string.Empty;
            txtPasswordCrear.Text = string.Empty;
            ddlGeneroCrear.SelectedIndex = 0;
            txtFechaNacimientoCrear.Text = string.Empty;
            ddlRolCrear.SelectedIndex = 0;
            ddlEspecialidadCrear.SelectedIndex = 0;
            txtColegiaturaCrear.Text = string.Empty;
            txtExperienciaCrear.Text = string.Empty;
            txtPesoPacienteCrear.Text = string.Empty;
            txtFechaReferenciaPacienteCrear.Text = string.Empty;
            lblErrorCrear.Visible = false;
        }

        private List<string> ValidarFormularioCrear()
        {
            List<string> errores = new List<string>();

            if (string.IsNullOrWhiteSpace(txtNombreCrear.Text))
                errores.Add("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtApellidoCrear.Text))
                errores.Add("El apellido es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtEmailCrear.Text))
                errores.Add("El correo electrónico es obligatorio.");

            if (string.IsNullOrWhiteSpace(ddlRolCrear.SelectedValue))
                errores.Add("Seleccione un rol para el usuario.");

            if (ddlRolCrear.SelectedItem.Text.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(ddlEspecialidadCrear.SelectedValue))
                    errores.Add("Seleccione una especialidad para el doctor.");
                if (string.IsNullOrWhiteSpace(txtColegiaturaCrear.Text))
                    errores.Add("Ingrese el número de colegiatura del doctor.");
            }

            return errores;
        }

        private List<string> ValidarFormularioEditar()
        {
            List<string> errores = new List<string>();

            if (string.IsNullOrWhiteSpace(txtNombreEditar.Text))
                errores.Add("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtApellidoEditar.Text))
                errores.Add("El apellido es obligatorio.");

            if (string.IsNullOrWhiteSpace(txtEmailEditar.Text))
                errores.Add("El correo electrónico es obligatorio.");

            if (string.IsNullOrWhiteSpace(ddlRolEditar.SelectedValue))
                errores.Add("Seleccione un rol para el usuario.");

            if (ddlRolEditar.SelectedItem.Text.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(ddlEspecialidadEditar.SelectedValue))
                    errores.Add("Seleccione una especialidad para el doctor.");
                if (string.IsNullOrWhiteSpace(txtColegiaturaEditar.Text))
                    errores.Add("Ingrese el número de colegiatura del doctor.");
            }

            return errores;
        }

        private void MostrarNotificacion(string mensaje, bool exito)
        {
            pnlNotificacion.Visible = true;
            string icono = exito ? "<i class='fas fa-check-circle' style='color:#04aa6d;margin-right:8px;'></i>" : "<i class='fas fa-exclamation-triangle' style='color:#d93b3b;margin-right:8px;'></i>";
            litNotificacion.Text = icono + mensaje;
        }

        private char ObtenerGeneroSeleccionado(DropDownList ddlGenero)
        {
            return string.IsNullOrWhiteSpace(ddlGenero.SelectedValue) ? ' ' : ddlGenero.SelectedValue[0];
        }

        private DateTime? ParseDate(string value)
        {
            if (DateTime.TryParse(value, out DateTime resultado))
            {
                return resultado;
            }
            return null;
        }

        private decimal? ParseDecimal(string value)
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado))
            {
                return resultado;
            }
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out resultado))
            {
                return resultado;
            }
            return null;
        }

        private int? ParseInt(string value)
        {
            if (int.TryParse(value, out int resultado))
            {
                return resultado;
            }
            return null;
        }

        private void SeleccionarValorDropDown(DropDownList ddl, string valor)
        {
            if (valor == null)
            {
                ddl.SelectedIndex = 0;
                return;
            }

            ListItem item = ddl.Items.FindByValue(valor);
            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
            else
            {
                ddl.SelectedIndex = 0;
            }
        }

        private void MostrarModal(string modalId)
        {
            hdnMostrarModal.Value = modalId;
            ScriptManager.RegisterStartupScript(this, GetType(), modalId + "_open", $"openModal('{modalId}');", true);
        }

        private void CerrarModal(string modalId)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), modalId + "_close", $"closeModal('{modalId}');", true);
        }

        private string GenerarPasswordTemporal()
        {
            const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            Random random = new Random();
            char[] buffer = new char[10];

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = caracteres[random.Next(caracteres.Length)];
            }

            return new string(buffer);
        }
    }
}

