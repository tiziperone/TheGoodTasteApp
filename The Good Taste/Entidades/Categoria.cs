namespace The_Good_Taste.Entidades
{
    public class Categoria //Clase que representa una categoría de productos en el sistema
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } // "Bondiolas", "Milanesas", "Pastas"
        public string Descripcion { get; set; }
    }
}