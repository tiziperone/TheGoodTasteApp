namespace The_Good_Taste.Entidades
{
    public class VentaDetalle
    {
        public int IdDetalle { get; set; }
        public int IdVenta { get; set; }

        // IMPORTANTE: Se usa el Código (string) en lugar de un ID numérico
        public string Codigo { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Relaciones y cálculos lógicos
        public Producto Producto { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}