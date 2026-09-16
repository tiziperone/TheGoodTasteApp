using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class UsuarioDatos
    {
        private readonly string _cadenaConexion = ConfigurationManager.ConnectionStrings["CadenaConexion"].ConnectionString;

        // 1. LOGIN (Usa DNI, quita NombreCompleto y valida contraseña en texto plano)
        public UsuarioSistema Autenticar(string user, string pass)
        {
            string query = @"
                SELECT DNI, Username, (Nombre + ' ' + Apellido) AS NombreCompleto, IdRol, Activo
                FROM Usuarios
                WHERE Username = @user 
                  AND PasswordHash = @pass 
                  AND Activo = 1";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@pass", pass);

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UsuarioSistema
                            {
                                IdUsuario = Convert.ToInt32(reader["DNI"]), // DNI ahora es el identificador principal
                                NombreUsuario = reader["Username"].ToString(),
                                Rol = (RolUsuario)Convert.ToInt32(reader["IdRol"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        // 2. INSERT (Agregado IdLocalidad, DNI en la consulta y sin NombreCompleto)
        public bool RegistrarUsuario(int dni, string username, string password, int idRol,
                                     string nombre, string apellido, string direccion, int idLocalidad,
                                     DateTime fechaNacimiento, string telefono, string email, string sexo)
        {
            string query = @"
                INSERT INTO Usuarios (DNI, Username, PasswordHash, IdRol, Activo, 
                                      Nombre, Apellido, Direccion, IdLocalidad, FechaNacimiento, Telefono, Email, Sexo)
                VALUES (@dni, @user, @pass, @rol, 1, 
                        @nombre, @apellido, @direccion, @idLocalidad, @fechaNacimiento, @telefono, @email, @sexo)";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    cmd.Parameters.AddWithValue("@rol", idRol);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellido", apellido);
                    cmd.Parameters.AddWithValue("@direccion", string.IsNullOrWhiteSpace(direccion) ? (object)DBNull.Value : direccion);
                    cmd.Parameters.AddWithValue("@idLocalidad", idLocalidad);
                    cmd.Parameters.AddWithValue("@fechaNacimiento", fechaNacimiento);
                    cmd.Parameters.AddWithValue("@telefono", string.IsNullOrWhiteSpace(telefono) ? (object)DBNull.Value : telefono);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@sexo", string.IsNullOrWhiteSpace(sexo) ? (object)DBNull.Value : sexo);

                    conexion.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 3. EXISTE DNI (Validamos el DNI directamente como entero)
        public bool ExisteDNI(int dni)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaConexion)) // Actualizado a _cadenaConexion para evitar errores con Conexion.cs
            {
                conexion.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE DNI = @DNI";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@DNI", dni);

                int cantidad = (int)cmd.ExecuteScalar();
                return cantidad > 0;
            }
        }

        // 4. GRILLA (DNI as ID, JOIN con Roles y concatenar Nombre/Apellido)
        public DataTable ObtenerUsuariosPorEstado(bool activos)
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT 
                    u.DNI AS [ID],
                    u.Username AS [Usuario],
                    (u.Nombre + ' ' + u.Apellido) AS [Nombre Completo],
                    r.Nombre AS [Rol],
                    u.Activo AS [Estado]
                FROM Usuarios u
                INNER JOIN Roles r ON u.IdRol = r.IdRol
                WHERE u.Activo = @activo";

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

        // 5. BAJA/ALTA DE USUARIO (El filtro ahora es por DNI)
        public bool CambiarEstadoUsuario(int dni, bool nuevoEstado)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                string query = "UPDATE Usuarios SET Activo = @Activo WHERE DNI = @DNI";

                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Activo", nuevoEstado ? 1 : 0);
                    cmd.Parameters.AddWithValue("@DNI", dni);

                    conexion.Open();
                    int filasAfectadas = cmd.ExecuteNonQuery();

                    return filasAfectadas > 0;
                }
            }
        }

        // 6. OBTENER LOCALIDADES (Para el combo del formulario)
        public DataTable ObtenerLocalidades()
        {
            DataTable dt = new DataTable();
            string query = "SELECT IdLocalidad, (Nombre + ' (' + Provincia + ')') AS Descripcion FROM Localidad";

            using (SqlConnection con = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
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