using System;
using System.Data;
using System.Data.SqlClient;
using SistemaMedico.conexion;
using SistemaMedico.modelo;

namespace SistemaMedico.datos
{
    public class AdministradoresDAO
    {
        private ConexionDB conexionDB;

        public AdministradoresDAO()
        {
            this.conexionDB = new ConexionDB();
        }

        // =====================================================
        // INSERTAR ADMINISTRADOR
        // =====================================================
        public string Insertar(string idUsuario, int nivelAcceso = 1, string permisosEspeciales = null)
        {
            string idGenerado = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertarAdministrador", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@NivelAcceso", nivelAcceso);
                    cmd.Parameters.AddWithValue("@PermisosEspeciales", permisosEspeciales ?? (object)DBNull.Value);

                    SqlParameter paramId = new SqlParameter("@IdGenerado", SqlDbType.VarChar, 20);
                    paramId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramId);

                    cmd.ExecuteNonQuery();

                    idGenerado = paramId.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar administrador: " + ex.Message);
            }

            return idGenerado;
        }

        // =====================================================
        // OBTENER ADMINISTRADOR POR ID USUARIO
        // =====================================================
        public Administradores ObtenerPorIdUsuario(string idUsuario)
        {
            Administradores admin = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("ObtenerAdministradorPorIdUsuario", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            admin = MapearAdministrador(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener administrador por ID de usuario: " + ex.Message);
            }

            return admin;
        }

        // =====================================================
        // OBTENER ADMINISTRADOR POR ID
        // =====================================================
        public Administradores ObtenerPorId(string id)
        {
            Administradores admin = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("ObtenerAdministradorPorId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            admin = MapearAdministrador(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener administrador: " + ex.Message);
            }

            return admin;
        }

        // =====================================================
        // ACTUALIZAR ADMINISTRADOR
        // =====================================================
        public bool Actualizar(Administradores admin)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("ActualizarAdministrador", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", admin.Id);
                    cmd.Parameters.AddWithValue("@NivelAcceso", admin.NivelAcceso);
                    cmd.Parameters.AddWithValue("@PermisosEspeciales", admin.PermisosEspeciales ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar administrador: " + ex.Message);
            }
        }

        // =====================================================
        // REGISTRAR ACCIÓN ADMINISTRATIVA
        // =====================================================
        public void RegistrarAccion(string idAdmin, string tipoAccion, string descripcion = null)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarAccionAdmin", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdAdmin", idAdmin);
                    cmd.Parameters.AddWithValue("@TipoAccion", tipoAccion);
                    cmd.Parameters.AddWithValue("@Descripcion", descripcion ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar acción administrativa: " + ex.Message);
            }
        }

        // =====================================================
        // LISTAR ADMINISTRADORES
        // =====================================================
        public DataTable ListarAdministradores()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("ListarAdministradores", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar administradores: " + ex.Message);
            }

            return dt;
        }

        // =====================================================
        // CONTAR ADMINISTRADORES ACTIVOS
        // =====================================================
        public int ContarAdministradoresActivos()
        {
            int total = 0;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    string query = @"SELECT COUNT(*) 
                                   FROM Administradores a
                                   INNER JOIN Usuarios u ON a.ID_USU = u.ID
                                   WHERE u.Activo = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    total = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al contar administradores activos: " + ex.Message);
            }

            return total;
        }

        // =====================================================
        // OBTENER ÚLTIMA ACCIÓN DEL ADMINISTRADOR
        // =====================================================
        public DateTime? ObtenerUltimaAccion(string idAdmin)
        {
            DateTime? ultimaAccion = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    string query = "SELECT ULTIMA_ACCION FROM Administradores WHERE ID = @Id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", idAdmin);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        ultimaAccion = Convert.ToDateTime(result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener última acción: " + ex.Message);
            }

            return ultimaAccion;
        }

        // =====================================================
        // VERIFICAR SI USUARIO ES ADMINISTRADOR
        // =====================================================
        public bool EsAdministrador(string idUsuario)
        {
            bool esAdmin = false;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    string query = "SELECT COUNT(*) FROM Administradores WHERE ID_USU = @IdUsuario";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    int count = (int)cmd.ExecuteScalar();
                    esAdmin = count > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar si es administrador: " + ex.Message);
            }

            return esAdmin;
        }

        // =====================================================
        // MAPEAR ADMINISTRADOR DESDE DATAREADER
        // =====================================================
        private Administradores MapearAdministrador(SqlDataReader reader)
        {
            return new Administradores
            {
                Id = reader["ID"].ToString(),
                IdUsu = reader["ID_USU"].ToString(),
                NivelAcceso = Convert.ToInt32(reader["NIVEL_ACCESO"]),
                PermisosEspeciales = reader["PERMISOS_ESPECIALES"] != DBNull.Value
                    ? reader["PERMISOS_ESPECIALES"].ToString()
                    : null,
                UltimaAccion = reader["ULTIMA_ACCION"] != DBNull.Value
                    ? Convert.ToDateTime(reader["ULTIMA_ACCION"])
                    : (DateTime?)null,
                AccionesRealizadas = Convert.ToInt32(reader["ACCIONES_REALIZADAS"])
            };
        }
    }
}