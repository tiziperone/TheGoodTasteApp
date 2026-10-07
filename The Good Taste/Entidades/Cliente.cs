using System;

namespace The_Good_Taste.Entidades
{
    public class Cliente
    {
        public string DniCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }

        public DateTime FechaNacimientoCliente { get; set; }

        public string CorreoCliente { get; set; }
        public string TelefonoCliente { get; set; }

        // Clave foránea hacia DireccionCliente
        public int IdDireccionCliente { get; set; }

        public bool Activo { get; set; }

        public DireccionCliente Direccion { get; set; }

    }
}