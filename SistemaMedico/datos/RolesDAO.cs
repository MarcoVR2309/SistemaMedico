using System;
using System.Data;
using System.Data.SqlClient;
using SistemaMedico.conexion;

namespace SistemaMedico.datos
{
    public class RolesDAO
    {
        private ConexionDB conexionDB;

        public RolesDAO()
        {
            this.conexionDB = new ConexionDB();
        }

        public string ObtenerIdRolPorNombre(string nombreRol)
        {
            string idRol = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    string query = "SELECT ID FROM Roles WHERE NOM_ROL = @NombreRol";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@NombreRol", nombreRol);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        idRol = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener ID del rol: " + ex.Message);
            }

            return idRol;
        }
    }
}
