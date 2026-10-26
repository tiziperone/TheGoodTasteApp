using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class VentaDatos
    {
        // Obtiene únicamente productos activos (DeleteAt IS NULL)
        public static List<Producto> ObtenerProductosActivos()
        {
            List<Producto> lista = new List<Producto>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                string query = @"SELECT Codigo, Nombre, Descripcion, Precio, Stock, StockMinimo 
                                 FROM Productos 
                                 WHERE DeleteAt IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Producto
                        {
                            Codigo = dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : "",
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            Stock = Convert.ToInt32(dr["Stock"]),
                            StockMinimo = dr["StockMinimo"] != DBNull.Value ? Convert.ToInt32(dr["StockMinimo"]) : 0
                        });
                    }
                }
            }
            return lista;
        }

        public static int ObtenerStockActual(string codigo)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                string query = "SELECT Stock FROM Productos WHERE Codigo = @Codigo AND DeleteAt IS NULL";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Codigo", codigo);

                object result = cmd.ExecuteScalar();

                return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
            }
        }

        public static int RegistrarVenta(Venta venta)
        {
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                SqlTransaction transaccion = con.BeginTransaction();

                try
                {
                    // 1. Insertar el encabezado de la Venta y obtener el ID generado
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
                        string queryStock = @"UPDATE Productos 
                                              SET Stock = Stock - @Cantidad 
                                              WHERE Codigo = @Codigo AND Stock >= @Cantidad AND DeleteAt IS NULL;";

                        SqlCommand cmdStock = new SqlCommand(queryStock, con, transaccion);
                        cmdStock.Parameters.AddWithValue("@Codigo", item.Codigo);
                        cmdStock.Parameters.AddWithValue("@Cantidad", item.Cantidad);

                        int filasAfectadas = cmdStock.ExecuteNonQuery();

                        if (filasAfectadas == 0)
                        {
                            throw new Exception($"El producto '{item.Codigo}' no cuenta con suficiente stock o no se encuentra activo.");
                        }

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

                    transaccion.Commit();
                    return idVentaGenerado; // Retornamos el número de factura/venta
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    throw ex;
                }
            }
        }
    }
}