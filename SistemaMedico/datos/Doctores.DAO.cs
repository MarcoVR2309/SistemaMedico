using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaMedico.conexion;
using SistemaMedico.modelo;

namespace SistemaMedico.datos
{
    public class DoctoresDAO
    {
        private SqlConnection GetConnection()
        {
            SistemaMedico.conexion.ConexionDB objConexion = new SistemaMedico.conexion.ConexionDB();
            return objConexion.ObtenerConexion();
        }

        public string AgregarDoctor(Doctores oDoctor)
        {
            string idGenerado = "";
            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertarDoctor", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", oDoctor.IdUsu);
                    cmd.Parameters.AddWithValue("@IdEspecialidad", oDoctor.IdEsp);
                    cmd.Parameters.AddWithValue("@NumColegiatura", oDoctor.CodMed);
                    cmd.Parameters.AddWithValue("@AniosExperiencia", oDoctor.Expe);

                    SqlParameter paramOut = cmd.Parameters.Add("@IdGenerado", SqlDbType.VarChar, 20);
                    paramOut.Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    idGenerado = paramOut.Value.ToString();
                }
                catch (SqlException ex)
                {
                    throw new Exception("DAO Error al insertar el doctor: " + ex.Message);
                }
            }
            return idGenerado;
        }

        public List<Doctores> ListarDoctores()
        {
            List<Doctores> lista = new List<Doctores>();
            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ListarDoctores", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(MapearDoctor(dr)); 
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("DAO Error al listar los doctores: " + ex.Message);
                }
            }
            return lista;
        }

        public Doctores ObtenerDoctorPorId(string idDoctor)
        {
            Doctores oDoctor = null;
            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerDoctorPorId", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", idDoctor);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            oDoctor = MapearDoctor(dr); 
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("DAO Error al obtener doctor por ID: " + ex.Message);
                }
            }
            return oDoctor;
        }

        public int ActualizarDoctor(Doctores oDoctor)
        {
            int resultado = 0;
            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarDoctor", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", oDoctor.Id);
                    cmd.Parameters.AddWithValue("@IdEspecialidad", oDoctor.IdEsp);
                    cmd.Parameters.AddWithValue("@NumColegiatura", oDoctor.CodMed);
                    cmd.Parameters.AddWithValue("@AniosExperiencia", oDoctor.Expe);

                    resultado = cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("DAO Error al actualizar el doctor: " + ex.Message);
                }
            }
            return resultado;
        }

        private Doctores MapearDoctor(SqlDataReader dr)
        {
            // Función para evitar la repetición de código de mapeo en los métodos READ
            return new Doctores()
            {
                Id = dr["ID"].ToString(),
                IdUsu = dr["ID_USU"].ToString(),
                IdEsp = dr["ID_ESP"].ToString(),
                CodMed = dr["NUM_COLEGIATURA"].ToString(),
                Expe = dr["ANIOS_EXP"] != DBNull.Value ? Convert.ToInt32(dr["ANIOS_EXP"]) : (int?)null,

                // Propiedades de Navegación (del JOIN)
                Nom = dr["NOM"].ToString(),
                Ape = dr["APE"].ToString(),
                Email = dr["EMAIL"].ToString(),
                Tel = dr["TEL"].ToString(),
                NombreEspecialidad = dr["NombreEspecialidad"].ToString()
            };
        }
        public Doctores ObtenerDoctorPorIdUsuario(string idUsuario)
        {
            Doctores oDoctor = null;
            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    // Llama al Stored Procedure que ya existe en tu BD
                    SqlCommand cmd = new SqlCommand("sp_ObtenerDoctorPorIdUsuario", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // Reutilizamos la función MapearDoctor que ya tienes
                            oDoctor = MapearDoctor(dr);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("DAO Error al obtener doctor por ID de Usuario: " + ex.Message);
                }
            }
            return oDoctor;
        }

    } // Fin de la clase DoctoresDAO
}