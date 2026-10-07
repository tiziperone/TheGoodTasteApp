namespace The_Good_Taste.Entidades
{
    public class VentaDetalle
    {
        public int IdDetalle { get; set; }
        public string Codigo { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        public int IdTipoPago { get; set; }
        public string NombreTipoPago { get; set; } 
    }
}