using System;
using System.Data;
using The_Good_Taste.Datos;

namespace TheGoodTaste.Negocio
{
    public class ReporteNegocio
    {
        private readonly ReporteDatos _datos = new ReporteDatos();

        // 1. VENTAS Y FACTURACIÓN (GERENCIA)
        public DataTable GenerarRecaudacionGlobal(DateTime desde, DateTime hasta) => _datos.ObtenerRecaudacionGlobal(desde, hasta);
        public DataTable GenerarVentasGenerales(DateTime desde, DateTime hasta) => _datos.ObtenerVentasGenerales(desde, hasta);
        public DataTable GenerarVentasPorMedioPago(DateTime desde, DateTime hasta) => _datos.ObtenerVentasPorMedioPago(desde, hasta);
        public DataTable GenerarTicketPromedio(DateTime desde, DateTime hasta) => _datos.ObtenerTicketPromedioDiario(desde, hasta);
        public DataTable GenerarVentasPorVendedor(DateTime desde, DateTime hasta, string nombreVendedor) => _datos.ObtenerVentasPorVendedor(desde, hasta, nombreVendedor);

        // 2. INVENTARIO Y PRODUCTOS (GERENCIA)
        public DataTable GenerarTopProductos(DateTime desde, DateTime hasta) => _datos.ObtenerTopProductosMasVendidos(desde, hasta);
        public DataTable GenerarStockCritico() => _datos.ObtenerStockCritico();
        public DataTable GenerarDiaMasVendidoPorProducto(DateTime desde, DateTime hasta) => _datos.ObtenerDiaMasVendidoPorProducto(desde, hasta);

        // 3. FIDELIZACIÓN Y CLIENTES (GERENCIA)
        public DataTable GenerarTopClientes(DateTime desde, DateTime hasta) => _datos.ObtenerTopClientes(desde, hasta);
        public DataTable GenerarClientesFieles(DateTime desde, DateTime hasta) => _datos.ObtenerClientesFieles(desde, hasta);

        // REPORTE EXCLUSIVO DEL VENDEDOR
        public DataTable GenerarMisVentas(int dniVendedor, DateTime desde, DateTime hasta) => _datos.ObtenerMisVentasPorPeriodo(dniVendedor, desde, hasta);
    }
}