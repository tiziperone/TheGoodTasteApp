using System;
using System.Data;
using System.Text.RegularExpressions;
using The_Good_Taste.Datos; // Ajustar namespace según tu capa de datos

namespace TheGoodTaste.Negocio
{
    public class ClienteNegocio
    {
        private readonly ClienteDatos _datos = new ClienteDatos();

        // Ya no recibe parámetros de estado
        public DataTable ObtenerClientes()
        {
            return _datos.ListarClientes();
        }

        public DataRow ObtenerClientePorDNI(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new Exception("El DNI es requerido para la consulta.");

            DataTable dt = _datos.ListarClientes();
            DataRow[] filas = dt.Select($"DNI = '{dni.Replace("'", "''")}'");

            return filas.Length > 0 ? filas[0] : null;
        }

        // NUEVO: Eliminación física
        public bool EliminarCliente(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
                throw new Exception("No se especificó un DNI válido para eliminar.");

            return _datos.EliminarCliente(dni);
        }

        // Guardar cliente con validaciones completas
        public void GuardarCliente(string dni, string nombre, string apellido, DateTime fechaNacimiento, string email, string telefono, string pais, string localidad, string provincia, string calle, string altura)
        {
            ValidarDatos(dni, nombre, apellido, fechaNacimiento, email, telefono);

            string campoDuplicado = _datos.VerificarDuplicados(dni, email, telefono);
            if (!string.IsNullOrEmpty(campoDuplicado))
                throw new Exception($"No se puede guardar. Ya existe un cliente registrado con el mismo {campoDuplicado}.");

            _datos.InsertarCliente(dni, nombre, apellido, fechaNacimiento, email, telefono, pais, localidad, provincia, calle, altura);
        }

        // Modificar cliente con validación de duplicados excluyendo el propio DNI
        public void ModificarCliente(string dniOriginal, string dniNuevo, string nombre, string apellido, DateTime fechaNacimiento, string email, string telefono, string pais, string localidad, string provincia, string calle, string altura)
        {
            ValidarDatos(dniNuevo, nombre, apellido, fechaNacimiento, email, telefono);

            string campoDuplicado = _datos.VerificarDuplicadosModificacion(dniOriginal, dniNuevo, email, telefono);
            if (!string.IsNullOrEmpty(campoDuplicado))
                throw new Exception($"No se puede modificar. Ya existe otro cliente con el mismo {campoDuplicado}.");

            _datos.ModificarCliente(dniOriginal, dniNuevo, nombre, apellido, fechaNacimiento, email, telefono, pais, localidad, provincia, calle, altura);
        }

        // Validaciones integrales
        private void ValidarDatos(string dni, string nombre, string apellido, DateTime fechaNacimiento, string email, string telefono)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new Exception("El nombre del cliente no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(apellido))
                throw new Exception("El apellido del cliente no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(dni) || dni.Length < 7 || dni.Length > 8)
                throw new Exception("El DNI debe contener entre 7 y 8 dígitos.");

            if (string.IsNullOrWhiteSpace(telefono) || telefono.Length < 10)
                throw new Exception("El número de teléfono debe tener al menos 10 dígitos.");

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, emailPattern))
                throw new Exception("El correo electrónico no tiene un formato válido.");

            int edad = DateTime.Today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > DateTime.Today.AddYears(-edad)) edad--;

            if (edad < 18)
                throw new Exception("El cliente debe ser mayor de edad (18 años o más).");
        }
    }
}