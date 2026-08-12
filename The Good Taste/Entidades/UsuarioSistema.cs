using System;

namespace The_Good_Taste.Entidades
{
    public class UsuarioSistema
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Rol { get; set; } // "admin" o "vendedor"
        public bool Activo { get; set; }
        public DateTime? LastSeenAt { get; set; }
    }
}