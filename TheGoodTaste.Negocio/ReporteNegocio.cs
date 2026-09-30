using System.Data;
using The_Good_Taste.Datos;

namespace TheGoodTaste.Negocio
{
    public class ReporteNegocio
    {
        private readonly ReporteDatos _datos = new ReporteDatos();

        public DataTable GenerarReporteGerente(string tipoReporte)
        {
            switch (tipoReporte)
            {
                case "Recaudación Global":
                    return _datos.ObtenerRecaudacionGlobal();
                case "Ventas por Vendedor":
                    return _datos.ObtenerVentasPorVendedor();
                case "Top Productos Más Vendidos":
                    return _datos.ObtenerTopProductosMasVendidos();
                default:
                    return new DataTable();
            }
        }

        public DataTable GenerarReporteVendedor(string tipoReporte, int dniVendedor)
        {
            switch (tipoReporte)
            {
                case "Mis Ventas por Período":
                    return _datos.ObtenerMisVentasPorPeriodo(dniVendedor);
                case "Mis Productos Más Vendidos":
                    return _datos.ObtenerMisProductosMasVendidos(dniVendedor);
                case "Mis Ventas por Categoría":
                    return _datos.ObtenerMisVentasPorCategoria(dniVendedor);
                default:
                    return new DataTable();
            }
        }
    }
}