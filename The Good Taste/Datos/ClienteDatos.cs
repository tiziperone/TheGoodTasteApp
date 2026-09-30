using System;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class ClienteDatos
    {
        public static Cliente ObtenerPorDni(string dni)
        {
            Cliente cliente = null;
            // Ajusté los campos según los nombres de columna de tus nuevas tablas para que coincida con el SELECT
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

        // Nuevo método para insertar los datos en las dos tablas relacionadas
        public void InsertarCliente(string dni, string nombre, string apellido, DateTime fechaNacimiento, string correo, string telefono, string pais, string localidad, string provincia, string calle, string altura)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();

                // Usamos una transacción para asegurar que ambas tablas se guarden correctamente o ninguna
                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar la Dirección y obtener el idDireccionCliente generado
                        string queryDireccion = @"INSERT INTO DireccionCliente (paisCliente, localidadCliente, provinciaCliente, calleCliente, altura) 
                                                  OUTPUT INSERTED.idDireccionCliente 
                                                  VALUES (@Pais, @Localidad, @Provincia, @Calle, @Altura)";

                        SqlCommand cmdDir = new SqlCommand(queryDireccion, con, tran);
                        cmdDir.Parameters.AddWithValue("@Pais", pais);
                        cmdDir.Parameters.AddWithValue("@Localidad", localidad);
                        cmdDir.Parameters.AddWithValue("@Provincia", provincia);
                        cmdDir.Parameters.AddWithValue("@Calle", calle);
                        cmdDir.Parameters.AddWithValue("@Altura", altura);

                        // Ejecutar y obtener el ID
                        int idDireccion = (int)cmdDir.ExecuteScalar();

                        // 2. Insertar el Cliente usando el idDireccionCliente que acabamos de obtener
                        string queryCliente = @"INSERT INTO Cliente (dniCliente, nombreCliente, apellidoCliente, fechaNaciminetoCliente, correoCliente, telefonoCliente, idDireccionCliente) 
                                                VALUES (@Dni, @Nombre, @Apellido, @FechaNac, @Correo, @Telefono, @IdDireccion)";

                        SqlCommand cmdCli = new SqlCommand(queryCliente, con, tran);
                        cmdCli.Parameters.AddWithValue("@Dni", dni);
                        cmdCli.Parameters.AddWithValue("@Nombre", nombre);
                        cmdCli.Parameters.AddWithValue("@Apellido", apellido);
                        cmdCli.Parameters.AddWithValue("@FechaNac", fechaNacimiento);
                        cmdCli.Parameters.AddWithValue("@Correo", correo);
                        cmdCli.Parameters.AddWithValue("@Telefono", telefono);
                        cmdCli.Parameters.AddWithValue("@IdDireccion", idDireccion);

                        // Ejecutar inserción de cliente
                        cmdCli.ExecuteNonQuery();

                        // Confirmar los cambios en la base de datos
                        tran.Commit();
                    }
                    catch (Exception)
                    {
                        // Si ocurre cualquier error, deshacer todos los cambios de esta transacción
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}