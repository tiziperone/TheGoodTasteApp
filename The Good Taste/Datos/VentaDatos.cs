using System;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class VentaDatos
    {
        public static int ObtenerStockActual(string codigo)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                string query = "SELECT Stock FROM Productos WHERE Codigo = @Codigo";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Codigo", codigo);

                object result = cmd.ExecuteScalar();

                return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
            }
        }

        public static bool RegistrarVenta(Venta venta)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                SqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    // --- CORRECCIÓN: Se agregó idTipoPago en el INSERT y en los VALUES ---
                    string queryVenta = @"INSERT INTO Venta (fechaVenta, dniCliente, DNIUsuario, totalVenta, idTipoPago) 
                                          VALUES (@Fecha, @IdCliente, @DNIUsuario, @Total, @IdTipoPago);
                                          SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdVenta = new SqlCommand(queryVenta, con, transaccion);
                    cmdVenta.Parameters.AddWithValue("@Fecha", venta.FechaVenta);
                    cmdVenta.Parameters.AddWithValue("@IdCliente", venta.DniCliente);
                    cmdVenta.Parameters.AddWithValue("@DNIUsuario", venta.DNIUsuario);
                    cmdVenta.Parameters.AddWithValue("@Total", venta.TotalVenta);

                    // --- CORRECCIÓN: Se agrega el parámetro con el valor que viene de la UI ---
                    cmdVenta.Parameters.AddWithValue("@IdTipoPago", venta.IdTipoPago);

                    int idVentaGenerado = Convert.ToInt32(cmdVenta.ExecuteScalar());

                    foreach (var item in venta.Detalles)
                    {
                        string queryStock = @"UPDATE Productos 
                                              SET Stock = Stock - @Cantidad 
                                              WHERE Codigo = @Codigo AND Stock >= @Cantidad;";

                        SqlCommand cmdStock = new SqlCommand(queryStock, con, transaccion);
                        cmdStock.Parameters.AddWithValue("@Codigo", item.Codigo);
                        cmdStock.Parameters.AddWithValue("@Cantidad", item.Cantidad);

                        int filasAfectadas = cmdStock.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            throw new Exception($"El producto con código '{item.Codigo}' ya no cuenta con suficiente stock debido a una venta simultánea.");
                        }

                        string queryDetalle = @"INSERT INTO VentaDetalle (idVenta, Codigo, cantidad, precioUnitario) 
                                                VALUES (@IdVenta, @Codigo, @Cantidad, @PrecioUnitario);";

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
                    if (transaccion != null && transaccion.Connection != null)
                    {
                        transaccion.Rollback();
                    }
                    throw;
                }
            }
        }
    }
}