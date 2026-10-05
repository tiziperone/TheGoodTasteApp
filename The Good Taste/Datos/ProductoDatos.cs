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
            string query = @"SELECT Codigo, Nombre, Descripcion, Precio, Stock, StockMinimo, IdCategoria 
                             FROM Productos 
                             WHERE DeleteAt IS NULL";

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

        public static List<Producto> ObtenerInactivos()
        {
            List<Producto> lista = new List<Producto>();
            string query = @"SELECT Codigo, Nombre, Descripcion, Precio, Stock, StockMinimo, IdCategoria 
                             FROM Productos 
                             WHERE DeleteAt IS NOT NULL";

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
            string query = @"INSERT INTO Productos (Codigo, Nombre, Descripcion, Precio, Stock, StockMinimo, IdCategoria, CreateAt) 
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

        public static bool Actualizar(Producto prod)
        {
            string query = @"UPDATE Productos 
                             SET Nombre = @Nombre, 
                                 Descripcion = @Descripcion, 
                                 Precio = @Precio, 
                                 Stock = @Stock, 
                                 StockMinimo = @StockMinimo, 
                                 IdCategoria = @IdCategoria
                             WHERE Codigo = @Codigo AND DeleteAt IS NULL";

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

        public static bool Eliminar(string codigo)
        {
            string query = "UPDATE Productos SET DeleteAt = GETDATE() WHERE Codigo = @Codigo";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Codigo", codigo);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Nuevo método para restaurar el producto
        public static bool Activar(string codigo)
        {
            string query = "UPDATE Productos SET DeleteAt = NULL WHERE Codigo = @Codigo";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Codigo", codigo);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}