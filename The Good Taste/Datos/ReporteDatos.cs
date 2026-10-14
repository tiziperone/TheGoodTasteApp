using System;
using System.Data;
using System.Data.SqlClient;

namespace The_Good_Taste.Datos
{
    public class ReporteDatos
    {
        public DataTable ObtenerRecaudacionGlobal(DateTime desde, DateTime hasta)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerRecaudacionGlobal", con);
                cmd.CommandType = CommandType.StoredProcedure; // <--- Esto le avisa a C# que es un SP

                cmd.Parameters.AddWithValue("@Desde", desde);
                cmd.Parameters.AddWithValue("@Hasta", hasta);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
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

        public DataTable ObtenerVentasPorMedioPago(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT ISNULL(tp.nombreTipoPago, 'Efectivo') AS [Medio de Pago], 
                                    SUM(vd.cantidad * vd.precioUnitario) AS Total
                             FROM Venta v
                             INNER JOIN VentaDetalle vd ON v.idVenta = vd.idVenta
                             INNER JOIN TipoPago tp ON vd.idTipoPago = tp.idTipoPago
                             WHERE v.fechaVenta BETWEEN @Desde AND @Hasta
                             GROUP BY tp.nombreTipoPago
                             ORDER BY Total DESC";
            return EjecutarConsulta(query, desde, hasta);
        }

        public DataTable ObtenerTicketPromedioDiario(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT CAST(fechaVenta AS DATE) AS Fecha, 
                                    ROUND(AVG(totalVenta), 2) AS [Ticket Promedio]
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

        public DataTable ObtenerStockCritico()
        {
            string query = @"SELECT Nombre AS Producto, Stock, StockMinimo AS [Stock Mínimo], (StockMinimo - Stock) AS [Faltante]
                             FROM Productos
                             WHERE Stock <= StockMinimo
                             ORDER BY (StockMinimo - Stock) DESC";
            return EjecutarConsultaLibre(query);
        }


        public DataTable ObtenerDiaMasVendidoPorProducto(DateTime desde, DateTime hasta)
        {
            string query = @"SET LANGUAGE Spanish;
                     WITH VentasPorDia AS (
                        SELECT 
                            p.Nombre AS Producto,
                            DATENAME(dw, v.fechaVenta) AS DiaSemana,
                            CAST(v.fechaVenta AS DATE) AS FechaPico,
                            SUM(vd.cantidad) AS CantidadVendida,
                            ROW_NUMBER() OVER (PARTITION BY p.Nombre ORDER BY SUM(vd.cantidad) DESC) AS Ranking
                        FROM VentaDetalle vd
                        INNER JOIN Venta v ON vd.idVenta = v.idVenta
                        INNER JOIN Productos p ON vd.Codigo = p.Codigo
                        WHERE v.fechaVenta BETWEEN @Desde AND @Hasta
                        GROUP BY p.Nombre, DATENAME(dw, v.fechaVenta), CAST(v.fechaVenta AS DATE)
                     )
                     SELECT 
                        Producto, 
                        UPPER(LEFT(DiaSemana, 1)) + LOWER(SUBSTRING(DiaSemana, 2, LEN(DiaSemana))) + ' ' + CAST(FechaPico AS VARCHAR) + ' (' + CAST(CantidadVendida AS VARCHAR) + ' un.)' AS [Día Pico]
                     FROM VentasPorDia
                     WHERE Ranking = 1
                     ORDER BY CantidadVendida DESC";
            return EjecutarConsulta(query, desde, hasta);
        }

        public DataTable ObtenerTopClientes(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT TOP 5 c.nombreCliente + ' ' + c.apellidoCliente AS Cliente, SUM(v.totalVenta) AS TotalComprado 
                             FROM Venta v 
                             INNER JOIN Cliente c ON v.dniCliente = c.dniCliente 
                             WHERE v.fechaVenta BETWEEN @Desde AND @Hasta
                             GROUP BY c.nombreCliente, c.apellidoCliente 
                             ORDER BY TotalComprado DESC";

            return EjecutarConsulta(query, desde, hasta);
        }

        public DataTable ObtenerClientesFieles(DateTime desde, DateTime hasta)
        {
            string query = @"SELECT c.nombreCliente + ' ' + c.apellidoCliente AS Cliente, 
                                    COUNT(v.idVenta) AS [Total Compras], 
                                    SUM(v.totalVenta) AS [Total Gastado]
                             FROM Venta v
                             INNER JOIN Cliente c ON v.dniCliente = c.dniCliente
                             WHERE v.fechaVenta BETWEEN @Desde AND @Hasta
                             GROUP BY c.nombreCliente, c.apellidoCliente
                             HAVING COUNT(v.idVenta) > 1
                             ORDER BY [Total Compras] DESC";
            return EjecutarConsulta(query, desde, hasta);
        }

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

        private DataTable EjecutarConsultaLibre(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Conexion.ObtenerConexion())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}