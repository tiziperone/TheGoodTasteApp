using System;
using System.Data;
using System.Text.RegularExpressions;
using The_Good_Taste.Datos;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.Negocio
{
    public class UsuarioNegocio //Clase que maneja la lógica de negocio relacionada con los usuarios del sistema
    {
        private readonly UsuarioDatos _repo = new UsuarioDatos();

        public UsuarioSistema Autenticar(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new Exception("Por favor, ingrese su nombre de usuario.");
            if (string.IsNullOrWhiteSpace(password)) throw new Exception("Por favor, ingrese su contraseña.");

            UsuarioSistema user = _repo.Autenticar(username, password);
            if (user == null) throw new Exception("Usuario o contraseña incorrectos.");

            return user;
        }

        public DataTable ObtenerUsuariosPorEstado(bool activos) => _repo.ObtenerUsuariosPorEstado(activos);
        public DataRow ObtenerUsuarioPorDNI(int dni) => _repo.ObtenerUsuarioPorDNI(dni);
        public DataTable ObtenerLocalidades() => _repo.ObtenerLocalidades();
        public bool CambiarEstadoUsuario(int dni, bool nuevoEstado) => _repo.CambiarEstadoUsuario(dni, nuevoEstado);

        public bool ExisteDNI(int dni) => _repo.ExisteDNI(dni);
        public bool ExisteUsuario(string username) => _repo.ExisteUsuario(username);
        public bool ExisteEmail(string email) => _repo.ExisteEmail(email);
        public bool ExisteTelefono(string telefono) => _repo.ExisteTelefono(telefono);

        // NUEVO MÉTODO: Validación y llamada para registrar nueva localidad separando la provincia
        public int RegistrarYObtenerIdLocalidad(string textoIngresado)
        {
            if (string.IsNullOrWhiteSpace(textoIngresado))
                throw new Exception("El nombre de la localidad no puede estar vacío.");

            textoIngresado = textoIngresado.Trim();
            string nombre = textoIngresado;
            string provincia = "Corrientes"; // Provincia por defecto si el usuario no escribe paréntesis

            // Buscar si el texto tiene el formato "Localidad (Provincia)"
            var match = Regex.Match(textoIngresado, @"^(.*?)\s*\((.*?)\)$");
            if (match.Success)
            {
                nombre = match.Groups[1].Value.Trim();
                provincia = match.Groups[2].Value.Trim();
            }

            return _repo.RegistrarYObtenerIdLocalidad(nombre, provincia);
        }

        public bool RegistrarUsuario(int dni, string username, string password, int idRol, string nombre, string apellido, string direccion, int idLocalidad, DateTime fechaNacimiento, string telefono, string email, string sexo)
        {
            return _repo.RegistrarUsuario(dni, username, password, idRol, nombre, apellido, direccion, idLocalidad, fechaNacimiento, telefono, email, sexo);
        }

        public bool ActualizarUsuario(int dni, string username, string password, int idRol, string nombre, string apellido, string direccion, int idLocalidad, DateTime fechaNacimiento, string telefono, string email, string sexo)
        {
            return _repo.ActualizarUsuario(dni, username, password, idRol, nombre, apellido, direccion, idLocalidad, fechaNacimiento, telefono, email, sexo);
        }

        public void GuardarUsuario(int? idSeleccionado, string dniTexto, string username, string nombre,
                                   string apellido, int idRol, string direccion, int idLocalidad,
                                   DateTime fechaNacimiento, string telefono, string email, string sexo,
                                   DataRow datosOriginales)
        {
            if (string.IsNullOrWhiteSpace(dniTexto) || dniTexto.Length < 7 || dniTexto.Length > 8 || !int.TryParse(dniTexto, out int dni))
                throw new Exception("El DNI debe contener 7 u 8 dígitos numéricos.");

            if (string.IsNullOrWhiteSpace(nombre)) throw new Exception("El nombre es un campo obligatorio.");
            if (string.IsNullOrWhiteSpace(apellido)) throw new Exception("El apellido es un campo obligatorio.");
            if (string.IsNullOrWhiteSpace(username)) throw new Exception("El nombre de usuario es obligatorio.");

            if (!Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new Exception("El correo electrónico no tiene un formato válido.");

            if (string.IsNullOrWhiteSpace(telefono) || telefono.Length < 10)
                throw new Exception("El número de teléfono debe tener al menos 10 dígitos (código de área + número).");

            DateTime fechaHoy = DateTime.Today;
            int edad = fechaHoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > fechaHoy.AddYears(-edad)) edad--;
            if (edad < 18) throw new Exception("El usuario debe ser mayor de edad (mínimo 18 años).");

            if (!idSeleccionado.HasValue)
            {
                if (_repo.ExisteDNI(dni)) throw new Exception("El DNI ingresado ya está registrado.");
                if (_repo.ExisteUsuario(username)) throw new Exception("El nombre de usuario autogenerado ya está en uso.");
                if (_repo.ExisteEmail(email)) throw new Exception("El correo electrónico ya se encuentra registrado.");
                if (_repo.ExisteTelefono(telefono)) throw new Exception("El número de teléfono ya está asociado a otro usuario.");

                if (!_repo.RegistrarUsuario(dni, username, dni.ToString(), idRol, nombre, apellido, direccion, idLocalidad, fechaNacimiento, telefono, email, sexo))
                    throw new Exception("No se pudo registrar el usuario en la base de datos.");
            }
            else
            {
                string usernameOrig = datosOriginales["Username"].ToString();
                string emailOrig = datosOriginales["Email"].ToString();
                string telOrig = datosOriginales["Telefono"]?.ToString() ?? "";
                string passwordActual = datosOriginales["PasswordHash"].ToString();

                if (username != usernameOrig && _repo.ExisteUsuario(username)) throw new Exception("El nuevo nombre de usuario ya está siendo utilizado.");
                if (email != emailOrig && _repo.ExisteEmail(email)) throw new Exception("El nuevo correo electrónico ya está registrado.");
                if (telefono != telOrig && _repo.ExisteTelefono(telefono)) throw new Exception("El nuevo número de teléfono ya pertenece a otro usuario.");

                if (!_repo.ActualizarUsuario(dni, username, passwordActual, idRol, nombre, apellido, direccion, idLocalidad, fechaNacimiento, telefono, email, sexo))
                    throw new Exception("No se pudo actualizar el usuario en la base de datos.");
            }
        }
    }
}