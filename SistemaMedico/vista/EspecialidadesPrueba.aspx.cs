using System;
using System.Collections.Generic;
using SistemaMedico.datos;
using SistemaMedico.modelo;
using System.Web.UI.WebControls;

namespace SistemaMedico.vista
{
    public partial class EspecialidadesPrueba : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarEspecialidades(); 
            }
        }

        // =========================================================
        // Lógica de LECTURA (READ)
        // =========================================================
        private void CargarEspecialidades()
        {
            EspecialidadesDAO dao = new EspecialidadesDAO();
            try
            {
                // Llama al método DAO para obtener la lista
                List<Especialidades> lista = dao.ListarEspecialidades();

                if (lista.Count > 0)
                {
                    gvEspecialidades.DataSource = lista;
                    gvEspecialidades.DataBind();

                    lblEstado.Text = $"✅ Conexión OK. Se encontraron {lista.Count} especialidades.";
                    lblEstado.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblEstado.Text = "⚠️ Conexión OK, pero la tabla está vacía.";
                    lblEstado.ForeColor = System.Drawing.Color.OrangeRed;
                }
            }
            catch (Exception ex)
            {
                lblEstado.Text = $"❌ ERROR CRÍTICO DE LECTURA: {ex.Message}";
                lblEstado.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

    
        // Lógica de INSERCIÓN (CREATE)
        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            EspecialidadesDAO dao = new EspecialidadesDAO();
            Especialidades nuevaEspecialidad = new Especialidades();

            try
            {
                // 1. Validar y Asignar valores
                if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtDescripcion.Text))
                {
                    lblEstado.Text = "El nombre y la descripción no pueden estar vacíos.";
                    lblEstado.ForeColor = System.Drawing.Color.Red;
                    CargarEspecialidades();
                    return;
                }

                // Generar una ID temporal para que el constructor del modelo Especialidades se inicialice.
                nuevaEspecialidad.Id = "TEMP" + DateTime.Now.Ticks.ToString();
                nuevaEspecialidad.NomEsp = txtNombre.Text.Trim();
                nuevaEspecialidad.DesEsp = txtDescripcion.Text.Trim();

                int filasAfectadas = dao.AgregarEspecialidad(nuevaEspecialidad);

                if (filasAfectadas > 0)
                {
                    lblEstado.Text = $"¡Especialidad '{nuevaEspecialidad.NomEsp}' agregada con éxito! (¡CRUD OK!)";
                    lblEstado.ForeColor = System.Drawing.Color.Green;
                    txtNombre.Text = "";
                    txtDescripcion.Text = "";
                    CargarEspecialidades();
                }
                else
                {
                    lblEstado.Text = "Error: La base de datos no insertó la fila. (Revisa el SP 'sp_InsertarEspecialidad')";
                    lblEstado.ForeColor = System.Drawing.Color.Red;
                    CargarEspecialidades();
                }
            }
            catch (Exception ex)
            {
                lblEstado.Text = $"❌ ERROR CRÍTICO al Insertar: {ex.Message}";
                lblEstado.ForeColor = System.Drawing.Color.DarkRed;
            }
        }
    }
}
