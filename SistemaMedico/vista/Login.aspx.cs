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
    public partial class Login : System.Web.UI.Page
    {
        private UsuariosDAO usuariosDAO;
        private PacientesDAO pacientesDAO;
        private DoctoresDAO doctoresDAO;
        private AdministradoresDAO administradoresDAO;

        public Login()
        {
            usuariosDAO = new UsuariosDAO();
            pacientesDAO = new PacientesDAO();
            doctoresDAO = new DoctoresDAO();
            administradoresDAO = new AdministradoresDAO();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioId"] != null)
                {
                    RedirigirSegunRol();
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MostrarError("Por favor, ingrese su correo y contraseña.");
                return;
            }

            try
            {
                Usuarios usuario = null;
                bool credencialesValidas = usuariosDAO.ValidarCredenciales(email, password, out usuario);

                if (credencialesValidas && usuario != null)
                {
                    // Guardar datos básicos en sesión
                    Session["UsuarioId"] = usuario.Id;
                    Session["UsuarioNombreCompleto"] = usuario.Nom + " " + usuario.Ape;
                    Session["UsuarioEmail"] = usuario.Email;
                    Session["IdRol"] = usuario.IdRol;
                    Session["UsuarioDNI"] = usuario.NumDoc;
                    Session["UsuarioTelefono"] = usuario.Tel;
                    Session["UsuarioFechaNac"] = usuario.FecNac;

                    string nombreRol = ObtenerNombreRol(usuario.IdRol);
                    Session["UsuarioRol"] = nombreRol;

                    System.Diagnostics.Debug.WriteLine($"🔐 Login exitoso: Email={usuario.Email}, IdRol={usuario.IdRol}, NombreRol={nombreRol}");

                    // ✅ PROCESAMIENTO SEGÚN ROL
                    if (nombreRol == "Administrador")
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine($"🔍 Buscando administrador con ID_USU: {usuario.Id}");

                            Administradores admin = administradoresDAO.ObtenerPorIdUsuario(usuario.Id);

                            if (admin != null)
                            {
                                Session["AdminId"] = admin.Id;
                                Session["NivelAcceso"] = admin.NivelAcceso;

                                System.Diagnostics.Debug.WriteLine($"✅ Administrador encontrado: ID={admin.Id}, NivelAcceso={admin.NivelAcceso}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("❌ No se encontró administrador en la BD");
                                MostrarError("Error: No se encontró el registro de administrador asociado a esta cuenta.");
                                Session.Clear();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"💥 Error al obtener administrador: {ex.Message}");
                            MostrarError($"Error al obtener información del administrador: {ex.Message}");
                            Session.Clear();
                            return;
                        }
                    }
                    else if (nombreRol == "Médico")
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine($"🔍 Buscando médico con ID_USU: {usuario.Id}");

                            Doctores doctor = doctoresDAO.ObtenerDoctorPorIdUsuario(usuario.Id);

                            if (doctor != null)
                            {
                                Session["DoctorId"] = doctor.Id;
                                System.Diagnostics.Debug.WriteLine($"✅ Médico encontrado: ID={doctor.Id}, Nombre={doctor.Nom} {doctor.Ape}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("❌ No se encontró médico en la BD");
                                MostrarError("Error: No se encontró el registro de médico asociado a esta cuenta.");
                                Session.Clear();
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"💥 Error al obtener médico: {ex.Message}");
                            MostrarError($"Error al obtener información del médico: {ex.Message}");
                            Session.Clear();
                            return;
                        }
                    }
                    else if (nombreRol == "Paciente")
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine($"🔍 Buscando paciente con ID_USU: {usuario.Id}");

                            Pacientes paciente = pacientesDAO.ObtenerPorIdUsuario(usuario.Id);

                            if (paciente != null)
                            {
                                Session["PacienteId"] = paciente.Id;
                                System.Diagnostics.Debug.WriteLine($"✅ Paciente encontrado: ID={paciente.Id}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("⚠️ No se encontró paciente en la BD - continuando...");
                                // No es crítico, el paciente puede no tener registro aún
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"⚠️ Error al obtener paciente (no crítico): {ex.Message}");
                            // No bloqueamos el login
                        }
                    }

                    // ✅ REDIRIGIR (FUERA DE LOS BLOQUES IF/ELSE)
                    System.Diagnostics.Debug.WriteLine($"🔀 Redirigiendo según rol: {nombreRol}");
                    RedirigirSegunRol();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Credenciales inválidas para: {email}");
                    MostrarError("Correo o contraseña incorrectos. Por favor, intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 Error general en login: {ex.Message}\n{ex.StackTrace}");
                MostrarError("Error al iniciar sesión: " + ex.Message);
            }
        }

        // ✅ CORRECTO SEGÚN TU BASE DE DATOS
        private string ObtenerNombreRol(string idRol)
        {
            if (idRol.StartsWith("R"))
            {
                if (idRol == "R0000001") return "Administrador"; // ✅
                if (idRol == "R0000002") return "Médico";        // ✅
                if (idRol == "R0000003") return "Paciente";      // ✅
            }
            return "Paciente"; // Por defecto
        }

        private void RedirigirSegunRol()
        {
            string rol = Session["UsuarioRol"]?.ToString().Trim().ToLower();

            System.Diagnostics.Debug.WriteLine($"📍 RedirigirSegunRol - Rol en sesión: {rol}");

            switch (rol)
            {
                case "paciente":
                    System.Diagnostics.Debug.WriteLine("➡️ Redirigiendo a PanelPaciente.aspx");
                    Response.Redirect("~/vista/PanelPaciente.aspx");
                    break;

                case "médico":
                    System.Diagnostics.Debug.WriteLine("➡️ Redirigiendo a PanelMedico.aspx");
                    Response.Redirect("~/vista/PanelMedico.aspx");
                    break;

                case "administrador":
                    System.Diagnostics.Debug.WriteLine("➡️ Redirigiendo a PanelAdministrador.aspx");
                    Response.Redirect("~/vista/PanelAdministrador.aspx");
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine($"❌ Rol no reconocido: {rol}");
                    Session.Clear();
                    MostrarError("No se pudo determinar el tipo de usuario.");
                    break;
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