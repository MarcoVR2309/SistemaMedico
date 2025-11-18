using SistemaMedico.datos;
using SistemaMedico.modelo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaMedico.vista
{
    public partial class PanelMedico : System.Web.UI.Page
    {
        private string CurrentUserId
        {
            get { return Session["UsuarioId"]?.ToString() ?? "U000001"; }
        }

        // Instancia de DAO
        private CitasDAO citasDAO = new CitasDAO();
        private PacientesDAO pacientesDAO = new PacientesDAO();
        private SedesDAO sedesDAO = new SedesDAO();
        private DoctoresDAO doctoresDAO = new DoctoresDAO();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                CargarDatosDoctor();
                CargarCitasDelDia();
                CargarDropdownsModal();
            }
        }

        private void CargarDatosDoctor()
        {
            try
            {
                Doctores doctor = doctoresDAO.ObtenerDoctorPorIdUsuario(CurrentUserId);

                if (doctor != null)
                {
                    lblDoctorName.Text = "Dr. " + doctor.Nom + " " + doctor.Ape;

                    lblPerfilNombreCompleto.Text = doctor.Nom + " " + doctor.Ape;
                    lblPerfilEmail.Text = doctor.Email;
                    lblPerfilTelefono.Text = string.IsNullOrEmpty(doctor.Tel) ? "No registrado" : doctor.Tel;

                    lblPerfilEspecialidad.Text = doctor.NombreEspecialidad;
                    lblPerfilCMP.Text = doctor.CodMed;
                    lblPerfilExperiencia.Text = (doctor.Expe.HasValue ? doctor.Expe.Value.ToString() : "0") + " años";
                    lblPerfilID.Text = doctor.Id;
                }
                else
                {
                    lblDoctorName.Text = "Doctor no encontrado";
                    lblPerfilNombreCompleto.Text = "Error de Carga";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar datos del doctor: " + ex.Message);
                lblDoctorName.Text = "Error al cargar";
            }
        }

        private void CargarCitasDelDia()
        {
            Doctores doctor = doctoresDAO.ObtenerDoctorPorIdUsuario(CurrentUserId);
            DateTime fechaHoy = DateTime.Now.Date;

            var listaCitas = citasDAO.ListarCitasDelDia(doctor.Id, fechaHoy);

            if (listaCitas.Count > 0)
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

        private void CargarDropdownsModal()
        {
            try
            {
                ddlPacienteModal.DataSource = pacientesDAO.ListarPacientesParaDropdown();
                ddlPacienteModal.DataTextField = "NombreCompleto";
                ddlPacienteModal.DataValueField = "ID";
                ddlPacienteModal.DataBind();
                ddlPacienteModal.Items.Insert(0, new ListItem("-- Seleccionar Paciente --", ""));

                ddlSedeModal.DataSource = sedesDAO.ListarSedes();
                ddlSedeModal.DataTextField = "NomSede";
                ddlSedeModal.DataValueField = "ID";
                ddlSedeModal.DataBind();
                ddlSedeModal.Items.Insert(0, new ListItem("-- Seleccionar Sede --", ""));

                if (ddlHoraModal.Items.Count <= 1)
                {
                    ddlHoraModal.Items.Clear();
                    ddlHoraModal.Items.Insert(0, new ListItem("-- Seleccionar Hora --", ""));

                    DateTime startTime = DateTime.Today.AddHours(6);
                    DateTime endTime = DateTime.Today.AddHours(23);

                    DateTime currentTime = startTime;
                    while (currentTime <= endTime)
                    {
                        string displayText = currentTime.ToString("hh:mm tt");
                        string valueText = currentTime.ToString("HH:mm");

                        ddlHoraModal.Items.Add(new ListItem(displayText, valueText));
                        currentTime = currentTime.AddMinutes(30);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeModal("Error al cargar datos: " + ex.Message, "error");
            }
        }

        protected void repeaterCitas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string idCita = e.CommandArgument.ToString();

            switch (e.CommandName)
            {
                case "Iniciar":
                    IniciarConsulta(idCita);
                    break;

                case "AbrirModalFinalizar":
                    AbrirModalFinalizar(idCita);
                    break;

                case "VerFicha":
                    VerFicha(idCita);
                    break;
            }
        }

        private void AbrirModalFinalizar(string idCita)
        {
            try
            {
                string idConsulta = citasDAO.ObtenerConsultaIdPorCitaId(idCita);

                if (string.IsNullOrEmpty(idConsulta))
                {
                    idConsulta = citasDAO.IniciarConsulta(idCita);

                    if (string.IsNullOrEmpty(idConsulta))
                        throw new Exception("No se pudo iniciar la consulta.");

                    CargarCitasDelDia();
                }

                hfConsultaIdCita.Value = idCita;
                hfConsultaIdConsulta.Value = idConsulta;

                txtSintomas.Text = "";
                txtDiagnostico.Text = "";
                txtTratamiento.Text = "";
                txtObservaciones.Text = "";
                lblModalConsultaMensaje.Text = "";
                lblModalConsultaMensaje.CssClass = "modal-mensaje";

                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowFinalizarModal", "showFinalizarModal();", true);
            }
            catch (Exception ex)
            {
                MostrarMensajeModal("Error: " + ex.Message, "error");
            }
        }

        protected void btnGuardarConsulta_Click(object sender, EventArgs e)
        {
            try
            {
                string idCita = hfConsultaIdCita.Value;
                string idConsulta = hfConsultaIdConsulta.Value;

                if (string.IsNullOrEmpty(idCita) || string.IsNullOrEmpty(idConsulta))
                {
                    MostrarMensajeConsultaModal("Error de sesión.", "error");
                    return;
                }

                ConsultasMedicas consulta = new ConsultasMedicas
                {
                    Id = idConsulta,
                    IdCita = idCita,
                    Sintomas = txtSintomas.Text,
                    Diagnostico = txtDiagnostico.Text,
                    Tratamiento = txtTratamiento.Text,
                    Observaciones = txtObservaciones.Text
                };

                citasDAO.FinalizarConsulta(consulta);

                CargarCitasDelDia();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "HideFinalizarModal", "hideFinalizarModal();", true);
            }
            catch (Exception ex)
            {
                MostrarMensajeConsultaModal("Error al guardar: " + ex.Message, "error");
            }
        }

        private void MostrarMensajeConsultaModal(string mensaje, string tipo)
        {
            lblModalConsultaMensaje.Text = mensaje;
            lblModalConsultaMensaje.CssClass = "modal-mensaje show " + tipo;

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowFinalizarModalOnError",
                $"showFinalizarModal();", true);
        }

        protected void btnGuardarCita_Click(object sender, EventArgs e)
        {
            Doctores doctor = doctoresDAO.ObtenerDoctorPorIdUsuario(CurrentUserId);

            try
            {
                if (string.IsNullOrEmpty(ddlPacienteModal.SelectedValue) ||
                    string.IsNullOrEmpty(ddlSedeModal.SelectedValue) ||
                    string.IsNullOrEmpty(txtFechaModal.Text) ||
                    string.IsNullOrEmpty(ddlHoraModal.SelectedValue))
                {
                    MostrarMensajeModal("Complete todos los campos.", "error");
                    return;
                }

                if (doctor == null)
                {
                    MostrarMensajeModal("No se encontró el doctor.", "error");
                    return;
                }

                Citas nuevaCita = new Citas
                {
                    IdPac = ddlPacienteModal.SelectedValue,
                    IdDoc = doctor.Id,
                    IdSede = ddlSedeModal.SelectedValue,
                    IdEsp = doctor.IdEsp,
                    Fecha = Convert.ToDateTime(txtFechaModal.Text),
                    Hora = TimeSpan.Parse(ddlHoraModal.SelectedValue),
                    Motivo = txtMotivoModal.Text,
                    TipoPago = "Presencial",
                    Monto = 0,
                    PagoReali = false
                };

                string nuevoId = citasDAO.RegistrarCita(nuevaCita);

                CargarCitasDelDia();
                LimpiarModal();

                ScriptManager.RegisterStartupScript(this, this.GetType(), "HideModal", "hideModalFromCodeBehind();", true);
            }
            catch (SqlException sqEx)
            {
                MostrarMensajeModal("Error de BD: " + sqEx.Message, "error");
            }
            catch (Exception ex)
            {
                MostrarMensajeModal("Error al guardar: " + ex.Message, "error");
            }
        }

        private void MostrarMensajeModal(string mensaje, string tipo)
        {
            lblModalMensaje.Text = mensaje;
            lblModalMensaje.CssClass = "modal-mensaje show " + tipo;

            string mensajeEscapado = mensaje
                .Replace("'", "\\'")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModalOnError",
                $"showModalFromCodeBehind('{mensajeEscapado}', '{tipo}');", true);
        }

        private void LimpiarModal()
        {
            ddlPacienteModal.SelectedIndex = 0;
            ddlSedeModal.SelectedIndex = 0;
            ddlHoraModal.SelectedIndex = 0;
            txtFechaModal.Text = "";
            txtMotivoModal.Text = "";
            lblModalMensaje.Text = "";
            lblModalMensaje.CssClass = "modal-mensaje";
        }

        private void IniciarConsulta(string idCita)
        {
            try
            {
                string nuevoIdConsulta = citasDAO.IniciarConsulta(idCita);

                CargarCitasDelDia();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR al iniciar consulta: {ex.Message}");
            }
        }

        private void VerFicha(string idCita)
        {
            try
            {
                DetalleCitaDTO ficha = citasDAO.ObtenerFichaCita(idCita);

                if (ficha != null)
                {
                    lblFichaId.Text = ficha.IdCita;

                    lblFichaPaciente.Text = ficha.PacienteNombre;
                    lblFichaDNI.Text = ficha.PacienteDNI;
                    lblFichaTelefono.Text = ficha.PacienteTelefono;
                    lblFichaPeso.Text = ficha.PacientePeso.HasValue ? ficha.PacientePeso.Value.ToString("0.0") : "N/A";

                    lblFichaFecha.Text = ficha.Fecha.ToString("dd/MM/yyyy") + " - " + DateTime.Today.Add(ficha.Hora).ToString("hh:mm tt");
                    lblFichaSede.Text = ficha.Sede;
                    lblFichaEspecialidad.Text = ficha.Especialidad;
                    lblFichaEstado.Text = ficha.Estado == "P" ? "Pendiente" : (ficha.Estado == "F" ? "Finalizada" : "En Progreso");
                    lblFichaEstado.ForeColor = ficha.Estado == "F" ? System.Drawing.Color.Green : System.Drawing.Color.Orange;

                    lblFichaMotivo.Text = ficha.Motivo;
                    lblFichaDiagnostico.Text = ficha.Diagnostico;
                    lblFichaTratamiento.Text = ficha.Tratamiento;
                    lblFichaObservaciones.Text = ficha.Observaciones;

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowFichaModal", "showFichaModal();", true);
                }
                else
                {
                    MostrarMensajeModal("No se encontraron detalles.", "error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeModal("Error al cargar la ficha: " + ex.Message, "error");
            }
        }

        protected void btnNuevaCita_Click(object sender, EventArgs e)
        {
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }
    }
}
