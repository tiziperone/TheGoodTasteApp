using System;
using System.Text.RegularExpressions;

namespace TheGoodTaste.Negocio
{
    public class ClienteNegocio //Clase que contiene la lógica de negocio relacionada con los clientes
    {
        public void GuardarCliente(string dni, string nombre, string apellido, string email, string telefono)
        {
            if (dni.Length < 7 || dni.Length > 8)
                throw new Exception("El DNI debe tener 7 u 8 dígitos.");

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
                throw new Exception("El correo electrónico no tiene un formato válido (ejemplo: usuario@correo.com).");
        }
    }
}