using System;
using System.Data;
using System.Text.RegularExpressions;
using The_Good_Taste.Datos;

namespace TheGoodTaste.Negocio
{
    public class ClienteNegocio
    {
        private readonly ClienteDatos _datos = new ClienteDatos();

        // Obtener clientes para la grilla
        public DataTable ObtenerClientes(bool estadoActivo)
        {
            return _datos.ListarClientes(estadoActivo);
        }

        public void GuardarCliente(string dni, string nombre, string apellido, DateTime fechaNacimiento, string email, string telefono, string pais, string localidad, string provincia, string calle, string altura)
        {
            ValidarDatos(dni, fechaNacimiento, email, telefono);

            string campoDuplicado = _datos.VerificarDuplicados(dni, email, telefono);
            if (!string.IsNullOrEmpty(campoDuplicado))
                throw new Exception($"No se puede guardar. Ya existe un cliente registrado con el mismo {campoDuplicado}.");

            _datos.InsertarCliente(dni, nombre, apellido, fechaNacimiento, email, telefono, pais, localidad, provincia, calle, altura);
        }

        public void ModificarCliente(string dniOriginal, string dniNuevo, string nombre, string apellido, DateTime fechaNacimiento, string email, string telefono, string pais, string localidad, string provincia, string calle, string altura)
        {
            ValidarDatos(dniNuevo, fechaNacimiento, email, telefono);

            // Validamos duplicados asegurándonos de NO contar al propio usuario que estamos modificando
            string campoDuplicado = _datos.VerificarDuplicadosModificacion(dniOriginal, dniNuevo, email, telefono);
            if (!string.IsNullOrEmpty(campoDuplicado))
                throw new Exception($"No se puede modificar. Ya existe otro cliente con el mismo {campoDuplicado}.");

            _datos.ModificarCliente(dniOriginal, dniNuevo, nombre, apellido, fechaNacimiento, email, telefono, pais, localidad, provincia, calle, altura);
        }

        // Centralizamos las validaciones para no repetir código
        private void ValidarDatos(string dni, DateTime fechaNacimiento, string email, string telefono)
        {
            if (dni.Length < 7 || dni.Length > 8)
                throw new Exception("El DNI debe tener 7 u 8 dígitos.");

            if (telefono.Length < 10)
                throw new Exception("El número de teléfono debe tener al menos 10 dígitos.");

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
                throw new Exception("El correo electrónico no tiene un formato válido.");

            int edad = DateTime.Today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;

            if (edad < 18)
                throw new Exception("El cliente debe ser mayor de edad (18 años o más).");
        }
    }
}