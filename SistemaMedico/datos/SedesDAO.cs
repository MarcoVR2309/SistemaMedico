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
                                DirSede = reader["DIR_SEDE"].ToString()
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
                                DirSede = reader["DIR_SEDE"].ToString()
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
    }
}