using System;

namespace The_Good_Taste.Entidades
{
    public class UsuarioSistema
    {
        public int DNI { get; set; } // PK
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public int IdRol { get; set; } // FK a la tabla Roles
        public bool Activo { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Sexo { get; set; }

        public int? IdLocalidad { get; set; } // FK a Localidad (puede ser null)

        // Propiedad de ayuda en C# para mapear el IdRol fácilmente
        public RolUsuario Rol => (RolUsuario)IdRol;
    }

    public enum RolUsuario
    {
        Admin = 1,
        Gerente = 2,
        Vendedor = 3
    }
}