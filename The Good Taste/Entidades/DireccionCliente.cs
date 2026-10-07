namespace The_Good_Taste.Entidades
{
    public class DireccionCliente
    {
        // Clave primaria
        public int IdDireccionCliente { get; set; }

        public string PaisCliente { get; set; }
        public string LocalidadCliente { get; set; }
        public string ProvinciaCliente { get; set; }
        public string CalleCliente { get; set; }
        public string Altura { get; set; } 
    }
}