using System;
using System.Collections.Generic;

namespace The_Good_Taste.Entidades
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public int IdCliente { get; set; }
        public Cliente Cliente { get; set; }
        public int IdUsuarioSistema { get; set; } // El Admin que cobra la venta
        public string MetodoEnvio { get; set; } // "retiro" o "delivery"
        public string DireccionEnvio { get; set; }
        public decimal Total { get; set; }

        public List<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }

    public class VentaDetalle
    {
        public int IdDetalle { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}