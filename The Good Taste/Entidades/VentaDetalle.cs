namespace The_Good_Taste.Entidades
{
    public class VentaDetalle
    {
        public int IdDetalle { get; set; } // PK
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public int IdVenta { get; set; } // FK
        public string Codigo { get; set; } // FK a Producto

        // Relaciones y cálculos
        public Producto Producto { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}