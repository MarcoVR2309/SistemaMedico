using SistemaMedico.conexion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SistemaMedico.modelo;
using System.Data;
using System.Data.SqlClient;

namespace SistemaMedico.datos
{
    public class SedesDAO
    {
        private ConexionDB conexionDB;

        public SedesDAO()
        {
            this.conexionDB = new ConexionDB();
        }

        public List<Sedes> ListarSedes()
        {
            List<Sedes> listaSedes = new List<Sedes>();
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("ListarSedes", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Sedes sede = new Sedes
                            {
                                Id = reader["ID"].ToString(),
                                NomSede = reader["NOM_SEDE"].ToString(),
                                // Intentar leer DIR_SEDE primero, si no existe usar DIRECCION
                                DirSede = reader["DIR_SEDE"] != DBNull.Value 
                                    ? reader["DIR_SEDE"].ToString() 
                                    : (reader["DIRECCION"] != DBNull.Value ? reader["DIRECCION"].ToString() : "")
                            };
                            listaSedes.Add(sede);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar sedes: " + ex.Message);
            }
            return listaSedes;
        }

        public Sedes ObtenerPorId(string id)
        {
            Sedes sede = null;
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    // Usar el nombre SIN prefijo sp_ como está en tu BD
                    SqlCommand cmd = new SqlCommand("ObtenerSedePorId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            sede = new Sedes
                            {
                                Id = reader["ID"].ToString(),
                                NomSede = reader["NOM_SEDE"].ToString(),
                                // Intentar leer DIR_SEDE primero, si no existe usar DIRECCION
                                DirSede = reader["DIR_SEDE"] != DBNull.Value 
                                    ? reader["DIR_SEDE"].ToString() 
                                    : (reader["DIRECCION"] != DBNull.Value ? reader["DIRECCION"].ToString() : "")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener sede por ID: " + ex.Message);
            }
            return sede;
        }

        public bool Actualizar(Sedes sede)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    // Usar el nombre SIN prefijo sp_ como está en tu BD
                    SqlCommand cmd = new SqlCommand("ActualizarSede", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", sede.Id);
                    cmd.Parameters.AddWithValue("@Nombre", sede.NomSede);
                    cmd.Parameters.AddWithValue("@Direccion", 
                        string.IsNullOrWhiteSpace(sede.DirSede) ? (object)DBNull.Value : sede.DirSede);
                    cmd.Parameters.AddWithValue("@Telefono", DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", DBNull.Value);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar sede: " + ex.Message);
            }
        }
    }
}