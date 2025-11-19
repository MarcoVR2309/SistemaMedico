using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using SistemaMedico.conexion;
using SistemaMedico.datos;
using SistemaMedico.modelo;

namespace SistemaMedico.vista
{
    public partial class PanelAdministrador : System.Web.UI.Page
    {
        private ConexionDB conexionDB;
        private RolesDAO rolesDAO;
        private SedesDAO sedesDAO;
        private EspecialidadesDAO especialidadesDAO;

        public PanelAdministrador()
        {
            conexionDB = new ConexionDB();
            rolesDAO = new RolesDAO();
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
            // Cargar información del administrador
            lblNombreAdmin.Text = Session["UsuarioNombreCompleto"]?.ToString() ?? "Administrador";

            // Cargar roles en el dropdown
            CargarRoles();

            // Cargar roles para el modal
            CargarRolesModal();

            // Cargar sedes
            CargarSedes();

            // Cargar especialidades
            CargarEspecialidades();

            // Cargar estadísticas
            CargarEstadisticas();

            // Cargar usuarios
            CargarUsuarios(null, null);
            
            // Cargar listas de especialidades y sedes
            CargarListaEspecialidades();
            CargarListaSedes();
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

                    ddlFiltroRol.Items.Clear();
                    ddlFiltroRol.Items.Add(new ListItem("Todos los roles", ""));

                    while (reader.Read())
                    {
                        ddlFiltroRol.Items.Add(new ListItem(reader["NOM_ROL"].ToString(), reader["ID"].ToString()));
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar roles: {ex.Message}");
            }
        }

        private void CargarRolesModal()
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
                System.Diagnostics.Debug.WriteLine($"Error al cargar roles del modal: {ex.Message}");
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

        private void CargarEstadisticas()
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_Admin_ObtenerEstadisticasUsuarios", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblTotalUsuarios.Text = reader["TotalUsuarios"].ToString();
                        lblDoctores.Text = reader["TotalDoctores"].ToString();
                        lblPacientes.Text = reader["TotalPacientes"].ToString();
                        lblActivos.Text = reader["UsuariosActivos"].ToString();
                        lblAdministradores.Text = reader["TotalAdministradores"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar estadísticas: {ex.Message}");
                lblTotalUsuarios.Text = "0";
                lblDoctores.Text = "0";
                lblPacientes.Text = "0";
                lblActivos.Text = "0";
            }
        }

        private void CargarUsuarios(string filtroRol, string filtroBusqueda)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("Admin_ListarUsuarios", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FiltroRol", string.IsNullOrEmpty(filtroRol) ? (object)DBNull.Value : filtroRol);
                    cmd.Parameters.AddWithValue("@FiltroBusqueda", string.IsNullOrEmpty(filtroBusqueda) ? (object)DBNull.Value : filtroBusqueda);
                    cmd.Parameters.AddWithValue("@SoloActivos", 0); // Mostrar todos (activos e inactivos)

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvUsuarios.DataSource = dt;
                    gvUsuarios.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar usuarios: {ex.Message}");
                gvUsuarios.DataSource = null;
                gvUsuarios.DataBind();
            }
        }

        // =====================================================
        // MÉTODOS PARA ESPECIALIDADES
        // =====================================================

        private void CargarListaEspecialidades()
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("Admin_ListarEspecialidadesConDoctores", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvEspecialidades.DataSource = dt;
                    gvEspecialidades.DataBind();

                    lblTotalEspecialidades.Text = dt.Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar especialidades: {ex.Message}");
                gvEspecialidades.DataSource = null;
                gvEspecialidades.DataBind();
            }
        }

        protected void btnCrearEspecialidad_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreEspecialidad.Text))
            {
                MostrarMensajeEsp("El nombre de la especialidad es obligatorio", "danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalEsp", "abrirModalEspecialidad();", true);
                return;
            }

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertarEspecialidad", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nombre", txtNombreEspecialidad.Text.Trim());
                    cmd.Parameters.AddWithValue("@Descripcion", 
                        string.IsNullOrWhiteSpace(txtDescripcionEspecialidad.Text) ? (object)DBNull.Value : txtDescripcionEspecialidad.Text.Trim());

                    SqlParameter paramId = new SqlParameter("@IdGenerado", SqlDbType.VarChar, 20);
                    paramId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramId);

                    cmd.ExecuteNonQuery();
                }

                string adminId = Session["AdminId"]?.ToString();
                if (!string.IsNullOrEmpty(adminId))
                {
                    AdministradoresDAO adminDAO = new AdministradoresDAO();
                    adminDAO.RegistrarAccion(adminId, "Crear Especialidad", $"Especialidad creada: {txtNombreEspecialidad.Text}");
                }

                LimpiarFormularioEspecialidad();
                CargarListaEspecialidades();
                CargarEspecialidades();

                MostrarMensajeEsp("Especialidad creada exitosamente", "success");
                ScriptManager.RegisterStartupScript(this, GetType(), "closeModalEsp", 
                    "setTimeout(function(){ cerrarModalEspecialidad(); }, 2000);", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear especialidad: {ex.Message}");
                MostrarMensajeEsp($"Error al crear especialidad: {ex.Message}", "danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalEsp", "abrirModalEspecialidad();", true);
            }
        }

        protected void gvEspecialidades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string idEspecialidad = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                // TODO: Implementar edición
            }
        }

        private void MostrarMensajeEsp(string mensaje, string tipo)
        {
            lblMensajeEspecialidad.Text = $"<i class='fas fa-{(tipo == "success" ? "check-circle" : "exclamation-circle")}'></i> {mensaje}";
            lblMensajeEspecialidad.CssClass = $"alert alert-{tipo} show";
            lblMensajeEspecialidad.Visible = true;
        }

        private void LimpiarFormularioEspecialidad()
        {
            txtNombreEspecialidad.Text = "";
            txtDescripcionEspecialidad.Text = "";
            lblMensajeEspecialidad.Visible = false;
        }

        // =====================================================
        // MÉTODOS PARA SEDES
        // =====================================================

        private void CargarListaSedes()
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("Admin_ListarSedesConEstadisticas", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    gvSedes.DataSource = dt;
                    gvSedes.DataBind();

                    lblTotalSedes.Text = dt.Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar sedes: {ex.Message}");
                gvSedes.DataSource = null;
                gvSedes.DataBind();
            }
        }

        protected void btnCrearSede_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreSede.Text))
            {
                MostrarMensajeSede("El nombre de la sede es obligatorio", "danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalSede", "abrirModalSede();", true);
                return;
            }

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertarSede", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nombre", txtNombreSede.Text.Trim());
                    cmd.Parameters.AddWithValue("@Direccion", 
                        string.IsNullOrWhiteSpace(txtDireccionSede.Text) ? (object)DBNull.Value : txtDireccionSede.Text.Trim());
                    cmd.Parameters.AddWithValue("@Telefono", 
                        string.IsNullOrWhiteSpace(txtTelefonoSede.Text) ? (object)DBNull.Value : txtTelefonoSede.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", 
                        string.IsNullOrWhiteSpace(txtEmailSede.Text) ? (object)DBNull.Value : txtEmailSede.Text.Trim());

                    SqlParameter paramId = new SqlParameter("@IdGenerado", SqlDbType.VarChar, 20);
                    paramId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramId);

                    cmd.ExecuteNonQuery();
                }

                string adminId = Session["AdminId"]?.ToString();
                if (!string.IsNullOrEmpty(adminId))
                {
                    AdministradoresDAO adminDAO = new AdministradoresDAO();
                    adminDAO.RegistrarAccion(adminId, "Crear Sede", $"Sede creada: {txtNombreSede.Text}");
                }

                LimpiarFormularioSede();
                CargarListaSedes();
                CargarSedes();

                MostrarMensajeSede("Sede creada exitosamente", "success");
                ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSede", 
                    "setTimeout(function(){ cerrarModalSede(); }, 2000);", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear sede: {ex.Message}");
                MostrarMensajeSede($"Error al crear sede: {ex.Message}", "danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalSede", "abrirModalSede();", true);
            }
        }

        protected void gvSedes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string idSede = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                // TODO: Implementar edición
            }
        }

        private void MostrarMensajeSede(string mensaje, string tipo)
        {
            lblMensajeSede.Text = $"<i class='fas fa-{(tipo == "success" ? "check-circle" : "exclamation-circle")}'></i> {mensaje}";
            pnlMensajeSede.CssClass = $"alert alert-{tipo} show";
            pnlMensajeSede.Visible = true;
        }

        private void LimpiarFormularioSede()
        {
            txtNombreSede.Text = "";
            txtDireccionSede.Text = "";
            txtTelefonoSede.Text = "";
            txtEmailSede.Text = "";
            pnlMensajeSede.Visible = false;
        }

        // =====================================================
        // MÉTODOS PARA USUARIOS (FUNCIONANDO)
        // =====================================================

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string filtroRol = ddlFiltroRol.SelectedValue;
            string filtroBusqueda = txtBusqueda.Text.Trim();

            CargarUsuarios(filtroRol, filtroBusqueda);
        }

        protected void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario())
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "abrirModalCrearUsuario();", true);
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

                string adminId = Session["AdminId"]?.ToString();
                if (!string.IsNullOrEmpty(adminId))
                {
                    AdministradoresDAO adminDAO = new AdministradoresDAO();
                    string nombreRol = ddlRol.SelectedItem.Text;
                    adminDAO.RegistrarAccion(adminId, "Crear Usuario", 
                        $"Usuario creado: {txtNombre.Text} {txtApellido.Text} ({nombreRol})");
                }

                LimpiarFormularioModal();
                CargarEstadisticas();
                CargarUsuarios(null, null);

                MostrarMensaje("Usuario creado exitosamente", "success");
                ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", 
                    "setTimeout(function(){ cerrarModalCrearUsuario(); }, 2000);", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear usuario: {ex.Message}");
                MostrarMensaje($"Error al crear usuario: {ex.Message}", "danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "abrirModalCrearUsuario();", true);
            }
        }

        private bool ValidarFormulario()
        {
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

        private void LimpiarFormularioModal()
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            ddlTipoDoc.SelectedIndex = 0;
            txtNumDoc.Text = "";
            ddlGenero.SelectedIndex = 0;
            txtFechaNac.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            ddlSede.SelectedIndex = 0;
            txtPassword.Text = "";
            ddlRol.SelectedIndex = 0;
            ddlEspecialidad.SelectedIndex = 0;
            txtNumColegiatura.Text = "";
            txtExperiencia.Text = "";
            ddlNivelAcceso.SelectedIndex = 0;
            txtPeso.Text = "";
            txtEdad.Text = "";
            pnlMensaje.Visible = false;
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string idUsuario = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                Response.Redirect($"~/vista/EditarUsuario.aspx?id={idUsuario}");
            }
            else if (e.CommandName == "Desactivar")
            {
                DesactivarUsuario(idUsuario);
            }
        }

        private void DesactivarUsuario(string idUsuario)
        {
            try
            {
                string idUsuarioActual = Session["UsuarioId"]?.ToString();
                if (idUsuario == idUsuarioActual)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", 
                        "alert('No puede desactivar su propia cuenta');", true);
                    return;
                }

                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_Admin_CambiarEstadoUsuario", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@NuevoEstado", 0); // 0 = Desactivado

                    cmd.ExecuteNonQuery();

                    string adminId = Session["AdminId"]?.ToString();
                    if (!string.IsNullOrEmpty(adminId))
                    {
                        AdministradoresDAO adminDAO = new AdministradoresDAO();
                        adminDAO.RegistrarAccion(adminId, "Desactivar Usuario", $"Usuario desactivado: {idUsuario}");
                    }
                }

                CargarEstadisticas();
                btnBuscar_Click(null, null);

                ClientScript.RegisterStartupScript(this.GetType(), "alert", 
                    "alert('Usuario desactivado exitosamente');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al desactivar usuario: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "alert", 
                    $"alert('Error al desactivar usuario: {ex.Message}');", true);
            }
        }

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/vista/Login.aspx");
        }

        // Método auxiliar para obtener la clase CSS del badge según el rol
        protected string GetRolClass(string rol)
        {
            switch (rol?.ToLower())
            {
                case "paciente":
                    return "paciente";
                case "médico":
                case "medico":
                    return "medico";
                case "administrador":
                    return "admin";
                default:
                    return "paciente";
            }
        }
    }
}
