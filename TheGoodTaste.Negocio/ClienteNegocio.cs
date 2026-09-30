using System;
using System.Text.RegularExpressions;
using The_Good_Taste.Datos; // Importamos la capa de datos

namespace TheGoodTaste.Negocio
{
    public class ClienteNegocio // Clase que contiene la lógica de negocio relacionada con los clientes
    {
        private readonly ClienteDatos _datos = new ClienteDatos();

        public void GuardarCliente(string dni, string nombre, string apellido, DateTime fechaNacimiento, string email, string telefono, string pais, string localidad, string provincia, string calle, string altura)
        {
            // Validaciones de negocio
            if (dni.Length < 7 || dni.Length > 8)
                throw new Exception("El DNI debe tener 7 u 8 dígitos.");

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
                throw new Exception("El correo electrónico no tiene un formato válido (ejemplo: usuario@correo.com).");

            if (fechaNacimiento > DateTime.Now)
                throw new Exception("La fecha de nacimiento no puede ser mayor a la fecha actual.");

            // Llamada a la capa de datos
            _datos.InsertarCliente(dni, nombre, apellido, fechaNacimiento, email, telefono, pais, localidad, provincia, calle, altura);
        }
    }
}