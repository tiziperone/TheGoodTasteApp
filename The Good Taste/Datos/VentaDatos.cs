using System;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class VentaDatos
    {
        // --- CORRECCIÓN: Método agregado para leer el stock directo de la BD ---
        public static int ObtenerStockActual(string codigo)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                string query = "SELECT Stock FROM Productos WHERE Codigo = @Codigo";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Codigo", codigo);

                object result = cmd.ExecuteScalar();

                // Retorna el stock actual, o 0 si por alguna razón no encuentra el registro
                return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
            }
        }
        // -----------------------------------------------------------------------

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

                        // Si devuelve 0, sólo lanzamos la excepción sin hacer Rollback acá
                        if (filasAfectadas == 0)
                        {
                            throw new Exception($"El producto con código '{item.Codigo}' ya no cuenta con suficiente stock debido a una venta simultánea.");
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
                    // El Rollback se centraliza una sola vez acá para cancelar todo de forma segura
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