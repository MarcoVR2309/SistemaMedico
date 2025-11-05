using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using SistemaMedico.datos;
using SistemaMedico.modelo;
using System.Linq;

namespace SistemaMedico.vista
{
    public partial class PanelPaciente : System.Web.UI.Page
    {
        // Propiedades auxiliares para leer la Sesión
        private string CurrentUserId
        {
            get { return Session["UsuarioId"]?.ToString() ?? "U000001"; } 
        }

        private string CurrentUserNameCompleto
        {
            get { return Session["UsuarioNombreCompleto"]?.ToString() ?? "Paciente Invitado"; }
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

                CargarEspecialidades();
                CargarCitasPendientes(CurrentUserId); 
                CargarDatosPerfil(CurrentUserId);

                if (ddlEspecialidades.Items.Count > 1 && !string.IsNullOrEmpty(ddlEspecialidades.SelectedValue))
                {
                    CargarDoctores(ddlEspecialidades.SelectedValue);
                }
                else
                {
                    ddlDoctores.Items.Clear();
                    ddlDoctores.Items.Insert(0, new ListItem("-- Seleccione Especialidad Primero --", ""));
                }
            }
            string activeTab = hdnActiveTab.Value ?? "misCitas";
            ClientScript.RegisterStartupScript(this.GetType(), "ActivateTabScript", $"changeTab('{activeTab}');", true);
        }

        private void CargarEspecialidades()
        {
            EspecialidadesDAO dao = new EspecialidadesDAO();
            try
            {
                List<Especialidades> lista = dao.ListarEspecialidades();
                
                ddlEspecialidades.DataSource = lista;
                ddlEspecialidades.DataBind();
                
                ddlEspecialidades.Items.Insert(0, new ListItem("-- Seleccione Especialidad --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensajeGlobalError("Error al cargar especialidades: " + ex.Message);
            }
        }

        private void CargarDoctores(string idEsp)
        {
            DoctoresDAO dao = new DoctoresDAO();
            ddlDoctores.Items.Clear();
            ddlHoras.Items.Clear();
            ddlHoras.Items.Insert(0, new ListItem("-- Seleccione Doctor y Fecha --", ""));

            if (string.IsNullOrEmpty(idEsp))
            {
                ddlDoctores.Items.Insert(0, new ListItem("-- Seleccione Especialidad Primero --", ""));
                return;
            }

            try
            {
                List<Doctores> todosLosDoctores = dao.ListarDoctores();
                List<Doctores> doctoresFiltrados = todosLosDoctores
                    .Where(d => d.IdEsp == idEsp)
                    .ToList();
                
                var listaMapeada = doctoresFiltrados.Select(d => new 
                {
                    Id = d.Id,
                    NombreCompleto = $"{d.Nom} {d.Ape} ({d.NombreEspecialidad})"
                }).ToList();

                ddlDoctores.DataSource = listaMapeada;
                ddlDoctores.DataTextField = "NombreCompleto"; 
                ddlDoctores.DataValueField = "Id";
                ddlDoctores.DataBind();
                ddlDoctores.Items.Insert(0, new ListItem("-- Seleccione Doctor --", ""));
                
                if (listaMapeada.Count == 1) ddlDoctores.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                MostrarMensajeGlobalError("Error al cargar doctores: " + ex.Message);
            }
        }
        
        private void CargarCitasPendientes(string idPaciente)
        {
            
            try
            {
                gvCitas.DataSource = new List<object>(); 
                gvCitas.DataBind();
            }
            catch (Exception)
            {
                // Manejo de errores de citas
            }
        }

        private void CargarDatosPerfil(string idUsuario)
        {
            string nombreCompleto = Session["UsuarioNombreCompleto"]?.ToString();
            string email = Session["UsuarioEmail"]?.ToString();
            string dni = Session["UsuarioDNI"]?.ToString();
            string tel = Session["UsuarioTelefono"]?.ToString();
            DateTime? fechaNac = (DateTime?)Session["UsuarioFechaNac"];

            txtProfileNombre.Text = nombreCompleto;
            txtProfileEmail.Text = email;
            txtProfileTelefono.Text = tel;
            txtProfileUserId.Text = idUsuario;
            txtProfileDNI.Text = dni;           
            txtProfileFechaNac.Text = fechaNac.HasValue ? fechaNac.Value.ToShortDateString() : "No especificado";
        }
        
        protected void ddlEspecialidades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEspecialidades.SelectedValue))
            {
                CargarDoctores(ddlEspecialidades.SelectedValue);
            }
            ddlHoras.Items.Clear();
            ddlHoras.Items.Insert(0, new ListItem("-- Seleccione Fecha --", ""));
        }
        
        protected void ddlDoctores_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarHorasDisponibles(); 
        }

        protected void txtFechaCita_TextChanged(object sender, EventArgs e)
        {
             CargarHorasDisponibles(); 
        }
        private void CargarHorasDisponibles()
        {
            ddlHoras.Items.Clear();
            string idDoctor = ddlDoctores.SelectedValue;
            string fecha = txtFechaCita.Text; 

            if (string.IsNullOrEmpty(idDoctor) || string.IsNullOrEmpty(fecha) || idDoctor == "-- Seleccione Doctor --")
            {
                ddlHoras.Items.Insert(0, new ListItem("-- Seleccione Doctor y Fecha --", ""));
                return;
            }
            ddlHoras.Items.Add(new ListItem("10:00 AM", "10:00"));
            ddlHoras.Items.Add(new ListItem("11:00 AM", "11:00"));
            ddlHoras.Items.Add(new ListItem("14:00 PM", "14:00"));
            ddlHoras.Items.Insert(0, new ListItem("-- Seleccione hora --", ""));
        }
        
        protected void btnReservar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlEspecialidades.SelectedValue) || 
                string.IsNullOrEmpty(ddlDoctores.SelectedValue) ||
                string.IsNullOrEmpty(txtFechaCita.Text) ||
                string.IsNullOrEmpty(ddlHoras.SelectedValue))
            {
                MostrarMensajeGlobalError("Por favor, complete todos los campos de la cita.");
                return;
            }
            
            MostrarMensajeGlobalExito($"¡Cita reservada para {CurrentUserNameCompleto}!");
            CargarCitasPendientes(CurrentUserId);
        }
        
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("Login.aspx");
        }

        // =========================================================
        // MÉTODOS DE UTILIDAD PARA MENSAJES GLOBALES
        // =========================================================

        private void MostrarMensajeGlobalError(string mensaje)
        {
            lblMensajeGlobal.Text = "ERROR: " + mensaje;
            lblMensajeGlobal.CssClass = "status-message status-error";
            lblMensajeGlobal.Visible = true;
        }

        private void MostrarMensajeGlobalExito(string mensaje)
        {
            lblMensajeGlobal.Text = mensaje;
            lblMensajeGlobal.CssClass = "status-message status-success";
            lblMensajeGlobal.Visible = true;
        }
    }
}
