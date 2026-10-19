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
                    // 1. Insertar el encabezado de la Venta
                    string queryVenta = @"INSERT INTO Venta (fechaVenta, dniCliente, DNIUsuario, totalVenta) 
                                  VALUES (@Fecha, @IdCliente, @DNIUsuario, @Total);
                                  SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdVenta = new SqlCommand(queryVenta, con, transaccion);
                    cmdVenta.Parameters.AddWithValue("@Fecha", venta.FechaVenta);
                    cmdVenta.Parameters.AddWithValue("@IdCliente", venta.DniCliente);
                    cmdVenta.Parameters.AddWithValue("@DNIUsuario", venta.DNIUsuario);
                    cmdVenta.Parameters.AddWithValue("@Total", venta.TotalVenta);

                    int idVentaGenerado = Convert.ToInt32(cmdVenta.ExecuteScalar());

                    // 2. Recorrer cada producto del carrito
                    foreach (var item in venta.Detalles)
                    {
                        // Descontar stock con verificación atómica
                        string queryStock = @"UPDATE Productos 
                                      SET Stock = Stock - @Cantidad 
                                      WHERE Codigo = @Codigo AND Stock >= @Cantidad;";

                        SqlCommand cmdStock = new SqlCommand(queryStock, con, transaccion);
                        cmdStock.Parameters.AddWithValue("@Codigo", item.Codigo);
                        cmdStock.Parameters.AddWithValue("@Cantidad", item.Cantidad);

                        int filasAfectadas = cmdStock.ExecuteNonQuery();

                        // Si filasAfectadas == 0 significa que en ese milisegundo otra caja se llevó el stock
                        if (filasAfectadas == 0)
                        {
                            throw new Exception($"El producto '{item.Codigo}' ya no cuenta con suficiente stock debido a una venta simultánea.");
                        }

                        // Insertar el detalle de la venta
                        string queryDetalle = @"INSERT INTO VentaDetalle (idVenta, Codigo, cantidad, precioUnitario, idTipoPago) 
                                        VALUES (@IdVenta, @Codigo, @Cantidad, @Precio, @IdTipoPago);";

                        SqlCommand cmdDetalle = new SqlCommand(queryDetalle, con, transaccion);
                        cmdDetalle.Parameters.AddWithValue("@IdVenta", idVentaGenerado);
                        cmdDetalle.Parameters.AddWithValue("@Codigo", item.Codigo);
                        cmdDetalle.Parameters.AddWithValue("@Cantidad", item.Cantidad);
                        cmdDetalle.Parameters.AddWithValue("@Precio", item.PrecioUnitario);
                        cmdDetalle.Parameters.AddWithValue("@IdTipoPago", item.IdTipoPago);

                        cmdDetalle.ExecuteNonQuery();
                    }

                    // Si no hubo errores en ningún producto, confirmamos la transacción
                    transaccion.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Si algo falla o salta la excepción de stock, deshacemos todo
                    transaccion.Rollback();
                    throw ex; // Re-lanzamos la excepción para que llegue a la UI y muestre el MessageBox
                }
            }
        }
    }
}