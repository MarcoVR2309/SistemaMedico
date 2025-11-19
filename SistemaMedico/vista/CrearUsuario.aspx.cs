using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaMedico.conexion;
using SistemaMedico.datos;

namespace SistemaMedico.vista
{
    public partial class CrearUsuario : System.Web.UI.Page
    {
        private ConexionDB conexionDB;
        private SedesDAO sedesDAO;
        private EspecialidadesDAO especialidadesDAO;

        public CrearUsuario()
        {
            conexionDB = new ConexionDB();
            sedesDAO = new SedesDAO();
            especialidadesDAO = new EspecialidadesDAO();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Verificar que el usuario esté logueado y sea administrador
            if (Session["UsuarioId"] == null)
            {
                Response.Redirect("~/vista/Login.aspx");
                return;
            }

            string rol = Session["UsuarioRol"]?.ToString();
            if (rol != "Administrador")
            {
                Response.Redirect("~/vista/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CargarDatosIniciales();
            }
        }

        private void CargarDatosIniciales()
        {
            // Cargar roles
            CargarRoles();

            // Cargar sedes
            CargarSedes();

            // Cargar especialidades
            CargarEspecialidades();
        }

        private void CargarRoles()
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    string query = "SELECT ID, NOM_ROL FROM Roles ORDER BY NOM_ROL";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlRol.Items.Clear();
                    ddlRol.Items.Add(new ListItem("Seleccione un rol...", ""));

                    while (reader.Read())
                    {
                        ddlRol.Items.Add(new ListItem(reader["NOM_ROL"].ToString(), reader["ID"].ToString()));
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar roles: {ex.Message}");
            }
        }

        private void CargarSedes()
        {
            try
            {
                var sedes = sedesDAO.ListarSedes();
                ddlSede.Items.Clear();
                ddlSede.Items.Add(new ListItem("Seleccione...", ""));

                foreach (var sede in sedes)
                {
                    ddlSede.Items.Add(new ListItem(sede.NomSede, sede.Id));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar sedes: {ex.Message}");
            }
        }

        private void CargarEspecialidades()
        {
            try
            {
                var especialidades = especialidadesDAO.ListarEspecialidades();
                ddlEspecialidad.Items.Clear();
                ddlEspecialidad.Items.Add(new ListItem("Seleccione...", ""));

                foreach (var esp in especialidades)
                {
                    ddlEspecialidad.Items.Add(new ListItem(esp.NomEsp, esp.Id));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar especialidades: {ex.Message}");
            }
        }

        protected void ddlRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ocultar todos los paneles dinámicos
            pnlMedico.CssClass = "dynamic-fields";
            pnlAdministrador.CssClass = "dynamic-fields";
            pnlPaciente.CssClass = "dynamic-fields";

            // Mostrar el panel correspondiente según el rol seleccionado
            string rolSeleccionado = ddlRol.SelectedValue;

            if (rolSeleccionado == "R0000002") // Médico
            {
                pnlMedico.CssClass = "dynamic-fields show";
            }
            else if (rolSeleccionado == "R0000001") // Administrador
            {
                pnlAdministrador.CssClass = "dynamic-fields show";
            }
            else if (rolSeleccionado == "R0000003") // Paciente
            {
                pnlPaciente.CssClass = "dynamic-fields show";
            }
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario())
            {
                return;
            }

            try
            {
                string idUsuarioGenerado = null;
                string idRolGenerado = null;

                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("Admin_CrearUsuarioCompleto", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros básicos
                    cmd.Parameters.AddWithValue("@IdRol", ddlRol.SelectedValue);
                    cmd.Parameters.AddWithValue("@TipoDoc", ddlTipoDoc.SelectedValue);
                    cmd.Parameters.AddWithValue("@NumDoc", txtNumDoc.Text.Trim());
                    cmd.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                    cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@Genero", ddlGenero.SelectedValue);
                    cmd.Parameters.AddWithValue("@FechaNac", Convert.ToDateTime(txtFechaNac.Text));
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@SedePref", 
                        string.IsNullOrEmpty(ddlSede.SelectedValue) ? (object)DBNull.Value : ddlSede.SelectedValue);

                    // Parámetros específicos según rol
                    string rolSeleccionado = ddlRol.SelectedValue;

                    if (rolSeleccionado == "R0000002") // Médico
                    {
                        cmd.Parameters.AddWithValue("@IdEspecialidad", ddlEspecialidad.SelectedValue);
                        cmd.Parameters.AddWithValue("@NumColegiatura", txtNumColegiatura.Text.Trim());
                        cmd.Parameters.AddWithValue("@AniosExperiencia", Convert.ToInt32(txtExperiencia.Text));
                        cmd.Parameters.AddWithValue("@NivelAcceso", DBNull.Value);
                    }
                    else if (rolSeleccionado == "R0000001") // Administrador
                    {
                        cmd.Parameters.AddWithValue("@IdEspecialidad", DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumColegiatura", DBNull.Value);
                        cmd.Parameters.AddWithValue("@AniosExperiencia", DBNull.Value);
                        cmd.Parameters.AddWithValue("@NivelAcceso", Convert.ToInt32(ddlNivelAcceso.SelectedValue));
                    }
                    else // Paciente
                    {
                        cmd.Parameters.AddWithValue("@IdEspecialidad", DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumColegiatura", DBNull.Value);
                        cmd.Parameters.AddWithValue("@AniosExperiencia", DBNull.Value);
                        cmd.Parameters.AddWithValue("@NivelAcceso", DBNull.Value);
                    }

                    // Parámetros de salida
                    SqlParameter paramIdUsuario = new SqlParameter("@IdGeneradoUsuario", SqlDbType.VarChar, 20);
                    paramIdUsuario.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramIdUsuario);

                    SqlParameter paramIdRol = new SqlParameter("@IdGeneradoRol", SqlDbType.VarChar, 20);
                    paramIdRol.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramIdRol);

                    cmd.ExecuteNonQuery();

                    idUsuarioGenerado = paramIdUsuario.Value?.ToString();
                    idRolGenerado = paramIdRol.Value?.ToString();
                }

                // Registrar la acción del administrador
                string adminId = Session["AdminId"]?.ToString();
                if (!string.IsNullOrEmpty(adminId))
                {
                    AdministradoresDAO adminDAO = new AdministradoresDAO();
                    string nombreRol = ddlRol.SelectedItem.Text;
                    adminDAO.RegistrarAccion(adminId, "Crear Usuario", 
                        $"Usuario creado: {txtNombre.Text} {txtApellido.Text} ({nombreRol})");
                }

                // Mostrar mensaje de éxito y redirigir
                MostrarMensaje("Usuario creado exitosamente", "success");
                
                // Redirigir después de 2 segundos
                ClientScript.RegisterStartupScript(this.GetType(), "redirect", 
                    "setTimeout(function(){ window.location.href = 'PanelAdministrador.aspx'; }, 2000);", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear usuario: {ex.Message}");
                MostrarMensaje($"Error al crear usuario: {ex.Message}", "danger");
            }
        }

        private bool ValidarFormulario()
        {
            // Validar campos obligatorios básicos
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarMensaje("El nombre es obligatorio", "danger");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MostrarMensaje("El apellido es obligatorio", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(ddlTipoDoc.SelectedValue))
            {
                MostrarMensaje("Debe seleccionar un tipo de documento", "danger");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNumDoc.Text))
            {
                MostrarMensaje("El número de documento es obligatorio", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(ddlGenero.SelectedValue))
            {
                MostrarMensaje("Debe seleccionar un género", "danger");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFechaNac.Text))
            {
                MostrarMensaje("La fecha de nacimiento es obligatoria", "danger");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MostrarMensaje("El email es obligatorio", "danger");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MostrarMensaje("El teléfono es obligatorio", "danger");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text) || txtPassword.Text.Length < 6)
            {
                MostrarMensaje("La contraseña debe tener al menos 6 caracteres", "danger");
                return false;
            }

            if (string.IsNullOrEmpty(ddlRol.SelectedValue))
            {
                MostrarMensaje("Debe seleccionar un rol", "danger");
                return false;
            }

            // Validar campos específicos según el rol
            string rolSeleccionado = ddlRol.SelectedValue;

            if (rolSeleccionado == "R0000002") // Médico
            {
                if (string.IsNullOrEmpty(ddlEspecialidad.SelectedValue))
                {
                    MostrarMensaje("Debe seleccionar una especialidad para el médico", "danger");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtNumColegiatura.Text))
                {
                    MostrarMensaje("El número de colegiatura es obligatorio para médicos", "danger");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtExperiencia.Text))
                {
                    MostrarMensaje("Los años de experiencia son obligatorios para médicos", "danger");
                    return false;
                }
            }

            return true;
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            lblMensaje.Text = $"<i class='fas fa-{(tipo == "success" ? "check-circle" : "exclamation-circle")}'></i> {mensaje}";
            pnlMensaje.CssClass = $"alert alert-{tipo} show";
            pnlMensaje.Visible = true;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/vista/PanelAdministrador.aspx");
        }

        protected void btnCerrar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/vista/PanelAdministrador.aspx");
        }
    }
}
