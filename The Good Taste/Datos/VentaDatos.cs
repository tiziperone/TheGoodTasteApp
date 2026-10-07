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
                    // Nombres exactos de tu DER: Tabla Venta, columnas fechaVenta, dniCliente, DNIUsuario, totalVenta
                    string queryVenta = @"INSERT INTO Venta (fechaVenta, dniCliente, DNIUsuario, totalVenta) 
                                          VALUES (@Fecha, @IdCliente, @DNIUsuario, @Total);
                                          SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdVenta = new SqlCommand(queryVenta, con, transaccion);
                    cmdVenta.Parameters.AddWithValue("@Fecha", venta.FechaVenta);
                    cmdVenta.Parameters.AddWithValue("@IdCliente", venta.DniCliente);
                    cmdVenta.Parameters.AddWithValue("@DNIUsuario", venta.DNIUsuario);
                    cmdVenta.Parameters.AddWithValue("@Total", venta.TotalVenta);

                    int idVentaGenerado = Convert.ToInt32(cmdVenta.ExecuteScalar());

                    foreach (var item in venta.Detalles)
                    {
                        // 1. DESCONTAR STOCK CON CONDICIÓN DE CONCURRENCIA
                        // El 'AND Stock >= @Cantidad' evita la condición de carrera
                        string queryStock = @"UPDATE Productos 
                                              SET Stock = Stock - @Cantidad 
                                              WHERE Codigo = @Codigo AND Stock >= @Cantidad;";

                        SqlCommand cmdStock = new SqlCommand(queryStock, con, transaccion);
                        cmdStock.Parameters.AddWithValue("@Codigo", item.Codigo);
                        cmdStock.Parameters.AddWithValue("@Cantidad", item.Cantidad);

                        int filasAfectadas = cmdStock.ExecuteNonQuery();

                        // Si devuelve 0, significa que otro cajero vendió el producto un instante antes
                        if (filasAfectadas == 0)
                        {
                            transaccion.Rollback();
                            throw new Exception($"Conflicto de concurrencia: El producto código '{item.Codigo}' se quedó sin stock suficiente.");
                        }

                        // 2. REGISTRAR DETALLE DE VENTA
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
                    transaccion.Rollback();
                    throw;
                }
            }
        }
    }
}