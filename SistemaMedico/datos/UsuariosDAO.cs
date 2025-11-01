using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using SistemaMedico.conexion;
using SistemaMedico.modelo;

namespace SistemaMedico.datos
{
    public class UsuariosDAO
    {
        private ConexionDB conexionDB;

        public UsuariosDAO()
        {
            this.conexionDB = new ConexionDB();
        }

        public string Insertar(Usuarios usuario)
        {
            string idGenerado = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_InsertarUsuario", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdRol", usuario.IdRol);
                    cmd.Parameters.AddWithValue("@TipoDoc", usuario.TipoDoc ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumDoc", usuario.NumDoc ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Ape);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nom);
                    cmd.Parameters.AddWithValue("@Genero", usuario.Gen);
                    cmd.Parameters.AddWithValue("@FechaNac", usuario.FecNac ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.Tel ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@PasswordHash", usuario.PswdHash);
                    cmd.Parameters.AddWithValue("@SedePref", usuario.SedePref ?? (object)DBNull.Value);

                    SqlParameter paramId = new SqlParameter("@IdGenerado", SqlDbType.VarChar, 20);
                    paramId.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(paramId);

                    cmd.ExecuteNonQuery();

                    idGenerado = paramId.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al insertar usuario: " + ex.Message);
            }

            return idGenerado;
        }

        public Usuarios ObtenerPorId(string id)
        {
            Usuarios usuario = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerUsuarioPorId", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapearUsuario(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuario: " + ex.Message);
            }

            return usuario;
        }

        public Usuarios ObtenerPorEmail(string email)
        {
            Usuarios usuario = null;

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerUsuarioPorEmail", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = MapearUsuario(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener usuario por email: " + ex.Message);
            }

            return usuario;
        }

        public bool Actualizar(Usuarios usuario)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_ActualizarUsuario", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", usuario.Id);
                    cmd.Parameters.AddWithValue("@IdRol", usuario.IdRol);
                    cmd.Parameters.AddWithValue("@TipoDoc", usuario.TipoDoc ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumDoc", usuario.NumDoc ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Ape);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nom);
                    cmd.Parameters.AddWithValue("@Genero", usuario.Gen);
                    cmd.Parameters.AddWithValue("@FechaNac", usuario.FecNac ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", usuario.Email);
                    cmd.Parameters.AddWithValue("@Telefono", usuario.Tel ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SedePref", usuario.SedePref ?? (object)DBNull.Value);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar usuario: " + ex.Message);
            }
        }

        public bool Desactivar(string id)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                {
                    SqlCommand cmd = new SqlCommand("sp_DesactivarUsuario", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al desactivar usuario: " + ex.Message);
            }
        }

        public bool ValidarCredenciales(string email, string password, out Usuarios usuario)
        {
            usuario = null;

            try
            {
                usuario = ObtenerPorEmail(email);

                if (usuario == null || !usuario.Activo)
                {
                    return false;
                }

                byte[] passwordHash = HashPassword(password);
                
                if (usuario.PswdHash == null || usuario.PswdHash.Length == 0)
                {
                    return false;
                }

                return CompararHashes(passwordHash, usuario.PswdHash);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar credenciales: " + ex.Message);
            }
        }

        public byte[] HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                return sha256.ComputeHash(bytes);
            }
        }

        private bool CompararHashes(byte[] hash1, byte[] hash2)
        {
            if (hash1.Length != hash2.Length)
                return false;

            for (int i = 0; i < hash1.Length; i++)
            {
                if (hash1[i] != hash2[i])
                    return false;
            }

            return true;
        }

        private Usuarios MapearUsuario(SqlDataReader reader)
        {
            return new Usuarios
            {
                Id = reader["ID"].ToString(),
                IdRol = reader["ID_ROL"].ToString(),
                TipoDoc = reader["TIPO_DOC"] != DBNull.Value ? reader["TIPO_DOC"].ToString() : null,
                NumDoc = reader["NUM_DOC"] != DBNull.Value ? reader["NUM_DOC"].ToString() : null,
                Ape = reader["APE"].ToString(),
                Nom = reader["NOM"].ToString(),
                Gen = reader["GEN"].ToString()[0],
                FecNac = reader["FEC_NAC"] != DBNull.Value ? Convert.ToDateTime(reader["FEC_NAC"]) : (DateTime?)null,
                Email = reader["EMAIL"].ToString(),
                Tel = reader["TEL"] != DBNull.Value ? reader["TEL"].ToString() : null,
                PswdHash = reader["PSWDHASH"] as byte[],
                SedePref = reader["SEDE_PREF"] != DBNull.Value ? reader["SEDE_PREF"].ToString() : null,
                Activo = Convert.ToBoolean(reader["Activo"]),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                FechaUltimaActualizacion = Convert.ToDateTime(reader["FechaUltimaActualizacion"])
            };
        }
    }
}
