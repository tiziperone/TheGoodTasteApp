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
                    // 1. Insertar cabecera de la venta
                    string queryVenta = @"INSERT INTO Ventas (Fecha, IdCliente, MetodoEnvio, DireccionEnvio, Total) 
                                          VALUES (@Fecha, @IdCliente, @MetodoEnvio, @DireccionEnvio, @Total);
                                          SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdVenta = new SqlCommand(queryVenta, con, transaccion);
                    cmdVenta.Parameters.AddWithValue("@Fecha", venta.Fecha);
                    cmdVenta.Parameters.AddWithValue("@IdCliente", venta.IdCliente);
                    cmdVenta.Parameters.AddWithValue("@MetodoEnvio", (object)venta.MetodoEnvio ?? DBNull.Value);
                    cmdVenta.Parameters.AddWithValue("@DireccionEnvio", (object)venta.DireccionEnvio ?? DBNull.Value);
                    cmdVenta.Parameters.AddWithValue("@Total", venta.Total);

                    int idVentaGenerado = Convert.ToInt32(cmdVenta.ExecuteScalar());

                    // 2. Insertar cada detalle y descontar stock
                    foreach (var item in venta.Detalles)
                    {
                        string queryDetalle = @"INSERT INTO VentaDetalles (IdVenta, IdProducto, Cantidad, PrecioUnitario) 
                                                VALUES (@IdVenta, @IdProducto, @Cantidad, @PrecioUnitario);
                                                UPDATE Productos SET Stock = Stock - @Cantidad WHERE IdProducto = @IdProducto;";

                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, con, transaccion);
                        cmdDetalle.Parameters.AddWithValue("@IdVenta", idVentaGenerado);
                        cmdDetalle.Parameters.AddWithValue("@IdProducto", item.IdProducto);
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