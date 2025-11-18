using SistemaMedico.conexion; // Tu clase de conexión
using SistemaMedico.modelo; // Tus clases de entidad
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SistemaMedico.datos
{

    // =========================================================================
    // CLASES "VIEWMODEL" PARA MANEJAR DATOS DE TABLAS UNIDAS (JOINS)
    // (Las usamos porque tu Citas.cs solo tiene IDs)
    // =========================================================================

    // Usada para el Dashboard y "Mi Horario"
    public class CitaInfo
    {
        public string IdCita { get; set; }
        public DateTime HoraCompleta { get; set; } 
        public string NombresPaciente { get; set; }
        public string ApellidosPaciente { get; set; }
        public string ESTADO { get; set; } // <--- ¡AÑADE ESTA LÍNEA!
    }

    // Usada para la búsqueda de Pacientes
    public class PacienteInfo
    {
        public string IdPaciente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Documento { get; set; }
        public string Email { get; set; }
    }

    // Usada para el Historial Clínico
    public class HistorialInfo
    {
        public string IdCita { get; set; }
        public DateTime HoraCompleta { get; set; } // <--- SOLUCIÓN
        public string ESTADO { get; set; }
        public string Especialidad { get; set; }
        public string Doctor { get; set; }
        public string Diagnostico { get; set; }
    }

    public class DetalleCitaDTO
    {
        // Info Cita
        public string IdCita { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Motivo { get; set; }
        public string Estado { get; set; }

        // Info Paciente
        public string PacienteNombre { get; set; }
        public string PacienteDNI { get; set; }
        public string PacienteEmail { get; set; }
        public string PacienteTelefono { get; set; }
        public int? PacienteEdad { get; set; } // Calculado o directo
        public decimal? PacientePeso { get; set; }

        // Info Doctor/Sede
        public string DoctorNombre { get; set; }
        public string Especialidad { get; set; }
        public string Sede { get; set; }

        // Info Consulta (Puede ser null si no se ha hecho)
        public string Sintomas { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }
        public string Observaciones { get; set; }
    }
    // (La clase para "Ver Ficha" es muy grande, la podemos crear cuando la usemos)

    // =========================================================================
    // CLASE PRINCIPAL: CitasDAO
    // =========================================================================
    public class CitasDAO
    {
        private ConexionDB con = new ConexionDB();

        public SqlConnection Conectar()
        {
            // Antes decía: return con.AbrirConexion();
            return con.ObtenerConexion(); // <-- ¡Ajustado a tu método!
        }
       

        // --- 1. DASHBOARD: Listar Citas del Día ---
        public List<CitaInfo> ListarCitasDelDia(string idDoctor, DateTime fecha)
        {
            var lista = new List<CitaInfo>();
            SqlDataReader dr = null;
            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("sp_PanelMedico_ListarCitasDia", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_DOC", idDoctor);
                    cmd.Parameters.AddWithValue("@FECHA", fecha.Date);

                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        // --- INICIO DE LA CORRECCIÓN ---
                        var oCitaInfo = new CitaInfo();
                        oCitaInfo.IdCita = dr["IdCita"].ToString();

                        // 1. Obtenemos el TimeSpan (ej: 09:30:00)
                        TimeSpan horaCita = (TimeSpan)dr["HORA"];
                        // 2. Combinamos la fecha del día (del parámetro) con la hora
                        oCitaInfo.HoraCompleta = fecha.Date + horaCita; // <--- SOLUCIÓN

                        oCitaInfo.ESTADO = dr["ESTADO"].ToString();
                        oCitaInfo.NombresPaciente = dr["NombresPaciente"].ToString();
                        oCitaInfo.ApellidosPaciente = dr["ApellidosPaciente"].ToString();
                        lista.Add(oCitaInfo);
                        // --- FIN DE LA CORRECCIÓN ---
                    }
                }
            }
            return lista;
        }

        // --- 2. DASHBOARD: Registrar Nueva Cita ---
        public string RegistrarCita(Citas oCita) // Usa la clase de tu modelo
        {
            string nuevoId = "";
            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("sp_PanelMedico_RegistrarCita", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_PAC", oCita.IdPac);
                    cmd.Parameters.AddWithValue("@ID_DOC", oCita.IdDoc);
                    cmd.Parameters.AddWithValue("@ID_SEDE", oCita.IdSede);
                    cmd.Parameters.AddWithValue("@ID_ESP", oCita.IdEsp);
                    cmd.Parameters.AddWithValue("@FECHA", oCita.Fecha);
                    cmd.Parameters.AddWithValue("@HORA", oCita.Hora);
                    cmd.Parameters.AddWithValue("@MOTIVO", (object)oCita.Motivo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TIPO_PAGO", (object)oCita.TipoPago ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MONTO", (object)oCita.Monto ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PAGO_REALI", oCita.PagoReali);

                    // Capturamos el ID devuelto por el SP
                    nuevoId = cmd.ExecuteScalar().ToString();
                }
            }
            return nuevoId;
        }

        // --- 3. DASHBOARD: Iniciar Consulta ---
        public string IniciarConsulta(string idCita)
        {
            string nuevaConsultaId = "";
            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("sp_PanelMedico_IniciarConsulta", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_CITA", idCita);

                    // Capturamos el ID de consulta devuelto por el SP
                    nuevaConsultaId = cmd.ExecuteScalar().ToString();
                }
            }
            return nuevaConsultaId;
        }

        // --- 4. DASHBOARD: Finalizar Consulta ---
        public void FinalizarConsulta(ConsultasMedicas oConsulta) // Usa la clase de tu modelo
        {
            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("sp_PanelMedico_FinalizarConsulta", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_CONSULTA", oConsulta.Id);
                    cmd.Parameters.AddWithValue("@ID_CITA", oConsulta.IdCita);
                    cmd.Parameters.AddWithValue("@SINTOMAS", oConsulta.Sintomas);
                    cmd.Parameters.AddWithValue("@DIAGNOSTICO", oConsulta.Diagnostico);
                    cmd.Parameters.AddWithValue("@TRATAMIENTO", oConsulta.Tratamiento);
                    cmd.Parameters.AddWithValue("@OBSERVACIONES", oConsulta.Observaciones);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- 5. MI HORARIO: Listar Citas de la Semana ---
        public List<CitaInfo> ListarCitasSemana(string idDoctor, DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<CitaInfo>();
            SqlDataReader dr = null;
            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("sp_PanelMedico_ListarCitasSemana", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_DOC", idDoctor);
                    cmd.Parameters.AddWithValue("@FechaInicioSemana", fechaInicio.Date);
                    cmd.Parameters.AddWithValue("@FechaFinSemana", fechaFin.Date);

                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        // --- INICIO DE LA CORRECCIÓN ---
                        var oCitaInfo = new CitaInfo();
                        oCitaInfo.IdCita = dr["IdCita"].ToString();

                        // 1. Obtenemos el TimeSpan (ej: 09:30:00)
                        TimeSpan horaCita = (TimeSpan)dr["HORA"];
                        // 2. Obtenemos la FECHA (de la base de datos)
                        DateTime fechaCita = (DateTime)dr["FECHA"];
                        // 3. Combinamos la fecha con la hora
                        oCitaInfo.HoraCompleta = fechaCita.Date + horaCita; // <--- SOLUCIÓN

                        oCitaInfo.ESTADO = dr["ESTADO"].ToString();
                        oCitaInfo.NombresPaciente = dr["NombresPaciente"].ToString();
                        oCitaInfo.ApellidosPaciente = dr["ApellidosPaciente"].ToString();
                        lista.Add(oCitaInfo);
                        // --- FIN DE LA CORRECCIÓN ---
                    }
                }
            }
            return lista;
        }

        // --- 6. PACIENTES: Buscar Pacientes ---
        public List<PacienteInfo> BuscarPacientes(string termino, string tipoBusqueda)
        {
            var lista = new List<PacienteInfo>();
            SqlDataReader dr = null;
            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("sp_PanelMedico_BuscarPacientes", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TerminoBusqueda", termino);
                    cmd.Parameters.AddWithValue("@TipoBusqueda", tipoBusqueda);

                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var oPaciente = new PacienteInfo();
                        oPaciente.IdPaciente = dr["IdPaciente"].ToString();
                        oPaciente.Nombres = dr["Nombres"].ToString();
                        oPaciente.Apellidos = dr["Apellidos"].ToString();
                        oPaciente.Documento = dr["Documento"].ToString();
                        oPaciente.Email = dr["Email"].ToString();
                        lista.Add(oPaciente);
                    }
                }
            }
            return lista;
        }

        // --- 7. PACIENTES: Obtener Historial de Paciente ---
        public List<HistorialInfo> ObtenerHistorialPaciente(string idPaciente)
        {
            var lista = new List<HistorialInfo>();
            SqlDataReader dr = null;
            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("sp_PanelMedico_ObtenerHistorialPaciente", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_PAC", idPaciente);

                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var oHistorial = new HistorialInfo();
                        oHistorial.IdCita = dr["IdCita"].ToString();
                        // 1. Obtenemos el TimeSpan (ej: 09:30:00)
                        TimeSpan horaCita = (TimeSpan)dr["HORA"];
                        // 2. Obtenemos la FECHA (de la base de datos)
                        DateTime fechaCita = (DateTime)dr["FECHA"];
                        // 3. Combinamos la fecha con la hora
                        oHistorial.HoraCompleta = fechaCita.Date + horaCita; // <--- SOLUCIÓN
                        oHistorial.ESTADO = dr["ESTADO"].ToString();
                        oHistorial.Especialidad = dr["Especialidad"].ToString();
                        oHistorial.Doctor = dr["Doctor"].ToString();
                        // Manejo de posibles nulos en Diagnóstico
                        oHistorial.Diagnostico = dr["DIAGNOSTICO"] != DBNull.Value ? dr["DIAGNOSTICO"].ToString() : "N/A";
                        lista.Add(oHistorial);
                    }
                }
            }
            return lista;
        }

        
        // --- 8. OBTENER CONSULTA ID POR CITA ID ---

        public string ObtenerConsultaIdPorCitaId(string idCita)
        {
            string consultaId = null;
            using (var cn = Conectar())
            {
                // Tu SP sp_PanelMedico_IniciarConsulta ya creó esta fila.
                // Ahora solo la buscamos.
                using (var cmd = new SqlCommand("SELECT ID FROM dbo.ConsultasMedicas WHERE ID_CITA = @ID_CITA", cn))
                {
                    cmd.CommandType = CommandType.Text; // Es un query simple, no un SP
                    cmd.Parameters.AddWithValue("@ID_CITA", idCita);

                    var resultado = cmd.ExecuteScalar();
                    if (resultado != null)
                    {
                        consultaId = resultado.ToString();
                    }
                }
            }
            return consultaId;
        }

        // --- 9. OBTENER FICHA COMPLETA DE CITA ---
        public DetalleCitaDTO ObtenerFichaCita(string idCita)
        {
            DetalleCitaDTO ficha = null;
            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("sp_PanelMedico_ObtenerFichaCita", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_CITA", idCita);

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            ficha = new DetalleCitaDTO
                            {
                                IdCita = dr["IdCita"].ToString(),
                                Fecha = (DateTime)dr["FECHA"],
                                Hora = (TimeSpan)dr["HORA"],
                                Motivo = dr["MOTIVO"].ToString(),
                                Estado = dr["ESTADO"].ToString(),

                                PacienteNombre = dr["NombresPaciente"].ToString() + " " + dr["ApellidosPaciente"].ToString(),
                                PacienteDNI = dr["NumDocPaciente"].ToString(),
                                PacienteEmail = dr["EmailPaciente"].ToString(),
                                PacienteTelefono = dr["TelefonoPaciente"].ToString(),
                                PacientePeso = dr["PESO"] != DBNull.Value ? (decimal?)dr["PESO"] : null,

                                DoctorNombre = dr["NombresDoctor"].ToString() + " " + dr["ApellidosDoctor"].ToString(),
                                Especialidad = dr["Especialidad"].ToString(),
                                Sede = dr["Sede"].ToString(),

                                // Estos pueden venir vacíos si la consulta no se ha hecho
                                Sintomas = dr["SINTOMAS"] != DBNull.Value ? dr["SINTOMAS"].ToString() : "---",
                                Diagnostico = dr["DIAGNOSTICO"] != DBNull.Value ? dr["DIAGNOSTICO"].ToString() : "---",
                                Tratamiento = dr["TRATAMIENTO"] != DBNull.Value ? dr["TRATAMIENTO"].ToString() : "---",
                                Observaciones = dr["OBSERVACIONES"] != DBNull.Value ? dr["OBSERVACIONES"].ToString() : "---"
                            };
                        }
                    }
                }
            }
            return ficha;
        }
    }
}