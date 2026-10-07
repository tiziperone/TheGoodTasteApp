using System;
using System.Collections.Generic;

namespace The_Good_Taste.Entidades
{
    public class Venta
    {
        public int IdVenta { get; set; } // PK
        public DateTime FechaVenta { get; set; }
        public decimal TotalVenta { get; set; }

        public int DNIUsuario { get; set; } // FK a Usuarios
        public string DniCliente { get; set; } // FK a Cliente (mismo tipo que la PK de Cliente)

        // Relaciones en memoria (C#)
        public Cliente Cliente { get; set; }

        public List<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}