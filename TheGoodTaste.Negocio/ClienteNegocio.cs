using System;
using System.Text.RegularExpressions;
using The_Good_Taste.Datos;

namespace TheGoodTaste.Negocio
{
    public class ClienteNegocio
    {
        private readonly ClienteDatos _datos = new ClienteDatos();

        public void GuardarCliente(string dni, string nombre, string apellido, DateTime fechaNacimiento, string email, string telefono, string pais, string localidad, string provincia, string calle, string altura)
        {
            // 1. Validación de DNI
            if (dni.Length < 7 || dni.Length > 8)
                throw new Exception("El DNI debe tener 7 u 8 dígitos.");

            // 2. Validación de Teléfono (mínimo 10 números)
            if (telefono.Length < 10)
                throw new Exception("El número de teléfono debe tener al menos 10 dígitos.");

            // 3. Validación de Correo Electrónico
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
                throw new Exception("El correo electrónico no tiene un formato válido (ejemplo: usuario@correo.com).");

            // 4. Validación de Edad (Mayor o igual a 18 años)
            int edad = DateTime.Today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--; // Ajusta si aún no ha cumplido años este año

            if (edad < 18)
                throw new Exception("El cliente debe ser mayor de edad (18 años o más).");

            // 5. Validación de Duplicados en Base de Datos
            string campoDuplicado = _datos.VerificarDuplicados(dni, email, telefono);
            if (!string.IsNullOrEmpty(campoDuplicado))
            {
                throw new Exception($"No se puede guardar. Ya existe un cliente registrado con el mismo {campoDuplicado}.");
            }

            // Si pasa todas las validaciones, se guarda
            _datos.InsertarCliente(dni, nombre, apellido, fechaNacimiento, email, telefono, pais, localidad, provincia, calle, altura);
        }
    }
}