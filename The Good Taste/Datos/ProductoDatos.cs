using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class ProductoDatos
    {
        public static List<Producto> ObtenerActivos()
        {
            List<Producto> lista = new List<Producto>();

            string query = @"SELECT IdProducto, Codigo, Nombre, Descripcion, Precio, Stock, StockMinimo, IdCategoria 
                             FROM Productos 
                             WHERE DeletedAt IS NULL";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Producto
                        {
                            IdProducto = Convert.ToInt32(dr["IdProducto"]),
                            Codigo = dr["Codigo"].ToString(),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"] != DBNull.Value ? dr["Descripcion"].ToString() : string.Empty,
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            Stock = Convert.ToInt32(dr["Stock"]),
                            StockMinimo = Convert.ToInt32(dr["StockMinimo"]),
                            IdCategoria = Convert.ToInt32(dr["IdCategoria"])
                        });
                    }
                }
            }
            return lista;
        }

        public static bool Insertar(Producto prod)
        {
            string query = @"INSERT INTO Productos (Codigo, Nombre, Descripcion, Precio, Stock, StockMinimo, IdCategoria, CreatedAt) 
                             VALUES (@Codigo, @Nombre, @Descripcion, @Precio, @Stock, @StockMinimo, @IdCategoria, GETDATE())";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Codigo", prod.Codigo);
                cmd.Parameters.AddWithValue("@Nombre", prod.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object)prod.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Precio", prod.Precio);
                cmd.Parameters.AddWithValue("@Stock", prod.Stock);
                cmd.Parameters.AddWithValue("@StockMinimo", prod.StockMinimo);
                cmd.Parameters.AddWithValue("@IdCategoria", prod.IdCategoria);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool Eliminar(int idProducto)
        {
            string query = "UPDATE Productos SET DeletedAt = GETDATE() WHERE IdProducto = @IdProducto";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@IdProducto", idProducto);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}