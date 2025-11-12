using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SistemaMedico.conexion;
using SistemaMedico.modelo;

namespace SistemaMedico.datos
{
    public class AdminUsuariosDAO
    {
        private readonly ConexionDB conexionDB;
        private readonly UsuariosDAO usuariosDAO;

        public AdminUsuariosDAO()
        {
            conexionDB = new ConexionDB();
            usuariosDAO = new UsuariosDAO();
        }

        public AdminUsuariosLista ListarUsuarios(string textoBusqueda = null, string idRol = null, string estado = null)
        {
            AdminUsuariosLista resultado = new AdminUsuariosLista();

            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand("sp_Admin_ListarUsuarios", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TextoBusqueda", (object)textoBusqueda ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdRol", (object)idRol ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", (object)estado ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            resultado.Resumen.TotalUsuarios = reader["TotalUsuarios"] != DBNull.Value ? Convert.ToInt32(reader["TotalUsuarios"]) : 0;
                            resultado.Resumen.TotalDoctores = reader["TotalDoctores"] != DBNull.Value ? Convert.ToInt32(reader["TotalDoctores"]) : 0;
                            resultado.Resumen.TotalPacientes = reader["TotalPacientes"] != DBNull.Value ? Convert.ToInt32(reader["TotalPacientes"]) : 0;
                            resultado.Resumen.TotalActivos = reader["TotalActivos"] != DBNull.Value ? Convert.ToInt32(reader["TotalActivos"]) : 0;
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                resultado.Usuarios.Add(new AdminUsuarioItem
                                {
                                    Id = reader["ID"].ToString(),
                                    NombreCompleto = reader["NombreCompleto"].ToString(),
                                    Email = reader["EMAIL"].ToString(),
                                    Rol = reader["NombreRol"].ToString(),
                                    Telefono = reader["TEL"] != DBNull.Value ? reader["TEL"].ToString() : null,
                                    Activo = reader["Activo"] != DBNull.Value && Convert.ToBoolean(reader["Activo"]),
                                    FechaRegistro = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : DateTime.MinValue,
                                    IdDoctor = reader["IdDoctor"] != DBNull.Value ? reader["IdDoctor"].ToString() : null,
                                    NumeroColegiatura = reader["NUM_COLEGIATURA"] != DBNull.Value ? reader["NUM_COLEGIATURA"].ToString() : null,
                                    AniosExperiencia = reader["ANIOS_EXP"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ANIOS_EXP"]) : null,
                                    IdEspecialidad = reader["IdEspecialidad"] != DBNull.Value ? reader["IdEspecialidad"].ToString() : null,
                                    NombreEspecialidad = reader["NombreEspecialidad"] != DBNull.Value ? reader["NombreEspecialidad"].ToString() : null,
                                    IdPaciente = reader["IdPaciente"] != DBNull.Value ? reader["IdPaciente"].ToString() : null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar usuarios (admin): " + ex.Message, ex);
            }

            return resultado;
        }

        public AdminUsuarioDetalle ObtenerDetalle(string idUsuario)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand("sp_Admin_ObtenerUsuarioDetalle", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapearDetalle(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener detalle del usuario (admin): " + ex.Message, ex);
            }

            return null;
        }

        public (string IdUsuario, string IdRelacionado) CrearUsuario(
            string idRol,
            string tipoDoc,
            string numDoc,
            string apellido,
            string nombre,
            char genero,
            DateTime? fechaNac,
            string email,
            string telefono,
            string passwordPlano,
            string sedePref,
            string idEspecialidad,
            string numColegiatura,
            int? aniosExperiencia,
            decimal? pesoPaciente,
            DateTime? edadPaciente)
        {
            try
            {
                byte[] hash = usuariosDAO.HashPassword(passwordPlano);

                using (SqlConnection conn = conexionDB.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand("sp_Admin_CrearUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdRol", idRol);
                    cmd.Parameters.AddWithValue("@TipoDoc", (object)tipoDoc ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumDoc", (object)numDoc ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellido", apellido);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Genero", genero);
                    cmd.Parameters.AddWithValue("@FechaNac", (object)fechaNac ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Telefono", (object)telefono ?? DBNull.Value);

                    SqlParameter paramHash = new SqlParameter("@PasswordHash", SqlDbType.VarBinary, -1)
                    {
                        Value = (object)hash ?? DBNull.Value
                    };
                    cmd.Parameters.Add(paramHash);

                    cmd.Parameters.AddWithValue("@SedePref", (object)sedePref ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdEspecialidad", (object)idEspecialidad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumColegiatura", (object)numColegiatura ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AnosExperiencia", (object)aniosExperiencia ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PesoPaciente", (object)pesoPaciente ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EdadPaciente", (object)edadPaciente ?? DBNull.Value);

                    SqlParameter paramUsuario = new SqlParameter("@IdUsuarioGenerado", SqlDbType.VarChar, 20)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramUsuario);

                    SqlParameter paramRelacionado = new SqlParameter("@IdRelacionado", SqlDbType.VarChar, 20)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramRelacionado);

                    cmd.ExecuteNonQuery();

                    string idUsuarioGenerado = paramUsuario.Value != DBNull.Value ? paramUsuario.Value.ToString() : null;
                    string idRelacionado = paramRelacionado.Value != DBNull.Value ? paramRelacionado.Value.ToString() : null;

                    return (idUsuarioGenerado, idRelacionado);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear usuario (admin): " + ex.Message, ex);
            }
        }

        public void ActualizarUsuario(
            string idUsuario,
            string idRol,
            string tipoDoc,
            string numDoc,
            string apellido,
            string nombre,
            char genero,
            DateTime? fechaNac,
            string email,
            string telefono,
            string sedePref,
            string idEspecialidad,
            string numColegiatura,
            int? aniosExperiencia,
            decimal? pesoPaciente,
            DateTime? edadPaciente)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand("sp_Admin_ActualizarUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@IdRol", idRol);
                    cmd.Parameters.AddWithValue("@TipoDoc", (object)tipoDoc ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumDoc", (object)numDoc ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Apellido", apellido);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Genero", genero);
                    cmd.Parameters.AddWithValue("@FechaNac", (object)fechaNac ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Telefono", (object)telefono ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SedePref", (object)sedePref ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IdEspecialidad", (object)idEspecialidad ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NumColegiatura", (object)numColegiatura ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AnosExperiencia", (object)aniosExperiencia ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PesoPaciente", (object)pesoPaciente ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@EdadPaciente", (object)edadPaciente ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar usuario (admin): " + ex.Message, ex);
            }
        }

        public bool CambiarEstado(string idUsuario, bool activar)
        {
            try
            {
                using (SqlConnection conn = conexionDB.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand("sp_Admin_CambiarEstadoUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    cmd.Parameters.AddWithValue("@Activar", activar);

                    object resultado = cmd.ExecuteScalar();
                    if (resultado != null && resultado != DBNull.Value)
                    {
                        return Convert.ToBoolean(resultado);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cambiar estado del usuario (admin): " + ex.Message, ex);
            }

            return false;
        }

        public void ResetearPassword(string idUsuario, string nuevoPasswordPlano)
        {
            try
            {
                byte[] hash = usuariosDAO.HashPassword(nuevoPasswordPlano);

                using (SqlConnection conn = conexionDB.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand("sp_Admin_ResetearPasswordUsuario", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    SqlParameter paramHash = new SqlParameter("@PasswordHash", SqlDbType.VarBinary, -1)
                    {
                        Value = (object)hash ?? DBNull.Value
                    };
                    cmd.Parameters.Add(paramHash);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al resetear password (admin): " + ex.Message, ex);
            }
        }

        private AdminUsuarioDetalle MapearDetalle(SqlDataReader reader)
        {
            return new AdminUsuarioDetalle
            {
                Id = reader["ID"].ToString(),
                IdRol = reader["ID_ROL"].ToString(),
                NombreRol = reader["NombreRol"].ToString(),
                TipoDoc = reader["TIPO_DOC"] != DBNull.Value ? reader["TIPO_DOC"].ToString() : null,
                NumDoc = reader["NUM_DOC"] != DBNull.Value ? reader["NUM_DOC"].ToString() : null,
                Apellido = reader["APE"].ToString(),
                Nombre = reader["NOM"].ToString(),
                Genero = reader["GEN"] != DBNull.Value ? Convert.ToChar(reader["GEN"]) : ' ',
                FechaNacimiento = reader["FEC_NAC"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["FEC_NAC"]) : null,
                Email = reader["EMAIL"].ToString(),
                Telefono = reader["TEL"] != DBNull.Value ? reader["TEL"].ToString() : null,
                SedePreferida = reader["SEDE_PREF"] != DBNull.Value ? reader["SEDE_PREF"].ToString() : null,
                Activo = reader["Activo"] != DBNull.Value && Convert.ToBoolean(reader["Activo"]),
                FechaCreacion = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]) : DateTime.MinValue,
                FechaUltimaActualizacion = reader["FechaUltimaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaUltimaActualizacion"]) : DateTime.MinValue,
                IdDoctor = reader["IdDoctor"] != DBNull.Value ? reader["IdDoctor"].ToString() : null,
                IdEspecialidad = reader["ID_ESP"] != DBNull.Value ? reader["ID_ESP"].ToString() : null,
                NombreEspecialidad = reader["NombreEspecialidad"] != DBNull.Value ? reader["NombreEspecialidad"].ToString() : null,
                NumeroColegiatura = reader["NUM_COLEGIATURA"] != DBNull.Value ? reader["NUM_COLEGIATURA"].ToString() : null,
                AniosExperiencia = reader["ANIOS_EXP"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ANIOS_EXP"]) : null,
                IdPaciente = reader["IdPaciente"] != DBNull.Value ? reader["IdPaciente"].ToString() : null,
                PesoPaciente = reader["PESO"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["PESO"]) : null,
                EdadPaciente = reader["EDAD"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["EDAD"]) : null
            };
        }
    }
}

