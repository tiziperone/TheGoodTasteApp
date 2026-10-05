using System;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class VentaDatos
    {
        public static bool RegistrarVenta(Venta venta)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                SqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    // Ajustado al DER: Tabla Venta, columnas fechaVenta, dniCliente, totalVenta
                    string queryVenta = @"INSERT INTO Venta (fechaVenta, dniCliente, totalVenta) 
                                          VALUES (@Fecha, @IdCliente, @Total);
                                          SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdVenta = new SqlCommand(queryVenta, con, transaccion);
                    cmdVenta.Parameters.AddWithValue("@Fecha", venta.Fecha);
                    cmdVenta.Parameters.AddWithValue("@IdCliente", venta.IdCliente); // Este es el dniCliente
                    cmdVenta.Parameters.AddWithValue("@Total", venta.Total);

                    int idVentaGenerado = Convert.ToInt32(cmdVenta.ExecuteScalar());

                    // Ajustado al DER: Tabla VentaDetalle, usando Codigo como identificador
                    foreach (var item in venta.Detalles)
                    {
                        string queryDetalle = @"INSERT INTO VentaDetalle (idVenta, Codigo, cantidad, precioUnitario) 
                                                VALUES (@IdVenta, @Codigo, @Cantidad, @PrecioUnitario);
                                                UPDATE Productos SET Stock = Stock - @Cantidad WHERE Codigo = @Codigo;";

                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, con, transaccion);
                        cmdDetalle.Parameters.AddWithValue("@IdVenta", idVentaGenerado);
                        cmdDetalle.Parameters.AddWithValue("@Codigo", item.Codigo);
                        cmdDetalle.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                        cmdDetalle.Parameters.AddWithValue("@PrecioUnitario", item.PrecioUnitario);

                        cmdDetalle.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    return true;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }
    }
}