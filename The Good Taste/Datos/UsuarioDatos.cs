using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Datos
{
    public class UsuarioDatos
    {
        public UsuarioSistema Autenticar(string user, string pass)// solo temporalmente hasta conectar con sql
        {
            if(user == "admin" && pass == "1234")
                return new UsuarioSistema { IdUsuario = 1, NombreUsuario = "admin", Rol = RolUsuario.Admin };
            if (user == "gerente" && pass == "1234")
                return new UsuarioSistema { IdUsuario = 2, NombreUsuario = "gerente", Rol = RolUsuario.Gerente };
            if (user == "vendedor" && pass == "1234")
                return new UsuarioSistema { IdUsuario = 3, NombreUsuario = "vendedor", Rol = RolUsuario.Vendedor };
            return null; //para credenciales incorrectas
        }
    }
}
