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

                    if (dt.Rows.Count > 0)
                    {
                        rptEspecialidades.DataSource = dt;
                        rptEspecialidades.DataBind();
                        pnlEmptyEspecialidades.Visible = false;
                    }
                    else
                    {
                        rptEspecialidades.DataSource = null;
                        rptEspecialidades.DataBind();
                        pnlEmptyEspecialidades.Visible = true;
                    }

                    lblTotalEspecialidades.Text = dt.Rows.Count.ToString();
                    lblEspecialidadesActivas.Text = dt.Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar especialidades: {ex.Message}");
                rptEspecialidades.DataSource = null;
                rptEspecialidades.DataBind();
                pnlEmptyEspecialidades.Visible = true;
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
                // Verificar si estamos en modo edición
                bool modoEdicion = hfModoEdicionEsp.Value == "1";

                if (modoEdicion)
                {
                    ActualizarEspecialidad();
                }
                else
                {
                    CrearEspecialidad();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al procesar especialidad: {ex.Message}");
                MostrarMensajeEsp($"Error al procesar especialidad: {ex.Message}", "danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalEsp", "abrirModalEspecialidad();", true);
            }
        }

        // =====================================================
        // MÉTODO: CREAR ESPECIALIDAD (NUEVO)
        // =====================================================
        private void CrearEspecialidad()
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
                "setTimeout(function(){CerrarModalCrearEspecialidad(); }, 2000);", true);
        }

        // =====================================================
        // MÉTODO: ACTUALIZAR ESPECIALIDAD (NUEVO)
        // =====================================================
        private void ActualizarEspecialidad()
        {
            string idEspecialidad = hfIdEspecialidad.Value;

            using (SqlConnection conn = conexionDB.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_ActualizarEspecialidad", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", idEspecialidad);
                cmd.Parameters.AddWithValue("@Nombre", txtNombreEspecialidad.Text.Trim());
                cmd.Parameters.AddWithValue("@Descripcion", 
                    string.IsNullOrWhiteSpace(txtDescripcionEspecialidad.Text) ? (object)DBNull.Value : txtDescripcionEspecialidad.Text.Trim());

                cmd.ExecuteNonQuery();
            }

            string adminId = Session["AdminId"]?.ToString();
            if (!string.IsNullOrEmpty(adminId))
            {
                AdministradoresDAO adminDAO = new AdministradoresDAO();
                adminDAO.RegistrarAccion(adminId, "Actualizar Especialidad", 
                    $"Especialidad actualizada: {txtNombreEspecialidad.Text}");
            }

            LimpiarFormularioEspecialidad();
            CargarListaEspecialidades();
            CargarEspecialidades();

            MostrarMensajeEsp("Especialidad actualizada exitosamente", "success");
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModalEsp", 
                "setTimeout(function(){CerrarModalCrearEspecialidad(); }, 2000);", true);
        }

        protected void rptEspecialidades_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string idEspecialidad = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarEspecialidadParaEdicion(idEspecialidad);
            }
        }

        // =====================================================
        // MÉTODO: CARGAR ESPECIALIDAD PARA EDICIÓN
        // =====================================================
        private void CargarEspecialidadParaEdicion(string idEspecialidad)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== INICIANDO CARGA DE ESPECIALIDAD PARA EDICIÓN: {idEspecialidad} ===");

                // Establecer que estamos en el panel de especialidades
                hfPanelActivo.Value = "panel-especialidades";

                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerEspecialidadPorId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", idEspecialidad);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("Especialidad encontrada, cargando datos...");

                        // Configurar modo edición
                        hfIdEspecialidad.Value = idEspecialidad;
                        hfModoEdicionEsp.Value = "1";
                        lblTituloModalEspecialidad.Text = "Editar Especialidad";
                        btnCrearEspecialidad.Text = "Actualizar Especialidad";

                        // Cargar datos - Usar DES_ESP como está en Azure
                        txtNombreEspecialidad.Text = reader["NOM_ESP"].ToString();
                        txtDescripcionEspecialidad.Text = reader["DES_ESP"] != DBNull.Value 
                            ? reader["DES_ESP"].ToString() 
                            : "";

                        reader.Close();

                        System.Diagnostics.Debug.WriteLine("Datos cargados correctamente.");
                    }
                    else
                    {
                        reader.Close();
                        System.Diagnostics.Debug.WriteLine("ERROR: Especialidad no encontrada");
                        MostrarMensajeEsp("Especialidad no encontrada", "danger");
                        return;
                    }
                }

                // Abrir el modal con JavaScript - PASAR TRUE PARA NO LIMPIAR
                System.Diagnostics.Debug.WriteLine("Registrando script para abrir modal...");
                string script = @"
                    console.log('Ejecutando script para abrir modal de edición...');
                    setTimeout(function() {
                        console.log('Abriendo modal en modo edición...');
                        abrirModalEspecialidad(true);
                    }, 100);
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalEspEditar", script, true);

                System.Diagnostics.Debug.WriteLine("=== FIN DE CARGA DE ESPECIALIDAD PARA EDICIÓN ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN CARGA DE ESPECIALIDAD: {ex.Message}");
                MostrarMensajeEsp($"Error al cargar especialidad: {ex.Message}", "danger");
            }
        }

        private void MostrarMensajeEsp(string mensaje, string tipo)
        {
            lblMensajeEspecialidad.Text = $"<i class='fas fa-{(tipo == "success" ? "check-circle" : "exclamation-circle")}'></i> {mensaje}";
            pnlMensajeEspecialidad.CssClass = $"alert alert-{tipo} show";
            pnlMensajeEspecialidad.Visible = true;
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            lblMensaje.Text = $"<i class='fas fa-{(tipo == "success" ? "check-circle" : "exclamation-circle")}'></i> {mensaje}";
            pnlMensaje.CssClass = $"alert alert-{tipo} show";
            pnlMensaje.Visible = true;
        }

        private void LimpiarFormularioEspecialidad()
        {
            // Resetear modo edición
            hfModoEdicionEsp.Value = "0";
            hfIdEspecialidad.Value = "";
            lblTituloModalEspecialidad.Text = "Nueva Especialidad Médica";
            btnCrearEspecialidad.Text = "Crear Especialidad";

            // Limpiar campos
            txtNombreEspecialidad.Text = "";
            txtDescripcionEspecialidad.Text = "";
            pnlMensajeEspecialidad.Visible = false;
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

                    if (dt.Rows.Count > 0)
                    {
                        rptSedes.DataSource = dt;
                        rptSedes.DataBind();
                        pnlEmptySedes.Visible = false;
                    }
                    else
                    {
                        rptSedes.DataSource = null;
                        rptSedes.DataBind();
                        pnlEmptySedes.Visible = true;
                    }

                    lblTotalSedes.Text = dt.Rows.Count.ToString();
                    lblSedesActivas.Text = dt.Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar sedes: {ex.Message}");
                rptSedes.DataSource = null;
                rptSedes.DataBind();
                pnlEmptySedes.Visible = true;
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
                // Verificar si estamos en modo edición
                bool modoEdicion = hfModoEdicionSede.Value == "1";

                if (modoEdicion)
                {
                    ActualizarSede();
                }
                else
                {
                    CrearSede();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al procesar sede: {ex.Message}");
                MostrarMensajeSede($"Error al procesar sede: {ex.Message}", "danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalSede", "abrirModalSede();", true);
            }
        }

        // =====================================================
        // MÉTODO: CREAR SEDE (NUEVO)
        // =====================================================
        private void CrearSede()
        {
            using (SqlConnection conn = conexionDB.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarSede", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", txtNombreSede.Text.Trim());
                cmd.Parameters.AddWithValue("@Direccion", 
                    string.IsNullOrWhiteSpace(txtDireccionSede.Text) ? (object)DBNull.Value : txtDireccionSede.Text.Trim());
                cmd.Parameters.AddWithValue("@Telefono", DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", DBNull.Value);

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

            MostrarMensajeSede("Sede creada exitosamente", "success");
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSede", 
                "setTimeout(function(){ cerrarModalSede(); }, 2000);", true);
        }

        // =====================================================
        // MÉTODO: ACTUALIZAR SEDE (NUEVO)
        // =====================================================
        private void ActualizarSede()
        {
            string idSede = hfIdSede.Value;

            Sedes sede = new Sedes
            {
                Id = idSede,
                NomSede = txtNombreSede.Text.Trim(),
                DirSede = txtDireccionSede.Text.Trim()
            };

            sedesDAO.Actualizar(sede);

            string adminId = Session["AdminId"]?.ToString();
            if (!string.IsNullOrEmpty(adminId))
            {
                AdministradoresDAO adminDAO = new AdministradoresDAO();
                adminDAO.RegistrarAccion(adminId, "Actualizar Sede", 
                    $"Sede actualizada: {txtNombreSede.Text}");
            }

            LimpiarFormularioSede();
            CargarListaSedes();

            MostrarMensajeSede("Sede actualizada exitosamente", "success");
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModalSede", 
                "setTimeout(function(){ cerrarModalSede(); }, 2000);", true);
        }

        // =====================================================
        // MÉTODO: LIMPIAR FORMULARIO SEDE (ACTUALIZADO)
        // =====================================================
        private void LimpiarFormularioSede()
        {
            // Resetear modo edición
            hfModoEdicionSede.Value = "0";
            hfIdSede.Value = "";
            lblTituloModalSede.Text = "Nueva Sede";
            btnCrearSede.Text = "Crear Sede";

            // Limpiar campos
            txtNombreSede.Text = "";
            txtDireccionSede.Text = "";
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
            // Verificar si estamos en modo edición
            bool modoEdicion = hfModoEdicion.Value == "1";

            if (!ValidarFormulario(modoEdicion))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "abrirModalCrearUsuario();", true);
                return;
            }

            try
            {
                if (modoEdicion)
                {
                    ActualizarUsuario();
                }
                else
                {
                    CrearUsuario();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al procesar usuario: {ex.Message}");
                MostrarMensaje($"Error al procesar usuario: {ex.Message}", "danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "abrirModalCrearUsuario();", true);
            }
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string idUsuario = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarUsuarioParaEdicion(idUsuario);
            }
            else if (e.CommandName == "Desactivar")
            {
                DesactivarUsuario(idUsuario);
            }
            else if (e.CommandName == "Activar")
            {
                ActivarUsuario(idUsuario);
            }
        }

        // =====================================================
        // NUEVO MÉTODO: CARGAR USUARIO PARA EDICIÓN
        // =====================================================
        private void CargarUsuarioParaEdicion(string idUsuario)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== INICIANDO CARGA DE USUARIO PARA EDICIÓN: {idUsuario} ===");
                
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    // Obtener datos del usuario
                    SqlCommand cmd = new SqlCommand("sp_ObtenerUsuarioPorId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", idUsuario);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("Usuario encontrado, cargando datos...");
                        
                        // Configurar modo edición
                        hfModoEdicion.Value = "1";
                        hfIdUsuario.Value = idUsuario;
                        lblTituloModalUsuario.Text = "Editar Usuario";
                        btnCrearUsuario.Text = "Actualizar Usuario";

                        // Cargar datos básicos
                        txtNombre.Text = reader["NOM"].ToString();
                        txtApellido.Text = reader["APE"].ToString();
                        
                        string tipoDoc = reader["TIPO_DOC"] != DBNull.Value ? reader["TIPO_DOC"].ToString() : "";
                        if (!string.IsNullOrEmpty(tipoDoc) && ddlTipoDoc.Items.FindByValue(tipoDoc) != null)
                        {
                            ddlTipoDoc.SelectedValue = tipoDoc;
                        }
                        
                        txtNumDoc.Text = reader["NUM_DOC"] != DBNull.Value ? reader["NUM_DOC"].ToString() : "";
                        
                        string genero = reader["GEN"] != DBNull.Value ? reader["GEN"].ToString() : "";
                        if (!string.IsNullOrEmpty(genero) && ddlGenero.Items.FindByValue(genero) != null)
                        {
                            ddlGenero.SelectedValue = genero;
                        }
                        
                        if (reader["FEC_NAC"] != DBNull.Value)
                        {
                            DateTime fechaNac = Convert.ToDateTime(reader["FEC_NAC"]);
                            txtFechaNac.Text = fechaNac.ToString("yyyy-MM-dd");
                        }

                        txtEmail.Text = reader["EMAIL"].ToString();
                        txtTelefono.Text = reader["TEL"] != DBNull.Value ? reader["TEL"].ToString() : "";

                        // Sede preferida
                        if (reader["SEDE_PREF"] != DBNull.Value)
                        {
                            string sedePref = reader["SEDE_PREF"].ToString();
                            if (ddlSede.Items.FindByValue(sedePref) != null)
                            {
                                ddlSede.SelectedValue = sedePref;
                            }
                        }

                        // Rol
                        string idRol = reader["ID_ROL"].ToString();
                        if (ddlRol.Items.FindByValue(idRol) != null)
                        {
                            ddlRol.SelectedValue = idRol;
                        }

                        // Contraseña opcional en modo edición
                        txtPassword.Text = "";
                        lblPasswordOptional.Visible = true;
                        spanPasswordRequired.Visible = false;

                        reader.Close();

                        System.Diagnostics.Debug.WriteLine($"Datos básicos cargados. Rol: {idRol}");

                        // Cargar datos específicos según el rol
                        CargarDatosEspecificosRol(idUsuario, idRol);
                        
                        System.Diagnostics.Debug.WriteLine("Datos específicos cargados. Preparando para abrir modal...");
                    }
                    else
                    {
                        reader.Close();
                        System.Diagnostics.Debug.WriteLine("ERROR: Usuario no encontrado en la base de datos");
                        MostrarMensaje("Usuario no encontrado", "danger");
                        return;
                    }
                }

                // Abrir el modal con JavaScript - PASAR TRUE PARA NO LIMPIAR
                System.Diagnostics.Debug.WriteLine("Registrando script para abrir modal...");
                string script = @"
                    console.log('Ejecutando script para abrir modal de edición...');
                    setTimeout(function() {
                        console.log('Abriendo modal en modo edición...');
                        abrirModalCrearUsuario(true);
                    }, 100);
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalEditar", script, true);
                
                System.Diagnostics.Debug.WriteLine("=== FIN DE CARGA DE USUARIO PARA EDICIÓN ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN CARGA DE USUARIO: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                MostrarMensaje($"Error al cargar usuario: {ex.Message}", "danger");
            }
        }

        // =====================================================
        // NUEVO MÉTODO: CARGAR DATOS ESPECÍFICOS POR ROL
        // =====================================================
        private void CargarDatosEspecificosRol(string idUsuario, string idRol)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    if (idRol == "R0000002") // Médico
                    {
                        SqlCommand cmd = new SqlCommand("sp_ObtenerDoctorPorIdUsuario", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            // Verificar si las columnas existen antes de usarlas
                            if (HasColumn(reader, "ID_ESP") && reader["ID_ESP"] != DBNull.Value)
                            {
                                string idEsp = reader["ID_ESP"].ToString();
                                if (ddlEspecialidad.Items.FindByValue(idEsp) != null)
                                {
                                    ddlEspecialidad.SelectedValue = idEsp;
                                }
                            }
                            
                            if (HasColumn(reader, "NUM_COLEGIATURA") && reader["NUM_COLEGIATURA"] != DBNull.Value)
                            {
                                txtNumColegiatura.Text = reader["NUM_COLEGIATURA"].ToString();
                            }
                            
                            if (HasColumn(reader, "ANIOS_EXP") && reader["ANIOS_EXP"] != DBNull.Value)
                            {
                                txtExperiencia.Text = reader["ANIOS_EXP"].ToString();
                            }
                        }
                        reader.Close();

                        // Mostrar panel de médico con JavaScript
                        ScriptManager.RegisterStartupScript(this, GetType(), "mostrarMedico",
                            "document.getElementById('" + pnlMedico.ClientID + "').classList.add('show');", true);
                    }
                    else if (idRol == "R0000001") // Administrador
                    {
                        SqlCommand cmd = new SqlCommand("sp_ObtenerAdministradorPorIdUsuario", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            if (HasColumn(reader, "NIVEL_ACCESO") && reader["NIVEL_ACCESO"] != DBNull.Value)
                            {
                                int nivelAcceso = Convert.ToInt32(reader["NIVEL_ACCESO"]);
                                ddlNivelAcceso.SelectedValue = nivelAcceso.ToString();
                            }
                        }
                        reader.Close();

                        // Mostrar panel de administrador con JavaScript
                        ScriptManager.RegisterStartupScript(this, GetType(), "mostrarAdmin",
                            "document.getElementById('" + pnlAdministrador.ClientID + "').classList.add('show');", true);
                    }
                    else if (idRol == "R0000003") // Paciente
                    {
                        SqlCommand cmd = new SqlCommand("sp_ObtenerPacientePorIdUsuario", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            if (HasColumn(reader, "PESO") && reader["PESO"] != DBNull.Value)
                            {
                                txtPeso.Text = reader["PESO"].ToString();
                            }
                            // Nota: EDAD en tu BD es tipo DATE, aquí puedes manejarlo como necesites
                        }
                        reader.Close();

                        // Mostrar panel de paciente con JavaScript
                        ScriptManager.RegisterStartupScript(this, GetType(), "mostrarPaciente",
                            "document.getElementById('" + pnlPaciente.ClientID + "').classList.add('show');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos específicos: {ex.Message}");
                // No lanzar excepción para que el modal se abra aunque falten datos específicos
            }
        }

        // =====================================================
        // MÉTODO AUXILIAR: VERIFICAR SI EXISTE UNA COLUMNA EN EL DATAREADER
        // =====================================================
        private bool HasColumn(SqlDataReader reader, string columnName)
        {
            try
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
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

        // =====================================================
        // MÉTODO: ACTIVAR USUARIO
        // =====================================================
        private void ActivarUsuario(string idUsuario)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_Admin_CambiarEstadoUsuario", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@NuevoEstado", 1); // 1 = Activo

                    cmd.ExecuteNonQuery();

                    string adminId = Session["AdminId"]?.ToString();
                    if (!string.IsNullOrEmpty(adminId))
                    {
                        AdministradoresDAO adminDAO = new AdministradoresDAO();
                        adminDAO.RegistrarAccion(adminId, "Activar Usuario", $"Usuario activado: {idUsuario}");
                    }
                }

                CargarEstadisticas();
                btnBuscar_Click(null, null);

                ClientScript.RegisterStartupScript(this.GetType(), "alert", 
                    "alert('Usuario activado exitosamente');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al activar usuario: {ex.Message}");
                ClientScript.RegisterStartupScript(this.GetType(), "alert", 
                    $"alert('Error al activar usuario: {ex.Message}');", true);
            }
        }

        // =====================================================
        // NUEVO MÉTODO: CREAR USUARIO
        // =====================================================
        private void CrearUsuario()
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

        // =====================================================
        // NUEVO MÉTODO: ACTUALIZAR USUARIO
        // =====================================================
        private void ActualizarUsuario()
        {
            string idUsuario = hfIdUsuario.Value;

            using (SqlConnection conn = conexionDB.ObtenerConexion())
            {
                // 1. Actualizar tabla Usuarios
                SqlCommand cmd = new SqlCommand("sp_ActualizarUsuario", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Id", idUsuario);
                cmd.Parameters.AddWithValue("@IdRol", ddlRol.SelectedValue);
                cmd.Parameters.AddWithValue("@TipoDoc", ddlTipoDoc.SelectedValue);
                cmd.Parameters.AddWithValue("@NumDoc", txtNumDoc.Text.Trim());
                cmd.Parameters.AddWithValue("@Apellido", txtApellido.Text.Trim());
                cmd.Parameters.AddWithValue("@Nombre", txtNombre.Text.Trim());
                cmd.Parameters.AddWithValue("@Genero", ddlGenero.SelectedValue);
                cmd.Parameters.AddWithValue("@FechaNac", Convert.ToDateTime(txtFechaNac.Text));
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Telefono", txtTelefono.Text.Trim());
                cmd.Parameters.AddWithValue("@SedePref",
                    string.IsNullOrEmpty(ddlSede.SelectedValue) ? (object)DBNull.Value : ddlSede.SelectedValue);

                cmd.ExecuteNonQuery();

                // 2. Si se proporcionó nueva contraseña, actualizarla
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    UsuariosDAO usuarioDAO = new UsuariosDAO();
                    byte[] passwordHash = usuarioDAO.HashPassword(txtPassword.Text);

                    SqlCommand cmdPass = new SqlCommand(
                        "UPDATE Usuarios SET PSWDHASH = @PasswordHash WHERE ID = @Id", conn);
                    cmdPass.Parameters.AddWithValue("@Id", idUsuario);
                    cmdPass.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmdPass.ExecuteNonQuery();
                }

                // 3. Actualizar datos específicos según el rol
                string rolSeleccionado = ddlRol.SelectedValue;

                if (rolSeleccionado == "R0000002") // Médico
                {
                    SqlCommand cmdDoctor = new SqlCommand("sp_ActualizarDoctor", conn);
                    cmdDoctor.CommandType = CommandType.StoredProcedure;

                    // Obtener ID del doctor
                    SqlCommand cmdGetDoctor = new SqlCommand(
                        "SELECT ID FROM Doctores WHERE ID_USU = @IdUsuario", conn);
                    cmdGetDoctor.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    string idDoctor = cmdGetDoctor.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(idDoctor))
                    {
                        cmdDoctor.Parameters.AddWithValue("@Id", idDoctor);
                        cmdDoctor.Parameters.AddWithValue("@IdEspecialidad", ddlEspecialidad.SelectedValue);
                        cmdDoctor.Parameters.AddWithValue("@NumColegiatura", txtNumColegiatura.Text.Trim());
                        cmdDoctor.Parameters.AddWithValue("@AniosExperiencia", Convert.ToInt32(txtExperiencia.Text));
                        cmdDoctor.ExecuteNonQuery();
                    }
                }
                else if (rolSeleccionado == "R0000001") // Administrador
                {
                    SqlCommand cmdAdmin = new SqlCommand("sp_ActualizarAdministrador", conn);
                    cmdAdmin.CommandType = CommandType.StoredProcedure;

                    // Obtener ID del administrador
                    SqlCommand cmdGetAdmin = new SqlCommand(
                        "SELECT ID FROM Administradores WHERE ID_USU = @IdUsuario", conn);
                    cmdGetAdmin.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    string idAdmin = cmdGetAdmin.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(idAdmin))
                    {
                        cmdAdmin.Parameters.AddWithValue("@Id", idAdmin);
                        cmdAdmin.Parameters.AddWithValue("@NivelAcceso", Convert.ToInt32(ddlNivelAcceso.SelectedValue));
                        cmdAdmin.Parameters.AddWithValue("@PermisosEspeciales", DBNull.Value);
                        cmdAdmin.ExecuteNonQuery();
                    }
                }
                else if (rolSeleccionado == "R0000003") // Paciente
                {
                    SqlCommand cmdPaciente = new SqlCommand("sp_ActualizarPaciente", conn);
                    cmdPaciente.CommandType = CommandType.StoredProcedure;

                    // Obtener ID del paciente
                    SqlCommand cmdGetPaciente = new SqlCommand(
                        "SELECT ID FROM Pacientes WHERE ID_USU = @IdUsuario", conn);
                    cmdGetPaciente.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    string idPaciente = cmdGetPaciente.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(idPaciente))
                    {
                        cmdPaciente.Parameters.AddWithValue("@Id", idPaciente);
                        cmdPaciente.Parameters.AddWithValue("@Peso",
                            string.IsNullOrWhiteSpace(txtPeso.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtPeso.Text));
                        cmdPaciente.Parameters.AddWithValue("@Edad", DBNull.Value); // Ajustar según tu lógica
                        cmdPaciente.ExecuteNonQuery();
                    }
                }
            }

            string adminIdSession = Session["AdminId"]?.ToString();
            if (!string.IsNullOrEmpty(adminIdSession))
            {
                AdministradoresDAO adminDAO = new AdministradoresDAO();
                adminDAO.RegistrarAccion(adminIdSession, "Actualizar Usuario",
                    $"Usuario actualizado: {txtNombre.Text} {txtApellido.Text}");
            }

            LimpiarFormularioModal();
            CargarEstadisticas();
            CargarUsuarios(null, null);

            MostrarMensaje("Usuario actualizado exitosamente", "success");
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModal",
                "setTimeout(function(){ cerrarModalCrearUsuario(); }, 2000);", true);
        }

        private void LimpiarFormularioModal()
        {
            // Resetear modo edición
            hfModoEdicion.Value = "0";
            hfIdUsuario.Value = "";
            lblTituloModalUsuario.Text = "Crear Nuevo Usuario";
            btnCrearUsuario.Text = "Crear Usuario";
            lblPasswordOptional.Visible = false;
            spanPasswordRequired.Visible = true;

            // Limpiar campos
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

        private bool ValidarFormulario(bool modoEdicion = false)
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

            // Validar contraseña solo en modo creación o si se proporciona en edición
            if (!modoEdicion)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text) || txtPassword.Text.Length < 6)
                {
                    MostrarMensaje("La contraseña debe tener al menos 6 caracteres", "danger");
                    return false;
                }
            }
            else
            {
                // En modo edición, validar solo si se ingresó una contraseña
                if (!string.IsNullOrWhiteSpace(txtPassword.Text) && txtPassword.Text.Length < 6)
                {
                    MostrarMensaje("Si proporciona una nueva contraseña, debe tener al menos 6 caracteres", "danger");
                    return false;
                }
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

        protected void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/vista/Login.aspx");
        }

        // Método auxiliar para obtener la clase CSS del badge según el rol
        protected string GetRolClass(string rol)
        {
            if (string.IsNullOrEmpty(rol))
                return "paciente";

            switch (rol.ToLower().Trim())
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

        // =====================================================
        // MÉTODO: MOSTRAR MENSAJE EN MODAL DE SEDE
        // =====================================================
        private void MostrarMensajeSede(string mensaje, string tipo)
        {
            lblMensajeSede.Text = mensaje;
            pnlMensajeSede.Visible = true;
            pnlMensajeSede.CssClass = tipo == "success" ? "alert alert-success show" : "alert alert-danger show";

            // Mantener el modal abierto
            ScriptManager.RegisterStartupScript(this, GetType(), "showModalSede", "abrirModalSede();", true);
        }

        protected void rptSedes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string idSede = e.CommandArgument.ToString();

            if (e.CommandName == "Editar")
            {
                CargarSedeParaEdicion(idSede);
            }
        }

        // =====================================================
        // MÉTODO: CARGAR SEDE PARA EDICIÓN
        // =====================================================
        private void CargarSedeParaEdicion(string idSede)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== INICIANDO CARGA DE SEDE PARA EDICIÓN: {idSede} ===");

                // Establecer que estamos en el panel de sedes
                hfPanelActivo.Value = "panel-sedes";

                Sedes sede = sedesDAO.ObtenerPorId(idSede);

                if (sede != null)
                {
                    System.Diagnostics.Debug.WriteLine("Sede encontrada, cargando datos...");

                    // Configurar modo edición
                    hfIdSede.Value = idSede;
                    hfModoEdicionSede.Value = "1";
                    lblTituloModalSede.Text = "Editar Sede";
                    btnCrearSede.Text = "Actualizar Sede";

                    // Cargar datos
                    txtNombreSede.Text = sede.NomSede;
                    txtDireccionSede.Text = sede.DirSede ?? "";

                    System.Diagnostics.Debug.WriteLine("Datos cargados correctamente.");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Sede no encontrada");
                    MostrarMensajeSede("Sede no encontrada", "danger");
                    return;
                }

                // Abrir el modal con JavaScript - PASAR TRUE PARA NO LIMPIAR
                System.Diagnostics.Debug.WriteLine("Registrando script para abrir modal...");
                string script = @"
                    console.log('Ejecutando script para abrir modal de edición de sede...');
                    setTimeout(function() {
                        console.log('Abriendo modal de sede en modo edición...');
                        abrirModalSede(true);
                    }, 100);
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "showModalSedeEditar", script, true);

                System.Diagnostics.Debug.WriteLine("=== FIN DE CARGA DE SEDE PARA EDICIÓN ===");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN CARGA DE SEDE: {ex.Message}");
                MostrarMensajeSede($"Error al cargar sede: {ex.Message}", "danger");
            }
        }
    }
}
