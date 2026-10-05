using System;
using System.Collections.Generic;

namespace The_Good_Taste.Entidades
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public DateTime Fecha { get; set; }
        public int IdCliente { get; set; } // Representa el dniCliente
        public int DNIUsuario { get; set; } // El usuario que cobra la venta
        public decimal Total { get; set; }

        // Relaciones
        public Cliente Cliente { get; set; }
        public List<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}