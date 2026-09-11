using System;
using System.Configuration;
using System.Data.SqlClient;

namespace The_Good_Taste.Datos
{
    internal static class Conexion
    {
        // Obtiene la cadena de conexión dinámicamente desde el archivo App.config
        private static readonly string CadenaConexion =
            ConfigurationManager.ConnectionStrings["CadenaConexion"]?.ConnectionString
            ?? throw new InvalidOperationException("No se encontró la cadena 'CadenaConexion' en el archivo App.config.");

        internal static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(CadenaConexion);
        }
    }
}