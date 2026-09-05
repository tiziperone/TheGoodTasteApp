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
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public RolUsuario Rol { get; set; } // "admin" o "vendedor"
        public bool Activo { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }
}