using System;
using System.Data;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class ClienteDatos
    {
       
        public DataTable ListarClientes()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT c.dniCliente, c.nombreCliente, c.apellidoCliente, c.fechaNaciminetoCliente, 
                                    c.correoCliente, c.telefonoCliente, d.paisCliente, d.localidadCliente, 
                                    d.provinciaCliente, d.calleCliente, d.altura 
                             FROM Cliente c
                             INNER JOIN DireccionCliente d ON c.idDireccionCliente = d.idDireccionCliente";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public Cliente ObtenerPorDni(string dni)
        {
            Cliente cliente = null;
            string query = @"SELECT c.dniCliente, c.nombreCliente, c.apellidoCliente, c.telefonoCliente, c.correoCliente, 
                                    d.calleCliente, d.altura 
                             FROM Cliente c
                             INNER JOIN DireccionCliente d ON c.idDireccionCliente = d.idDireccionCliente
                             WHERE c.dniCliente = @Dni";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Dni", dni);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        cliente = new Cliente
                        {
                            Dni = dr["dniCliente"].ToString(),
                            Nombre = dr["nombreCliente"].ToString(),
                            Apellido = dr["apellidoCliente"].ToString(),
                            Telefono = dr["telefonoCliente"].ToString(),
                            Email = dr["correoCliente"].ToString(),
                            Calle = dr["calleCliente"].ToString(),
                            Numero = dr["altura"].ToString()
                        };
                    }
                }
            }
            return cliente;
        }


        public string VerificarDuplicados(string dni, string email, string telefono)
        {
            string query = "SELECT dniCliente, correoCliente, telefonoCliente FROM Cliente WHERE dniCliente = @Dni OR correoCliente = @Email OR telefonoCliente = @Telefono";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Dni", dni);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Telefono", telefono);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        if (dr["dniCliente"].ToString() == dni) return "DNI";
                        if (dr["correoCliente"].ToString() == email) return "Correo electrónico";
                        if (dr["telefonoCliente"].ToString() == telefono) return "Teléfono";
                    }
                }
            }
            return string.Empty;
        }

        public string VerificarDuplicadosModificacion(string dniOriginal, string dniNuevo, string email, string telefono)
        {
            string query = "SELECT dniCliente, correoCliente, telefonoCliente FROM Cliente WHERE (dniCliente = @DniNuevo OR correoCliente = @Email OR telefonoCliente = @Telefono) AND dniCliente != @DniOriginal";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DniOriginal", dniOriginal);
                cmd.Parameters.AddWithValue("@DniNuevo", dniNuevo);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Telefono", telefono);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        if (dr["dniCliente"].ToString() == dniNuevo) return "DNI";
                        if (dr["correoCliente"].ToString() == email) return "Correo electrónico";
                        if (dr["telefonoCliente"].ToString() == telefono) return "Teléfono";
                    }
                }
            }
            return string.Empty;
        }

        public void InsertarCliente(string dni, string nombre, string apellido, DateTime fechaNacimiento, string correo, string telefono, string pais, string localidad, string provincia, string calle, string altura)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar la Dirección
                        string queryDireccion = @"INSERT INTO DireccionCliente (paisCliente, localidadCliente, provinciaCliente, calleCliente, altura) 
                                                  OUTPUT INSERTED.idDireccionCliente 
                                                  VALUES (@Pais, @Localidad, @Provincia, @Calle, @Altura)";

                        SqlCommand cmdDir = new SqlCommand(queryDireccion, con, tran);
                        cmdDir.Parameters.AddWithValue("@Pais", pais);
                        cmdDir.Parameters.AddWithValue("@Localidad", localidad);
                        cmdDir.Parameters.AddWithValue("@Provincia", provincia);
                        cmdDir.Parameters.AddWithValue("@Calle", calle);
                        cmdDir.Parameters.AddWithValue("@Altura", altura);

                        int idDireccion = (int)cmdDir.ExecuteScalar();

                        // 2. Insertar el Cliente
                        string queryCliente = @"INSERT INTO Cliente (dniCliente, nombreCliente, apellidoCliente, fechaNaciminetoCliente, correoCliente, telefonoCliente, idDireccionCliente, Activo) 
                                                VALUES (@Dni, @Nombre, @Apellido, @FechaNac, @Correo, @Telefono, @IdDireccion, 1)";

                        SqlCommand cmdCli = new SqlCommand(queryCliente, con, tran);
                        cmdCli.Parameters.AddWithValue("@Dni", dni);
                        cmdCli.Parameters.AddWithValue("@Nombre", nombre);
                        cmdCli.Parameters.AddWithValue("@Apellido", apellido);
                        cmdCli.Parameters.AddWithValue("@FechaNac", fechaNacimiento);
                        cmdCli.Parameters.AddWithValue("@Correo", correo);
                        cmdCli.Parameters.AddWithValue("@Telefono", telefono);
                        cmdCli.Parameters.AddWithValue("@IdDireccion", idDireccion);

                        cmdCli.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public void ModificarCliente(string dniOriginal, string dniNuevo, string nombre, string apellido, DateTime fechaNacimiento, string correo, string telefono, string pais, string localidad, string provincia, string calle, string altura)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Averiguar el id de la dirección
                        string queryIdDir = "SELECT idDireccionCliente FROM Cliente WHERE dniCliente = @DniOriginal";
                        SqlCommand cmdGetId = new SqlCommand(queryIdDir, con, tran);
                        cmdGetId.Parameters.AddWithValue("@DniOriginal", dniOriginal);
                        int idDireccion = Convert.ToInt32(cmdGetId.ExecuteScalar());

                        // 2. Modificar el Cliente
                        string queryCliente = @"UPDATE Cliente 
                                                SET dniCliente = @DniNuevo, nombreCliente = @Nombre, apellidoCliente = @Apellido, 
                                                    fechaNaciminetoCliente = @FechaNac, correoCliente = @Correo, telefonoCliente = @Telefono 
                                                WHERE dniCliente = @DniOriginal";

                        SqlCommand cmdCli = new SqlCommand(queryCliente, con, tran);
                        cmdCli.Parameters.AddWithValue("@DniOriginal", dniOriginal);
                        cmdCli.Parameters.AddWithValue("@DniNuevo", dniNuevo);
                        cmdCli.Parameters.AddWithValue("@Nombre", nombre);
                        cmdCli.Parameters.AddWithValue("@Apellido", apellido);
                        cmdCli.Parameters.AddWithValue("@FechaNac", fechaNacimiento);
                        cmdCli.Parameters.AddWithValue("@Correo", correo);
                        cmdCli.Parameters.AddWithValue("@Telefono", telefono);
                        cmdCli.ExecuteNonQuery();

                        // 3. Modificar la Dirección
                        string queryDireccion = @"UPDATE DireccionCliente 
                                                  SET paisCliente = @Pais, localidadCliente = @Localidad, provinciaCliente = @Provincia, 
                                                      calleCliente = @Calle, altura = @Altura 
                                                  WHERE idDireccionCliente = @IdDireccion";

                        SqlCommand cmdDir = new SqlCommand(queryDireccion, con, tran);
                        cmdDir.Parameters.AddWithValue("@IdDireccion", idDireccion);
                        cmdDir.Parameters.AddWithValue("@Pais", pais);
                        cmdDir.Parameters.AddWithValue("@Localidad", localidad);
                        cmdDir.Parameters.AddWithValue("@Provincia", provincia);
                        cmdDir.Parameters.AddWithValue("@Calle", calle);
                        cmdDir.Parameters.AddWithValue("@Altura", altura);
                        cmdDir.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool EliminarCliente(string dni)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Obtener el idDireccionCliente antes de borrar el cliente
                        string queryObtenerDir = "SELECT idDireccionCliente FROM Cliente WHERE dniCliente = @Dni";
                        SqlCommand cmdDir = new SqlCommand(queryObtenerDir, con, tran);
                        cmdDir.Parameters.AddWithValue("@Dni", dni);

                        object result = cmdDir.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            int idDireccion = Convert.ToInt32(result);

                            // 2. Eliminar el Cliente
                            string queryDelCliente = "DELETE FROM Cliente WHERE dniCliente = @Dni";
                            SqlCommand cmdDelCliente = new SqlCommand(queryDelCliente, con, tran);
                            cmdDelCliente.Parameters.AddWithValue("@Dni", dni);
                            cmdDelCliente.ExecuteNonQuery();

                            // 3. Eliminar la Dirección asociada
                            string queryDelDir = "DELETE FROM DireccionCliente WHERE idDireccionCliente = @IdDir";
                            SqlCommand cmdDelDir = new SqlCommand(queryDelDir, con, tran);
                            cmdDelDir.Parameters.AddWithValue("@IdDir", idDireccion);
                            cmdDelDir.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        tran.Rollback();
                        if (ex.Number == 547)
                            throw new Exception("No se puede eliminar el cliente porque tiene registros asociados (ej: ventas o pedidos).");

                        throw new Exception("Error en la base de datos al intentar eliminar el cliente.");
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}