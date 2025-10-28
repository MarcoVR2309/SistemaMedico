using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace SistemaMedico.conexion
{
    public class ConexionDB
    {
        private string conexionString;

        public ConexionDB()
        {
            conexionString = ConfigurationManager.ConnectionStrings["RedClinicas"].ConnectionString;
        }

        public SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(conexionString);
            try
            {
                conexion.Open();
                return conexion;
            }
            catch (SqlException ex)
            {
                throw new Exception("error al conectar la base de datos" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("error al abrir la conexion" + ex.Message);
            }
        }
    }
}