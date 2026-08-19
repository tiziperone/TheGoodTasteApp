using System;

namespace The_Good_Taste.Entidades
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int StockMinimo { get; set; }
        public string UrlImagen { get; set; }
        public int IdCategoria { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }

        // Propiedad de lectura para compatibilidad si la lógica requiere "PrecioActual"
        public decimal PrecioActual => Precio;
    }
}