using SistemaMedico.conexion;
using SistemaMedico.modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SistemaMedico.datos
{
    // =========================================================================
    // CLASES "VIEWMODEL" PARA MANEJAR DATOS DE TABLAS UNIDAS (JOINS)
    // =========================================================================

    // Usada para el Dashboard y "Mi Horario"
    public class CitaInfo
    {
        public string IdCita { get; set; }
        public DateTime HoraCompleta { get; set; }
        public string NombresPaciente { get; set; }
        public string ApellidosPaciente { get; set; }
        public string ESTADO { get; set; }
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
        public DateTime HoraCompleta { get; set; }
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
            return con.ObtenerConexion();
        }

        // =========================================================================
        // MÉTODOS PARA PANEL MÉDICO Y PACIENTE (COMPARTIDOS)
        // =========================================================================

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
                        var oCitaInfo = new CitaInfo();
                        oCitaInfo.IdCita = dr["IdCita"].ToString();

                        TimeSpan horaCita = (TimeSpan)dr["HORA"];
                        oCitaInfo.HoraCompleta = fecha.Date + horaCita;

                        oCitaInfo.ESTADO = dr["ESTADO"].ToString();
                        oCitaInfo.NombresPaciente = dr["NombresPaciente"].ToString();
                        oCitaInfo.ApellidosPaciente = dr["ApellidosPaciente"].ToString();
                        lista.Add(oCitaInfo);
                    }
                }
            }
            return lista;
        }

        // --- 2. Registrar Nueva Cita (USADO POR MÉDICO Y PACIENTE) ---
        /// <summary>
        /// Registra una nueva cita. Usado tanto por el panel médico como por el panel del paciente.
        /// Para el paciente: enviar TipoPago=null, Monto=null, PagoReali=false
        /// </summary>
        public string RegistrarCita(Citas oCita)
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

                    nuevaConsultaId = cmd.ExecuteScalar().ToString();
                }
            }
            return nuevaConsultaId;
        }

        // --- 4. DASHBOARD: Finalizar Consulta ---
        public void FinalizarConsulta(ConsultasMedicas oConsulta)
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
                        var oCitaInfo = new CitaInfo();
                        oCitaInfo.IdCita = dr["IdCita"].ToString();

                        TimeSpan horaCita = (TimeSpan)dr["HORA"];
                        DateTime fechaCita = (DateTime)dr["FECHA"];
                        oCitaInfo.HoraCompleta = fechaCita.Date + horaCita;

                        oCitaInfo.ESTADO = dr["ESTADO"].ToString();
                        oCitaInfo.NombresPaciente = dr["NombresPaciente"].ToString();
                        oCitaInfo.ApellidosPaciente = dr["ApellidosPaciente"].ToString();
                        lista.Add(oCitaInfo);
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

                        TimeSpan horaCita = (TimeSpan)dr["HORA"];
                        DateTime fechaCita = (DateTime)dr["FECHA"];
                        oHistorial.HoraCompleta = fechaCita.Date + horaCita;

                        oHistorial.ESTADO = dr["ESTADO"].ToString();
                        oHistorial.Especialidad = dr["Especialidad"].ToString();
                        oHistorial.Doctor = dr["Doctor"].ToString();
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
                using (var cmd = new SqlCommand("SELECT ID FROM dbo.ConsultasMedicas WHERE ID_CITA = @ID_CITA", cn))
                {
                    cmd.CommandType = CommandType.Text;
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

        // =========================================================================
        // MÉTODOS ESPECÍFICOS PARA PANEL PACIENTE
        // =========================================================================

        // --- 9. PANEL PACIENTE: Listar todas las citas del paciente ---
        public List<Citas> ListarCitasPorPaciente(string idPaciente)
        {
            var lista = new List<Citas>();
            SqlDataReader dr = null;

            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("sp_ListarCitasPorPaciente", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                    dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        var cita = new Citas();
                        cita.Id = dr["ID"].ToString();

                        DateTime fechaHoraCita = Convert.ToDateTime(dr["FEC_CITA"]);
                        cita.Fecha = fechaHoraCita.Date;
                        cita.Hora = fechaHoraCita.TimeOfDay;

                        cita.Estado = dr["ESTADO"].ToString();
                        cita.Motivo = dr["MOTIVO"] != DBNull.Value ? dr["MOTIVO"].ToString() : "";

                        cita.NombreDoctor = dr["NombreDoctor"].ToString() + " " + dr["ApellidoDoctor"].ToString();
                        cita.NombreEspecialidad = dr["Especialidad"].ToString();
                        cita.SedeNombre = dr["NombreSede"].ToString();

                        lista.Add(cita);
                    }
                }
            }

            return lista;
        }

        // --- 10. PANEL PACIENTE: Cancelar Cita ---
        public void CancelarCitaPaciente(string idCita, string idPaciente)
        {
            using (var cn = Conectar())
            {
                using (var cmd = new SqlCommand("PanelPaciente_CancelarCita", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID_CITA", idCita);
                    cmd.Parameters.AddWithValue("@ID_PAC", idPaciente);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // --- 11. OBTENER FICHA COMPLETA DE CITA ---
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
                                Motivo = dr["MOTIVO"] != DBNull.Value ? dr["MOTIVO"].ToString() : "Sin motivo",
                                Estado = dr["ESTADO"].ToString(),

                                PacienteNombre = dr["NombresPaciente"].ToString() + " " + dr["ApellidosPaciente"].ToString(),
                                PacienteDNI = dr["NumDocPaciente"].ToString(),
                                PacienteEmail = dr["EmailPaciente"].ToString(),
                                PacienteTelefono = dr["TelefonoPaciente"] != DBNull.Value ? dr["TelefonoPaciente"].ToString() : "No registrado",
                                PacientePeso = dr["PESO"] != DBNull.Value ? (decimal?)dr["PESO"] : null,

                                DoctorNombre = dr["NombresDoctor"].ToString() + " " + dr["ApellidosDoctor"].ToString(),
                                Especialidad = dr["Especialidad"].ToString(),
                                Sede = dr["Sede"].ToString(),

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