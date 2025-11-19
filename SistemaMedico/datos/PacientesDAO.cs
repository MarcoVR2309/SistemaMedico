using SistemaMedico.conexion;
using SistemaMedico.modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SistemaMedico.datos
{
    public class PacientesDAO
    {
        private ConexionDB conexionDB;

        public PacientesDAO()
        {
            this.conexionDB = new ConexionDB();
        }

        public string Insertar(Pacientes paciente)
        {
            string idGenerado = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertarPaciente", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", paciente.IdUsu);
                    cmd.Parameters.AddWithValue("@Peso", paciente.Peso ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Edad", paciente.Edad ?? (object)DBNull.Value);

                    SqlParameter paramId = new SqlParameter("@IdGenerado", SqlDbType.VarChar, 20);
                    paramId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramId);

                    cmd.ExecuteNonQuery();

                    idGenerado = paramId.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar paciente: " + ex.Message);
            }

            return idGenerado;
        }

        public Pacientes ObtenerPorId(string id)
        {
            Pacientes paciente = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerPacientePorId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            paciente = MapearPaciente(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener paciente: " + ex.Message);
            }

            return paciente;
        }

        public Pacientes ObtenerPorIdUsuario(string idUsuario)
        {
            Pacientes paciente = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerPacientePorIdUsuario", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            paciente = MapearPaciente(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener paciente por ID usuario: " + ex.Message);
            }

            return paciente;
        }

        public bool Actualizar(Pacientes paciente)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarPaciente", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", paciente.Id);
                    cmd.Parameters.AddWithValue("@Peso", paciente.Peso ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Edad", paciente.Edad ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar paciente: " + ex.Message);
            }
        }

        private Pacientes MapearPaciente(SqlDataReader reader)
        {
            return new Pacientes
            {
                Id = reader["ID"].ToString(),
                IdUsu = reader["ID_USU"].ToString(),
                Peso = reader["PESO"] != DBNull.Value ? Convert.ToDecimal(reader["PESO"]) : (decimal?)null,
                Edad = reader["EDAD"] != DBNull.Value ? Convert.ToDateTime(reader["EDAD"]) : (DateTime?)null
            };
        }
        // NUEVA CLASE: Para llenar el DropDownList del modal
        // =================================================================
        public class PacienteParaDropdown
        {
            public string ID { get; set; } // ID del Paciente (ej: P0000001)
            public string NombreCompleto { get; set; }
        }
        // NUEVO M�TODO: El que faltaba
        // =================================================================
        public List<PacienteParaDropdown> ListarPacientesParaDropdown()
        {
            var lista = new List<PacienteParaDropdown>();
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    // Llama al Stored Procedure que ya tienes y que une con Usuarios
                    SqlCommand cmd = new SqlCommand("sp_ListarPacientes", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var paciente = new PacienteParaDropdown
                            {
                                // ID de la tabla Pacientes
                                ID = reader["ID"].ToString(),

                                // NOM y APE de la tabla Usuarios
                                NombreCompleto = reader["NOM"].ToString() + " " + reader["APE"].ToString()
                            };
                            lista.Add(paciente);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Lanza una excepci�n m�s espec�fica
                throw new Exception("Error al listar pacientes para dropdown: " + ex.Message);
            }
            return lista;
        }
    }
}
