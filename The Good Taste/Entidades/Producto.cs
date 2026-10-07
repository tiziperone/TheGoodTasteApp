using System;

namespace The_Good_Taste.Entidades
{
    public class Producto
    {
        public string Codigo { get; set; } // PK
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }

        public int IdCategoria { get; set; } // FK
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? DeleteAt { get; set; }

        public decimal PrecioActual => Precio;

    }
}