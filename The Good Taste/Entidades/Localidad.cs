namespace The_Good_Taste.Entidades
{
    public class Localidad
    {
        public int IdLocalidad { get; set; } // PK
        public string Nombre { get; set; }
        public string Provincia { get; set; }
        public string CodigoPostal { get; set; }
    }
}