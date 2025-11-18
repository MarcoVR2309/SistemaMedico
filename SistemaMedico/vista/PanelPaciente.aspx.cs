using SistemaMedico.datos;
using SistemaMedico.modelo;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaMedico.vista
{
    public partial class PanelPaciente : System.Web.UI.Page
    {
        // DAOs
        private CitasDAO citasDAO = new CitasDAO();
        private EspecialidadesDAO especialidadesDAO = new EspecialidadesDAO();
        private DoctoresDAO doctoresDAO = new DoctoresDAO();

        // Propiedades de sesión
        private string CurrentUserId
        {
            get { return Session["UsuarioId"]?.ToString() ?? "P000010"; }
        }

        private string CurrentUserNameCompleto
        {
            get { return Session["UsuarioNombreCompleto"]?.ToString() ?? "Paciente"; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                lblUserName.Text = CurrentUserNameCompleto;
                lblProfileUserName.Text = CurrentUserNameCompleto;
                lblAvatarInitial.Text = CurrentUserNameCompleto.FirstOrDefault().ToString().ToUpper();

                CargarDropdownsModal();
                CargarCitasPaciente();
                CargarDatosPerfil();
            }
        }

        // ============================================================================
        // CARGAR DATOS
        // ============================================================================

        private void CargarDropdownsModal()
        {
            try
            {
                var especialidades = especialidadesDAO.ListarEspecialidades();
                ddlEspecialidades.DataSource = especialidades;
                ddlEspecialidades.DataTextField = "NomEsp";
                ddlEspecialidades.DataValueField = "Id";
                ddlEspecialidades.DataBind();
                ddlEspecialidades.Items.Insert(0, new ListItem("-- Seleccionar Especialidad --", ""));

                ddlDoctores.Items.Clear();
                ddlDoctores.Items.Insert(0, new ListItem("-- Seleccione Especialidad Primero --", ""));

                ddlHoras.Items.Clear();
                ddlHoras.Items.Insert(0, new ListItem("-- Seleccione Fecha --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensajeModal("Error al cargar datos: " + ex.Message, "error");
            }
        }

        private void CargarCitasPaciente()
        {
            try
            {
                var listaCitas = citasDAO.ListarCitasPorPaciente(CurrentUserId);

                if (listaCitas != null && listaCitas.Count > 0)
                {
                    repeaterCitas.DataSource = listaCitas;
                    repeaterCitas.DataBind();
                    lblNoCitas.Visible = false;
                }
                else
                {
                    repeaterCitas.DataSource = null;
                    repeaterCitas.DataBind();
                    lblNoCitas.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar citas: " + ex.Message);
                lblNoCitas.Visible = true;
                lblNoCitas.Text = "Error al cargar las citas: " + ex.Message;
            }
        }

        private void CargarDatosPerfil()
        {
            try
            {
                txtProfileNombre.Text = Session["UsuarioNombreCompleto"]?.ToString() ?? "No especificado";
                txtProfileEmail.Text = Session["UsuarioEmail"]?.ToString() ?? "No especificado";
                txtProfileTelefono.Text = Session["UsuarioTelefono"]?.ToString() ?? "No especificado";
                txtProfileUserId.Text = CurrentUserId;
                txtProfileDNI.Text = Session["UsuarioDNI"]?.ToString() ?? "No especificado";

                DateTime? fechaNac = (DateTime?)Session["UsuarioFechaNac"];
                txtProfileFechaNac.Text = fechaNac.HasValue ? fechaNac.Value.ToShortDateString() : "No especificado";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar perfil: " + ex.Message);
            }
        }

        // ============================================================================
        // EVENTOS DE DROPDOWNS (AutoPostBack)
        // ============================================================================

        protected void ddlEspecialidades_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string idEsp = ddlEspecialidades.SelectedValue;

                if (string.IsNullOrEmpty(idEsp))
                {
                    ddlDoctores.Items.Clear();
                    ddlDoctores.Items.Insert(0, new ListItem("-- Seleccione Especialidad Primero --", ""));
                    return;
                }

                var todosDoctores = doctoresDAO.ListarDoctores();
                var doctoresFiltrados = todosDoctores.Where(d => d.IdEsp == idEsp).ToList();

                ddlDoctores.DataSource = doctoresFiltrados.Select(d => new
                {
                    Id = d.Id,
                    NombreCompleto = $"Dr. {d.Nom} {d.Ape}"
                }).ToList();
                ddlDoctores.DataTextField = "NombreCompleto";
                ddlDoctores.DataValueField = "Id";
                ddlDoctores.DataBind();
                ddlDoctores.Items.Insert(0, new ListItem("-- Seleccionar Doctor --", ""));

                ddlHoras.Items.Clear();
                ddlHoras.Items.Insert(0, new ListItem("-- Seleccione Fecha --", ""));

                ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen",
                    "showModalFromCodeBehind();", true);
            }
            catch (Exception ex)
            {
                MostrarMensajeModal("Error al cargar doctores: " + ex.Message, "error");
            }
        }

        protected void ddlDoctores_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarHorasDisponibles();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen",
                "showModalFromCodeBehind();", true);
        }

        protected void txtFechaCita_TextChanged(object sender, EventArgs e)
        {
            CargarHorasDisponibles();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "KeepModalOpen",
                "showModalFromCodeBehind();", true);
        }

        private void CargarHorasDisponibles()
        {
            ddlHoras.Items.Clear();

            if (string.IsNullOrEmpty(ddlDoctores.SelectedValue) || string.IsNullOrEmpty(txtFechaCita.Text))
            {
                ddlHoras.Items.Insert(0, new ListItem("-- Seleccione Doctor y Fecha --", ""));
                return;
            }

            DateTime startTime = DateTime.Today.AddHours(6);
            DateTime endTime = DateTime.Today.AddHours(23);
            DateTime currentTime = startTime;

            while (currentTime <= endTime)
            {
                ddlHoras.Items.Add(new ListItem(currentTime.ToString("hh:mm tt"), currentTime.ToString("HH:mm")));
                currentTime = currentTime.AddMinutes(30);
            }

            ddlHoras.Items.Insert(0, new ListItem("-- Seleccionar Hora --", ""));
        }

        // ============================================================================
        // GUARDAR CITA
        // ============================================================================

        protected void btnReservar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validaciones
                if (string.IsNullOrEmpty(ddlEspecialidades.SelectedValue) ||
                    string.IsNullOrEmpty(ddlDoctores.SelectedValue) ||
                    string.IsNullOrEmpty(txtFechaCita.Text) ||
                    string.IsNullOrEmpty(ddlHoras.SelectedValue))
                {
                    MostrarMensajeModal("Por favor, complete todos los campos obligatorios.", "error");
                    return;
                }

                // 2. Parsear fecha
                DateTime fechaCita;
                string[] formatosFecha = { "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy" };

                if (!DateTime.TryParseExact(txtFechaCita.Text, formatosFecha,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out fechaCita))
                {
                    MostrarMensajeModal("Formato de fecha inválido. Use DD/MM/AAAA", "error");
                    return;
                }

                // 3. Validar fecha
                if (fechaCita.Date < DateTime.Now.Date)
                {
                    MostrarMensajeModal("No puede agendar citas en fechas pasadas.", "error");
                    return;
                }

                if (fechaCita.Date > DateTime.Now.Date.AddMonths(6))
                {
                    MostrarMensajeModal("No puede agendar citas con más de 6 meses de anticipación.", "error");
                    return;
                }

                // 4. Parsear hora
                TimeSpan horaCita;
                if (!TimeSpan.TryParse(ddlHoras.SelectedValue, out horaCita))
                {
                    MostrarMensajeModal("Hora inválida seleccionada.", "error");
                    return;
                }

                // 5. Obtener doctor
                var doctor = doctoresDAO.ObtenerDoctorPorId(ddlDoctores.SelectedValue);
                if (doctor == null)
                {
                    MostrarMensajeModal("Error: No se pudo encontrar al doctor.", "error");
                    return;
                }

                // 6. Crear objeto Citas (con datos de pago en NULL para pacientes)
                var nuevaCita = new Citas
                {
                    IdPac = CurrentUserId,
                    IdDoc = ddlDoctores.SelectedValue,
                    IdSede = "S000002",  // Ajusta según tu sede
                    IdEsp = doctor.IdEsp,
                    Fecha = fechaCita,
                    Hora = horaCita,
                    Motivo = txtMotivo.Text.Trim(),
                    TipoPago = null,     // ← NULL para pacientes
                    Monto = null,        // ← NULL para pacientes
                    PagoReali = false    // ← false por defecto
                };

                // 7. Guardar usando el procedimiento compartido
                string nuevoIdCita = citasDAO.RegistrarCita(nuevaCita);

                if (!string.IsNullOrEmpty(nuevoIdCita))
                {
                    MostrarMensajeModal($"¡Cita agendada exitosamente para el {fechaCita:dd/MM/yyyy} a las {horaCita:hh\\:mm}!", "success");

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "SuccessAndClose",
                        @"setTimeout(function() { hideModalFromCodeBehind(); }, 2000);", true);

                    CargarCitasPaciente();
                    LimpiarModal();
                }
                else
                {
                    MostrarMensajeModal("Error: No se pudo generar el ID de la cita.", "error");
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR SQL: {sqlEx.Message}");
                MostrarMensajeModal($"Error de base de datos: {sqlEx.Message}", "error");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.Message}");
                MostrarMensajeModal("Error al guardar la cita: " + ex.Message, "error");
            }
        }

        // ============================================================================
        // MÉTODOS AUXILIARES
        // ============================================================================

        private void LimpiarModal()
        {
            ddlEspecialidades.SelectedIndex = 0;
            ddlDoctores.Items.Clear();
            ddlDoctores.Items.Insert(0, new ListItem("-- Seleccione Especialidad Primero --", ""));
            ddlHoras.Items.Clear();
            ddlHoras.Items.Insert(0, new ListItem("-- Seleccione Fecha --", ""));
            txtFechaCita.Text = string.Empty;
            txtMotivo.Text = string.Empty;
            lblModalMensaje.Text = string.Empty;
            lblModalMensaje.CssClass = "modal-mensaje";
        }

        private void MostrarMensajeModal(string mensaje, string tipo)
        {
            lblModalMensaje.Text = mensaje;
            lblModalMensaje.CssClass = "modal-mensaje show " + tipo;

            string mensajeEscapado = mensaje.Replace("'", "\\'").Replace("\r", "\\r").Replace("\n", "\\n");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModalOnError",
                $"showModalFromCodeBehind('{mensajeEscapado}', '{tipo}');", true);
        }

        protected string ObtenerClaseEstado(string estado)
        {
            if (string.IsNullOrEmpty(estado)) return "status-pendiente";

            switch (estado.ToUpper().Trim())
            {
                case "P":
                case "PENDIENTE":
                    return "status-pendiente";
                case "C":
                case "CONFIRMADA":
                    return "status-confirmada";
                case "E":
                case "EN PROCESO":
                    return "status-en-proceso";
                case "F":
                case "COMPLETADA":
                case "FINALIZADA":
                    return "status-completada";
                case "X":
                case "CANCELADA":
                    return "status-cancelada";
                default:
                    return "status-pendiente";
            }
        }

        protected string ObtenerIconoEstado(string estado)
        {
            if (string.IsNullOrEmpty(estado)) return "fas fa-clock";

            switch (estado.ToUpper().Trim())
            {
                case "P":
                case "PENDIENTE":
                    return "fas fa-clock";
                case "C":
                case "CONFIRMADA":
                    return "fas fa-check-circle";
                case "E":
                case "EN PROCESO":
                    return "fas fa-spinner";
                case "F":
                case "COMPLETADA":
                case "FINALIZADA":
                    return "fas fa-check-double";
                case "X":
                case "CANCELADA":
                    return "fas fa-times-circle";
                default:
                    return "fas fa-clock";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}