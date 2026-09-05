using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class UsuarioDatos
    {
        // Obtiene la cadena de conexión configurada en App.config
        private readonly string _cadenaConexion = ConfigurationManager.ConnectionStrings["CadenaConexion"].ConnectionString;

        // Genera el hash SHA256 de la contraseña ingresada para compararlo con el guardado en la base de datos
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

            // Consulta parametrizada para evitar inyecciones SQL
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

            return null; // Credenciales inválidas o usuario inactivo
        }
    }
}