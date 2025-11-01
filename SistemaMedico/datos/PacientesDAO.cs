using System;
using System.Data;
using System.Data.SqlClient;
using SistemaMedico.conexion;
using SistemaMedico.modelo;

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
    }
}
