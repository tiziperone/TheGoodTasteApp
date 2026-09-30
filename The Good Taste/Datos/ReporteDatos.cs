using System;
using System.Data;
using System.Data.SqlClient;

namespace The_Good_Taste.Datos
{
    public class ReporteDatos
    {

        public DataTable ObtenerRecaudacionGlobal()
        {
            // Agrupa por fecha de venta
            string query = @"SELECT CAST(fechaVenta AS DATE) AS Fecha, SUM(totalVenta) AS Recaudacion 
                             FROM Venta 
                             GROUP BY CAST(fechaVenta AS DATE) 
                             ORDER BY Fecha";
            return EjecutarConsulta(query);
        }

        public DataTable ObtenerVentasPorVendedor()
        {
            // Cruza Venta con Usuarios por el DNIUsuario
            string query = @"SELECT u.Nombre + ' ' + u.Apellido AS Vendedor, SUM(v.totalVenta) AS TotalVendido 
                             FROM Venta v 
                             INNER JOIN Usuarios u ON v.DNIUsuario = u.DNI 
                             GROUP BY u.Nombre, u.Apellido 
                             ORDER BY TotalVendido DESC";
            return EjecutarConsulta(query);
        }

        public DataTable ObtenerTopProductosMasVendidos()
        {
            // Cruza VentaDetalle con Productos para obtener el Nombre del producto
            string query = @"SELECT TOP 5 p.Nombre AS Producto, SUM(vd.cantidad) AS CantidadVendida 
                             FROM VentaDetalle vd 
                             INNER JOIN Productos p ON vd.Codigo = p.Codigo 
                             GROUP BY p.Nombre 
                             ORDER BY CantidadVendida DESC";
            return EjecutarConsulta(query);
        }

        public DataTable ObtenerMisVentasPorPeriodo(int dniVendedor)
        {
            string query = @"SELECT CAST(fechaVenta AS DATE) AS Fecha, SUM(totalVenta) AS Recaudacion 
                             FROM Venta 
                             WHERE DNIUsuario = @Dni 
                             GROUP BY CAST(fechaVenta AS DATE) 
                             ORDER BY Fecha";
            return EjecutarConsulta(query, dniVendedor);
        }

        public DataTable ObtenerMisProductosMasVendidos(int dniVendedor)
        {
            string query = @"SELECT TOP 5 p.Nombre AS Producto, SUM(vd.cantidad) AS Cantidad 
                             FROM VentaDetalle vd 
                             INNER JOIN Venta v ON vd.idVenta = v.idVenta 
                             INNER JOIN Productos p ON vd.Codigo = p.Codigo 
                             WHERE v.DNIUsuario = @Dni 
                             GROUP BY p.Nombre 
                             ORDER BY Cantidad DESC";
            return EjecutarConsulta(query, dniVendedor);
        }

        public DataTable ObtenerMisVentasPorCategoria(int dniVendedor)
        {
            // Cruza hasta Categorias para un gráfico de dona
            string query = @"SELECT c.nombreCategoria AS Categoria, SUM(vd.cantidad * vd.precioUnitario) AS Recaudacion 
                             FROM VentaDetalle vd 
                             INNER JOIN Venta v ON vd.idVenta = v.idVenta 
                             INNER JOIN Productos p ON vd.Codigo = p.Codigo 
                             INNER JOIN Categoria c ON p.IdCategoria = c.idCategoria 
                             WHERE v.DNIUsuario = @Dni 
                             GROUP BY c.nombreCategoria 
                             ORDER BY Recaudacion DESC";
            return EjecutarConsulta(query, dniVendedor);
        }

        private DataTable EjecutarConsulta(string query, int? dni = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                if (dni.HasValue)
                {
                    cmd.Parameters.AddWithValue("@Dni", dni.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}