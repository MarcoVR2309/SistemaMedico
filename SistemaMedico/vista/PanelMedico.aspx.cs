using SistemaMedico.datos; // <-- Importante: Añade el DAO
using SistemaMedico.modelo; // <-- (Opcional, pero bueno tenerlo)
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SistemaMedico.vista
{

    public partial class PanelMedico : System.Web.UI.Page
    {
        //Session["DoctorId"] = doctor.Id;
        private string CurrentUserId
        {
            get { return Session["UsuarioId"]?.ToString() ?? "U000001"; }
        }



        // Instancia del DAO que usaremos en esta página
        private CitasDAO citasDAO = new CitasDAO();
        private PacientesDAO pacientesDAO = new PacientesDAO();
        private SedesDAO sedesDAO = new SedesDAO();
        private DoctoresDAO doctoresDAO = new DoctoresDAO(); // Para obtener la especialidad


        protected void Page_Load(object sender, EventArgs e)
        {
            //En caso de que no exista usuario logueado, redirigimos a Login
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                // Solo cargamos las citas la primera vez que entra a la página
                CargarDatosDoctor();
                CargarCitasDelDia();
                CargarDropdownsModal();
            }
        }
        private void CargarDatosDoctor()
        {
            try
            {
                // 1. Llama al DAO
                Doctores doctor = doctoresDAO.ObtenerDoctorPorIdUsuario(CurrentUserId);
                

                if (doctor != null)
                {
                    // --- A) Actualiza la etiqueta de bienvenida (Sidebar) ---
                    lblDoctorName.Text = "Dr. " + doctor.Nom + " " + doctor.Ape;

                    // --- B) Actualiza el Panel "Mi Perfil" (NUEVO) ---
                    lblPerfilNombreCompleto.Text = doctor.Nom + " " + doctor.Ape;
                    lblPerfilEmail.Text = doctor.Email;
                    lblPerfilTelefono.Text = string.IsNullOrEmpty(doctor.Tel) ? "No registrado" : doctor.Tel;

                    // Datos Profesionales
                    lblPerfilEspecialidad.Text = doctor.NombreEspecialidad;
                    lblPerfilCMP.Text = doctor.CodMed; // Código Médico (Colegiatura)
                    lblPerfilExperiencia.Text = (doctor.Expe.HasValue ? doctor.Expe.Value.ToString() : "0") + " años";
                    lblPerfilID.Text = doctor.Id; // Muestra el ID (ej: D0000001)
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
            DateTime fechaHoy = DateTime.Now.Date; // Obtiene la fecha de hoy (sin la hora)

            // Llama al DAO para obtener la lista de citas
            var listaCitas = citasDAO.ListarCitasDelDia(doctor.Id, fechaHoy);

            if (listaCitas.Count > 0)
            {
                // Hay citas, las enlazamos al Repeater
                repeaterCitas.DataSource = listaCitas;
                repeaterCitas.DataBind();
                lblNoCitas.Visible = false;
            }
            else
            {
                // No hay citas, mostramos el mensaje
                repeaterCitas.DataSource = null;
                repeaterCitas.DataBind();
                lblNoCitas.Visible = true;
            }
        }
        // --- AÑADIR NUEVO MÉTODO PARA CARGAR DROPDOWNS ---
        private void CargarDropdownsModal()
        {
            try
            {
                // 1. Cargar Pacientes
                ddlPacienteModal.DataSource = pacientesDAO.ListarPacientesParaDropdown();
                ddlPacienteModal.DataTextField = "NombreCompleto"; // Asumiendo que Pacientes.cs tiene esta propiedad
                ddlPacienteModal.DataValueField = "ID";
                ddlPacienteModal.DataBind();
                ddlPacienteModal.Items.Insert(0, new ListItem("-- Seleccionar Paciente --", ""));

                // 2. Cargar Sedes
                ddlSedeModal.DataSource = sedesDAO.ListarSedes();
                ddlSedeModal.DataTextField = "NomSede";
                ddlSedeModal.DataValueField = "ID";
                ddlSedeModal.DataBind();
                ddlSedeModal.Items.Insert(0, new ListItem("-- Seleccionar Sede --", ""));

                // 3. Cargar Horas (Ejemplo simple)
                if (ddlHoraModal.Items.Count <= 1) // Solo cargar si está vacío
                {
                    ddlHoraModal.Items.Clear();
                    ddlHoraModal.Items.Insert(0, new ListItem("-- Seleccionar Hora --", ""));

                    // Definimos el inicio (6:00 AM) y el fin (11:00 PM)
                    DateTime startTime = DateTime.Today.AddHours(6); // 6:00
                    DateTime endTime = DateTime.Today.AddHours(23); // 23:00

                    DateTime currentTime = startTime;
                    while (currentTime <= endTime)
                    {
                        // Texto (lo que el usuario ve): ej. "06:30 AM"
                        string displayText = currentTime.ToString("hh:mm tt");

                        // Valor (lo que guardamos): ej. "06:30" o "23:00"
                        string valueText = currentTime.ToString("HH:mm");

                        ddlHoraModal.Items.Add(new ListItem(displayText, valueText));

                        // Avanzamos 30 minutos
                        currentTime = currentTime.AddMinutes(30);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeModal("Error al cargar datos: " + ex.Message, "error");
            }
        }
        /// <summary>
        /// Maneja TODOS los clics de botones dentro del Repeater de Citas.
        /// </summary>
        protected void repeaterCitas_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            // Obtenemos el ID de la cita desde el botón que se presionó
            string idCita = e.CommandArgument.ToString();

            // Usamos un switch para ver qué botón fue
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
        //
        private void AbrirModalFinalizar(string idCita)
        {
            try
            {
                // 1. Buscar el ID de la consulta que se creó al "Iniciar"
                string idConsulta = citasDAO.ObtenerConsultaIdPorCitaId(idCita);

                if (string.IsNullOrEmpty(idConsulta))
                {
                    // Si no se encuentra (ej. el doctor nunca dio "Iniciar"), lo creamos AHORA.
                    idConsulta = citasDAO.IniciarConsulta(idCita);

                    // Si AÚN no hay ID, lanzamos error.
                    if (string.IsNullOrEmpty(idConsulta))
                    {
                        throw new Exception("No se pudo iniciar el registro de consulta. Contacte a soporte.");
                    }

                    // Refrescamos la lista para que el botón "Iniciar" cambie a "Finalizar"
                    CargarCitasDelDia();
                    // Asegúrate de tener un UpdatePanel llamado "UpdatePanelContenido"
                    // si no, comenta la siguiente línea
                }

                // 2. Guardar los IDs en los campos ocultos del modal
                hfConsultaIdCita.Value = idCita;
                hfConsultaIdConsulta.Value = idConsulta;

                // 3. Limpiar campos del modal
                txtSintomas.Text = "";
                txtDiagnostico.Text = "";
                txtTratamiento.Text = "";
                txtObservaciones.Text = "";
                lblModalConsultaMensaje.CssClass = "modal-mensaje";
                lblModalConsultaMensaje.Text = "";

                // 4. Llamar al JS para mostrar el modal
                // (Asegúrate de tener un ScriptManager en tu .aspx)
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowFinalizarModal", "showFinalizarModal();", true);
            }
            catch (Exception ex)
            {
                // Muestra el error en el modal de AGENDAR CITA
                MostrarMensajeModal("Error: " + ex.Message, "error");
            }
        }

        protected void btnGuardarConsulta_Click(object sender, EventArgs e)
{
    try
    {
        // 1. Recuperar los IDs de los campos ocultos
        string idCita = hfConsultaIdCita.Value;
        string idConsulta = hfConsultaIdConsulta.Value;

        // 2. Validar que los IDs existan
        if (string.IsNullOrEmpty(idCita) || string.IsNullOrEmpty(idConsulta))
        {
            MostrarMensajeConsultaModal("Error de sesión. Cierre el modal y vuelva a intentarlo.", "error");
            return;
        }

        // 3. Crear el objeto ConsultasMedicas
        ConsultasMedicas consulta = new ConsultasMedicas
        {
            Id = idConsulta,
            IdCita = idCita,
            Sintomas = txtSintomas.Text,
            Diagnostico = txtDiagnostico.Text,
            Tratamiento = txtTratamiento.Text,
            Observaciones = txtObservaciones.Text
        };

        // 4. Llamar al DAO para guardar en la BD
        citasDAO.FinalizarConsulta(consulta); // Llama al SP sp_PanelMedico_FinalizarConsulta

        // 5. Refrescar la lista de citas (para que el estado cambie a 'F')
        CargarCitasDelDia();
        // (Asegúrate de tener un UpdatePanel)
        //UpdatePanelContenido.Update(); // Refresca el panel de citas

        // 6. Cerrar el modal
        ScriptManager.RegisterStartupScript(this, this.GetType(), "HideFinalizarModal", "hideFinalizarModal();", true);
    }
    catch (Exception ex)
    {
        // Muestra el error DENTRO del modal de consulta
        MostrarMensajeConsultaModal("Error al guardar: " + ex.Message, "error");
    }
}
        private void MostrarMensajeConsultaModal(string mensaje, string tipo) // tipo = "success" o "error"
        {
            lblModalConsultaMensaje.Text = mensaje;
            lblModalConsultaMensaje.CssClass = "modal-mensaje show " + tipo;

            // Mantiene el modal abierto para que se vea el error
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowFinalizarModalOnError",
                $"showFinalizarModal();", true);
        }
        // -- bd
        protected void btnGuardarCita_Click(object sender, EventArgs e)
        {
            Doctores doctor = doctoresDAO.ObtenerDoctorPorIdUsuario(CurrentUserId);

            try
            {
                // 1. Validaciones
                if (string.IsNullOrEmpty(ddlPacienteModal.SelectedValue) ||
                    string.IsNullOrEmpty(ddlSedeModal.SelectedValue) ||
                    string.IsNullOrEmpty(txtFechaModal.Text) ||
                    string.IsNullOrEmpty(ddlHoraModal.SelectedValue))
                {
                    MostrarMensajeModal("Por favor, complete todos los campos obligatorios.", "error");
                    return;
                }

                // 2. Obtener la especialidad del doctor logueado
                
                if (doctor == null)
                {
                    MostrarMensajeModal("Error: No se pudo encontrar al doctor.", "error");
                    return;
                }

                // 3. Crear el objeto Cita
                Citas nuevaCita = new Citas
                {
                    IdPac = ddlPacienteModal.SelectedValue,
                    IdDoc = doctor.Id,
                    IdSede = ddlSedeModal.SelectedValue,
                    IdEsp = doctor.IdEsp, // Asignamos la especialidad del doctor
                    Fecha = Convert.ToDateTime(txtFechaModal.Text),
                    Hora = TimeSpan.Parse(ddlHoraModal.SelectedValue),
                    Motivo = txtMotivoModal.Text,
                    TipoPago = "Presencial", // Valor por defecto
                    Monto = 0,                // Valor por defecto
                    PagoReali = false
                };

                // 4. Guardar en la BD
                string nuevoId = citasDAO.RegistrarCita(nuevaCita);

                // 5. Refrescar el dashboard (¡Importante!)
                CargarCitasDelDia();

                // 6. Mostrar mensaje de éxito y CERRAR el modal
                // Limpiamos los campos
                LimpiarModal();
                // Llamamos al JS para que oculte el "cuadro"
                ScriptManager.RegisterStartupScript(this, this.GetType(), "HideModal", "hideModalFromCodeBehind();", true);
            }
            catch (SqlException sqEx)
            {
                MostrarMensajeModal("Error de Base de Datos: " + sqEx.Message, "error");
            }
            catch (Exception ex)
            {
                MostrarMensajeModal("Error al guardar la cita: " + ex.Message, "error");
            }
        }
        // modal
        // --- AÑADIR MÉTODOS DE AYUDA PARA EL MODAL ---
        private void MostrarMensajeModal(string mensaje, string tipo) // tipo = "success" o "error"
        {
            // 1. Mostrar el mensaje en el Label (esto está bien)
            lblModalMensaje.Text = mensaje;
            lblModalMensaje.CssClass = "modal-mensaje show " + tipo;

            // 2. Escapar el mensaje para que sea seguro en JavaScript
            string mensajeEscapado = mensaje
                .Replace("'", "\\'")   // Escapa comillas simples
                .Replace("\r", "\\r")  // Escapa retornos de carro
                .Replace("\n", "\\n"); // Escapa nuevas líneas

            // 3. Usar el mensaje escapado en el script
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowModalOnError",
                $"showModalFromCodeBehind('{mensajeEscapado}', '{tipo}');", true);
        }
        // -- Limpiar model
        private void LimpiarModal()
        {
            ddlPacienteModal.SelectedIndex = 0;
            ddlSedeModal.SelectedIndex = 0;
            ddlHoraModal.SelectedIndex = 0;
            txtFechaModal.Text = string.Empty;
            txtMotivoModal.Text = string.Empty;
            lblModalMensaje.Text = string.Empty;
            lblModalMensaje.CssClass = "modal-mensaje";
        }
        // --- Lógica de los botones ---

        private void IniciarConsulta(string idCita)
        {
            // Lógica para "Iniciar Consulta" (RF08)
            try
            {
                // 1. Llamamos al DAO para que inicie la consulta y cree el registro
                string nuevoIdConsulta = citasDAO.IniciarConsulta(idCita);

                // 2. (Simulación) Mostramos un mensaje o actualizamos la UI
                System.Diagnostics.Debug.WriteLine($"Acción: Iniciar Consulta para CitaID: {idCita}. Nuevo ID de Consulta: {nuevoIdConsulta}");

                // 3. Volvemos a cargar la lista para que se actualice el estado a 'I' (Iniciada)
                CargarCitasDelDia();

                // (En un futuro, podríamos redirigir a una página de consulta)
                // Response.Redirect($"DetalleConsulta.aspx?ConsultaID={nuevoIdConsulta}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR al iniciar consulta: {ex.Message}");
                // (Aquí deberías mostrar un mensaje de error al usuario)
            }
        }

        private void VerFicha(string idCita)
        {
            try
            {
                // 1. Obtener los datos completos desde el DAO
                DetalleCitaDTO ficha = citasDAO.ObtenerFichaCita(idCita);

                if (ficha != null)
                {
                    // 2. Llenar los campos del modal
                    lblFichaId.Text = ficha.IdCita;

                    // Paciente
                    lblFichaPaciente.Text = ficha.PacienteNombre;
                    lblFichaDNI.Text = ficha.PacienteDNI;
                    lblFichaTelefono.Text = ficha.PacienteTelefono;
                    lblFichaPeso.Text = ficha.PacientePeso.HasValue ? ficha.PacientePeso.Value.ToString("0.0") : "N/A";

                    // Cita
                    lblFichaFecha.Text = ficha.Fecha.ToString("dd/MM/yyyy") + " - " + DateTime.Today.Add(ficha.Hora).ToString("hh:mm tt");
                    lblFichaSede.Text = ficha.Sede;
                    lblFichaEspecialidad.Text = ficha.Especialidad;
                    lblFichaEstado.Text = ficha.Estado == "P" ? "Pendiente" : (ficha.Estado == "F" ? "Finalizada" : "En Progreso");

                    // Colorear el estado
                    lblFichaEstado.ForeColor = ficha.Estado == "F" ? System.Drawing.Color.Green : System.Drawing.Color.Orange;

                    // Resumen Clínico
                    lblFichaMotivo.Text = ficha.Motivo;
                    lblFichaDiagnostico.Text = ficha.Diagnostico;
                    lblFichaTratamiento.Text = ficha.Tratamiento;
                    lblFichaObservaciones.Text = ficha.Observaciones;

                    // 3. Mostrar el modal
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowFichaModal", "showFichaModal();", true);
                }
                else
                {
                    MostrarMensajeModal("No se encontraron detalles para esta cita.", "error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeModal("Error al cargar la ficha: " + ex.Message, "error");
            }
        }

        // --- Eventos de botones FUERA del Repeater ---

        protected void btnNuevaCita_Click(object sender, EventArgs e)
        {
            
        }

        //Método ya "desloguea la cuenta"

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

    }
}