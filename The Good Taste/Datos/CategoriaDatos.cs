using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class CategoriaDatos
    {
        public static List<Categoria> ObtenerTodas()
        {
            List<Categoria> lista = new List<Categoria>();
            string query = "SELECT idCategoria, nombreCategoria FROM Categoria";

            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Categoria
                        {
                            IdCategoria = Convert.ToInt32(dr["idCategoria"]),
                            NombreCategoria = dr["nombreCategoria"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}