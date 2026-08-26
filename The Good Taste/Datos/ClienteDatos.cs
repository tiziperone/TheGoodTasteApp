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
            string query = "SELECT IdCliente, Dni, Nombre, Apellido, Telefono, Email, Calle, Numero, Barrio FROM Clientes WHERE Dni = @Dni AND Activo = 1";

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
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Dni = dr["Dni"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Apellido = dr["Apellido"].ToString(),
                            Telefono = dr["Telefono"].ToString(),
                            Email = dr["Email"].ToString(),
                            Calle = dr["Calle"].ToString(),
                            Numero = dr["Numero"].ToString()

                        };
                    }
                }
            }
            return cliente;
        }
    }
}