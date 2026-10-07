using System;
using System.Data;
using The_Good_Taste.Datos;

namespace TheGoodTaste.Negocio
{
    public class ReporteNegocio
    {
        private readonly ReporteDatos _datos = new ReporteDatos();

        // Métodos de Gerente
        public DataTable GenerarRecaudacionGlobal(DateTime desde, DateTime hasta)
        {
            return _datos.ObtenerRecaudacionGlobal(desde, hasta);
        }

        public DataTable GenerarVentasGenerales(DateTime desde, DateTime hasta)
        {
            return _datos.ObtenerVentasGenerales(desde, hasta);
        }

        public DataTable GenerarVentasPorVendedor(DateTime desde, DateTime hasta, string nombreVendedor)
        {
            return _datos.ObtenerVentasPorVendedor(desde, hasta, nombreVendedor);
        }

        public DataTable GenerarTopProductos(DateTime desde, DateTime hasta)
        {
            return _datos.ObtenerTopProductosMasVendidos(desde, hasta);
        }

        // Métodos de Vendedor (Solo lo que él vendió)
        public DataTable GenerarMisVentas(int dniVendedor, DateTime desde, DateTime hasta)
        {
            return _datos.ObtenerMisVentasPorPeriodo(dniVendedor, desde, hasta);
        }
    }
}