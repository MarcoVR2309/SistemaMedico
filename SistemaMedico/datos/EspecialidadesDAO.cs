using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaMedico.conexion;
using SistemaMedico.modelo;

namespace SistemaMedico.datos
{
    public class EspecialidadesDAO
    {
        private SqlConnection GetConnection()
        {
            SistemaMedico.conexion.ConexionDB objConexion = new SistemaMedico.conexion.ConexionDB();
            return objConexion.ObtenerConexion();
        }

        public int AgregarEspecialidad(Especialidades oEspecialidad)
        {
            int resultado = 0;
            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertarEspecialidad", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Nombre", oEspecialidad.NomEsp);
                    cmd.Parameters.AddWithValue("@Descripcion", oEspecialidad.DesEsp);

                    SqlParameter paramOut = cmd.Parameters.Add("@IdGenerado", SqlDbType.VarChar, 20);
                    paramOut.Direction = ParameterDirection.Output;

                    resultado = cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("DAO Error al agregar especialidad: " + ex.Message);
                }
            }
            return resultado;
        }

        public List<Especialidades> ListarEspecialidades()
        {
            List<Especialidades> lista = new List<Especialidades>();
            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ListarEspecialidades", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Especialidades()
                            {
                                Id = dr["ID"].ToString(),
                                NomEsp = dr["NOM_ESP"].ToString(),
                                DesEsp = dr["DES_ESP"] != DBNull.Value ? dr["DES_ESP"].ToString() : ""
                            });
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("DAO Error al listar especialidades: " + ex.Message);
                }
            }
            return lista;
        }

        public Especialidades ObtenerPorId(string id)
        {
            Especialidades especialidad = null;
            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerEspecialidadPorId", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            especialidad = new Especialidades()
                            {
                                Id = dr["ID"].ToString(),
                                NomEsp = dr["NOM_ESP"].ToString(),
                                DesEsp = dr["DES_ESP"] != DBNull.Value ? dr["DES_ESP"].ToString() : ""
                            };
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("DAO Error al obtener especialidad: " + ex.Message);
                }
            }
            return especialidad;
        }

        public bool Actualizar(Especialidades especialidad)
        {
            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarEspecialidad", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", especialidad.Id);
                    cmd.Parameters.AddWithValue("@Nombre", especialidad.NomEsp);
                    cmd.Parameters.AddWithValue("@Descripcion", 
                        string.IsNullOrWhiteSpace(especialidad.DesEsp) ? (object)DBNull.Value : especialidad.DesEsp);

                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex)
                {
                    throw new Exception("DAO Error al actualizar especialidad: " + ex.Message);
                }
            }
        }
    }
}
