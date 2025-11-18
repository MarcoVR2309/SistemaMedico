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
            ConexionDB objConexion = new ConexionDB();
            return objConexion.ObtenerConexion();
        }

        // --------------------------
        // INSERTAR DOCTOR
        // --------------------------
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

        // --------------------------
        // LISTAR DOCTORES
        // --------------------------
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

        // --------------------------
        // OBTENER POR ID
        // --------------------------
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

        // --------------------------
        // ACTUALIZAR DOCTOR
        // --------------------------
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

        // --------------------------
        // OBTENER DOCTOR POR ID_USUARIO
        // --------------------------
        public Doctores ObtenerDoctorPorIdUsuario(string idUsuario)
        {
            Doctores oDoctor = null;

            using (SqlConnection conexion = GetConnection())
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_ObtenerDoctorPorIdUsuario", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

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
                    throw new Exception("DAO Error al obtener doctor por ID de Usuario: " + ex.Message);
                }
            }

            return oDoctor;
        }

        // ==============================================================================
        // 🔥 FUNCIÓN MEJORADA PARA EVITAR ERRORES POR COLUMNAS INEXISTENTES (COD_MED, EXPE, etc.)
        // ==============================================================================
        private bool TieneColumna(SqlDataReader dr, string nombre)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        // ==============================================================================
        // 🔥 MAPEAR DOCTOR 100% SEGURO — NO FALLA SI UN SP NO RETORNA UNA COLUMNA
        // ==============================================================================
        private Doctores MapearDoctor(SqlDataReader dr)
        {
            return new Doctores()
            {
                Id = TieneColumna(dr, "ID") ? dr["ID"].ToString() : "",
                IdUsu = TieneColumna(dr, "ID_USU") ? dr["ID_USU"].ToString() : "",
                IdEsp = TieneColumna(dr, "ID_ESP") ? dr["ID_ESP"].ToString() : "",

                CodMed = TieneColumna(dr, "COD_MED") && dr["COD_MED"] != DBNull.Value
                    ? dr["COD_MED"].ToString()
                    : "",

                Expe = TieneColumna(dr, "EXPE") && dr["EXPE"] != DBNull.Value
                    ? Convert.ToInt32(dr["EXPE"])
                    : 0,

                Nom = TieneColumna(dr, "NOM") ? dr["NOM"].ToString() : "",
                Ape = TieneColumna(dr, "APE") ? dr["APE"].ToString() : "",
                Email = TieneColumna(dr, "EMAIL") ? dr["EMAIL"].ToString() : "",
                Tel = TieneColumna(dr, "TEL") ? dr["TEL"].ToString() : "",
                NombreEspecialidad = TieneColumna(dr, "NombreEspecialidad")
                    ? dr["NombreEspecialidad"].ToString()
                    : ""
            };
        }
    }
}
