using System;

namespace The_Good_Taste.Entidades
{
    public enum RolUsuario
    {
        Admin = 1,
        Gerente = 2,
        Vendedor = 3
    }

    public class UsuarioSistema
    {
        // Agregamos el DNI que es la clave primaria en la base de datos y la FK en Venta
        public int DNI { get; set; }

        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public RolUsuario Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }
}