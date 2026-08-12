using System;

namespace The_Good_Taste.Entidades
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }

        // Domicilio integrado
        public string Calle { get; set; }
        public string Numero { get; set; }
        public string Barrio { get; set; }

        public DateTime FechaAlta { get; set; }
        public bool Activo { get; set; }
    }
}