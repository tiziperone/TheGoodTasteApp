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

        //Login
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
                                IdUsuario = Convert.ToInt32(reader["DNI"]),
                                NombreUsuario = reader["Username"].ToString(),
                                Rol = (RolUsuario)Convert.ToInt32(reader["IdRol"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        // NUEVO MÉTODO: Registra una localidad nueva o devuelve el ID si ya existe (considera Provincia dinámicamente)
        public int RegistrarYObtenerIdLocalidad(string nombre, string provincia)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();

                // 1. Verificamos si la localidad ya existe buscando coincidencia por nombre y provincia
                string checkQuery = "SELECT IdLocalidad FROM Localidad WHERE Nombre = @Nombre AND Provincia = @Provincia";
                using (SqlCommand cmdCheck = new SqlCommand(checkQuery, conexion))
                {
                    cmdCheck.Parameters.AddWithValue("@Nombre", nombre);
                    cmdCheck.Parameters.AddWithValue("@Provincia", provincia);
                    object result = cmdCheck.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result); // Ya existía, retornamos su ID
                    }
                }

                // 2. Si no existe, la insertamos utilizando la provincia extraída
                string insertQuery = @"
                    INSERT INTO Localidad (Nombre, Provincia) 
                    OUTPUT INSERTED.IdLocalidad 
                    VALUES (@Nombre, @Provincia)";

                using (SqlCommand cmdInsert = new SqlCommand(insertQuery, conexion))
                {
                    cmdInsert.Parameters.AddWithValue("@Nombre", nombre);
                    cmdInsert.Parameters.AddWithValue("@Provincia", provincia);
                    return (int)cmdInsert.ExecuteScalar(); // Retorna el nuevo ID autogenerado
                }
            }
        }

        //Ingresar un nuevo usuario
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

        //Actualizar algun dato de algun usuario existente
        public bool ActualizarUsuario(int dni, string username, string password, int idRol,
                             string nombre, string apellido, string direccion, int idLocalidad,
                             DateTime fechaNacimiento, string telefono, string email, string sexo)
        {
            bool actualizaPass = password != "********" && !string.IsNullOrWhiteSpace(password);

            string query = @"
                UPDATE Usuarios 
                SET Username = @user, " +
                    (actualizaPass ? "PasswordHash = @pass, " : "") + @"
                    IdRol = @rol,
                    Nombre = @nombre,
                    Apellido = @apellido,
                    Direccion = @direccion,
                    IdLocalidad = @idLocalidad,
                    FechaNacimiento = @fechaNacimiento,
                    Telefono = @telefono,
                    Email = @email,
                    Sexo = @sexo
                WHERE DNI = @dni";

            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@dni", dni);
                    cmd.Parameters.AddWithValue("@user", username);
                    if (actualizaPass) cmd.Parameters.AddWithValue("@pass", password);
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

        //Comprueba si existe ya algun usuario con ese dni
        public bool ExisteDNI(int dni)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE DNI = @DNI";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@DNI", dni);

                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        //Comprueba si existe algun usuario con ese mismo nombre de usuario
        public bool ExisteUsuario(string username)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE Username = @Username";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@Username", username);

                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        //Comprueba si es que existe algun usuario con ese mail
        public bool ExisteEmail(string email)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@Email", email);

                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        //Comprueba si existe algun usuario con ese numero de telefono asociado
        public bool ExisteTelefono(string telefono)
        {
            using (SqlConnection conexion = new SqlConnection(_cadenaConexion))
            {
                conexion.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE Telefono = @Telefono";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@Telefono", telefono);

                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        //Obtiene un usuario por su dni (PK), devuelve null si no existe
        public DataRow ObtenerUsuarioPorDNI(int dni)
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM Usuarios WHERE DNI = @DNI";

            using (SqlConnection con = new SqlConnection(_cadenaConexion))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DNI", dni);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        //Graba un nuevo usuario en la base de datos, devuelve true si se pudo insertar correctamente
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

        //Cambia el estado de un usuario (activo/inactivo)
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
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        //Obtiene todas las localidades de la base de datos, devuelve un DataTable con IdLocalidad y Descripcion
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