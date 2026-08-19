using System.Data.SqlClient;

namespace The_Good_Taste.Datos
{
    internal static class Conexion
    {
        // Si tu instancia de SQL Server es SQLEXPRESS, cambia "Data Source=." por "Data Source=.\\SQLEXPRESS"
        private static readonly string CadenaConexion = "Data Source=.;Initial Catalog=TheGoodTasteDB;Integrated Security=True;TrustServerCertificate=True;";

        internal static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}