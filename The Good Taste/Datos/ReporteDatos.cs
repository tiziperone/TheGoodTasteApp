using System;
using System.Data;
using System.Data.SqlClient;

namespace The_Good_Taste.Datos
{
    public class ReporteDatos
    {
        public DataTable ObtenerRecaudacionGlobal(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT CAST(fechaVenta AS DATE) AS Fecha, SUM(totalVenta) AS Recaudacion 
                             FROM Venta 
                             WHERE fechaVenta BETWEEN @Desde AND @Hasta
                             GROUP BY CAST(fechaVenta AS DATE) 
                             ORDER BY Fecha";
            return EjecutarConsulta(query, desde, hasta);
        }

        public DataTable ObtenerVentasGenerales(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT CAST(fechaVenta AS DATE) AS Fecha, COUNT(idVenta) AS CantidadVentas, SUM(totalVenta) AS Total
                             FROM Venta 
                             WHERE fechaVenta BETWEEN @Desde AND @Hasta
                             GROUP BY CAST(fechaVenta AS DATE) 
                             ORDER BY Fecha";
            return EjecutarConsulta(query, desde, hasta);
        }

        public DataTable ObtenerVentasPorVendedor(DateTime desde, DateTime hasta, string nombreVendedor)
        {
            string query = @"SELECT u.Nombre + ' ' + u.Apellido AS Vendedor, SUM(v.totalVenta) AS TotalVendido 
                             FROM Venta v 
                             INNER JOIN Usuarios u ON v.DNIUsuario = u.DNI 
                             WHERE v.fechaVenta BETWEEN @Desde AND @Hasta ";

            if (!string.IsNullOrEmpty(nombreVendedor))
                query += " AND (u.Nombre LIKE '%' + @NombreBuscado + '%' OR u.Apellido LIKE '%' + @NombreBuscado + '%') ";

            query += " GROUP BY u.Nombre, u.Apellido ORDER BY TotalVendido DESC";

            return EjecutarConsulta(query, desde, hasta, null, nombreVendedor);
        }

        public DataTable ObtenerTopProductosMasVendidos(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT TOP 5 p.Nombre AS Producto, SUM(vd.cantidad) AS CantidadVendida 
                             FROM VentaDetalle vd 
                             INNER JOIN Venta v ON vd.idVenta = v.idVenta
                             INNER JOIN Productos p ON vd.Codigo = p.Codigo 
                             WHERE v.fechaVenta BETWEEN @Desde AND @Hasta
                             GROUP BY p.Nombre 
                             ORDER BY CantidadVendida DESC";
            return EjecutarConsulta(query, desde, hasta);
        }

        // REPORTE DE TOP CLIENTES (Adaptado a tu DER)
        public DataTable ObtenerTopClientes(DateTime desde, DateTime hasta)
        {
            // Se usa "nombreCliente", "apellidoCliente" y "dniCliente" tal cual está en tu DER
            string query = @"SELECT TOP 5 c.nombreCliente + ' ' + c.apellidoCliente AS Cliente, SUM(v.totalVenta) AS TotalComprado 
                             FROM Venta v 
                             INNER JOIN Cliente c ON v.dniCliente = c.dniCliente 
                             WHERE v.fechaVenta BETWEEN @Desde AND @Hasta
                             GROUP BY c.nombreCliente, c.apellidoCliente 
                             ORDER BY TotalComprado DESC";

            return EjecutarConsulta(query, desde, hasta);
        }

        // EL ÚNICO REPORTE DEL VENDEDOR: Solo su propia recaudación
        public DataTable ObtenerMisVentasPorPeriodo(int dniVendedor, DateTime desde, DateTime hasta)
        {
            string query = @"SELECT CAST(fechaVenta AS DATE) AS Fecha, SUM(totalVenta) AS Recaudacion 
                             FROM Venta 
                             WHERE DNIUsuario = @Dni 
                             AND fechaVenta BETWEEN @Desde AND @Hasta
                             GROUP BY CAST(fechaVenta AS DATE) 
                             ORDER BY Fecha";
            return EjecutarConsulta(query, desde, hasta, dniVendedor);
        }

        private DataTable EjecutarConsulta(string query, DateTime desde, DateTime hasta, int? dni = null, string nombreBuscado = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Desde", desde);
                cmd.Parameters.AddWithValue("@Hasta", hasta);

                if (dni.HasValue)
                    cmd.Parameters.AddWithValue("@Dni", dni.Value);

                if (!string.IsNullOrEmpty(nombreBuscado))
                    cmd.Parameters.AddWithValue("@NombreBuscado", nombreBuscado);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}