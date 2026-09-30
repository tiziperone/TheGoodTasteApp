using System;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class ClienteDatos
    {
        // Método para validar que no se repitan DNI, Correo o Teléfono
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
                        // Identificamos exactamente qué dato está duplicado para informarle al usuario
                        if (dr["dniCliente"].ToString() == dni) return "DNI";
                        if (dr["correoCliente"].ToString() == email) return "Correo electrónico";
                        if (dr["telefonoCliente"].ToString() == telefono) return "Teléfono";
                    }
                }
            }
            return string.Empty; // Retorna vacío si no hay duplicados
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
    }
}