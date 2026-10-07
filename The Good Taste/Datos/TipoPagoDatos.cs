using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class TipoPagoDatos
    {
        public List<TipoPago> ObtenerTiposPago()
        {
            List<TipoPago> lista = new List<TipoPago>();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                con.Open();
                string query = "SELECT idTipoPago, nombreTipoPago FROM TipoPago";
                SqlCommand cmd = new SqlCommand(query, con);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new TipoPago
                        {
                            IdTipoPago = Convert.ToInt32(reader["idTipoPago"]),
                            NombreTipoPago = reader["nombreTipoPago"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
    }
}