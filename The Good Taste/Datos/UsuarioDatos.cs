using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class UsuarioDatos
    {
        private readonly string _cadenaConexion = ConfigurationManager.ConnectionStrings["CadenaConexion"].ConnectionString;

        private string GenerarHashSHA256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public UsuarioSistema Autenticar(string user, string pass)
        {
            string hashPassword = GenerarHashSHA256(pass);

            string query = @"
                SELECT IdUsuario, Username, NombreCompleto, IdRol, Activo
                FROM Usuarios
                WHERE Username = @user 
                  AND PasswordHash = @pass 
                  AND Activo = 1";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@pass", hashPassword);

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UsuarioSistema
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                NombreUsuario = reader["Username"].ToString(),
                                Rol = (RolUsuario)Convert.ToInt32(reader["IdRol"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        public bool RegistrarUsuario(string username, string password, string nombreCompleto, int idRol)
        {
            string hashPassword = GenerarHashSHA256(password);

            string query = @"
                INSERT INTO Usuarios (Username, PasswordHash, NombreCompleto, IdRol, Activo)
                VALUES (@user, @pass, @nombre, @rol, 1)";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", hashPassword);
                    cmd.Parameters.AddWithValue("@nombre", nombreCompleto);
                    cmd.Parameters.AddWithValue("@rol", idRol);

                    conexion.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public DataTable ObtenerUsuariosPorEstado(bool activos)
        {
            DataTable dt = new DataTable();

            string query = @"
        SELECT 
            IdUsuario AS [ID],
            Username AS [Usuario],
            NombreCompleto AS [Nombre Completo],
            CASE IdRol
                WHEN 1 THEN 'Admin'
                WHEN 2 THEN 'Gerente'
                WHEN 3 THEN 'Vendedor'
                ELSE 'Desconocido'
            END AS [Rol],
            Activo AS [Estado]
        FROM Usuarios
        WHERE Activo = @activo";

            using (SqlConnection con = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@activo", activos ? 1 : 0);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
    }
}