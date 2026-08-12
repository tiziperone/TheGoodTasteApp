using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Good_Taste.Datos
{
    internal class Conexion
    {
        // Cadena de conexión interna para el servidor local
        private static string cadenaConexion = "Data Source=.;Initial Catalog=TheGoodTasteDB;Integrated Security=True";

        internal static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}

