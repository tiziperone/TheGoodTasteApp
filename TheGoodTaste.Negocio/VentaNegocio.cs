using System;
using System.Data.SqlClient;
using The_Good_Taste.Datos;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.Negocio
{
    public class VentaNegocio
    {
        private readonly ClienteDatos _clienteDatos = new ClienteDatos();

        public void RegistrarVenta(Venta nuevaVenta)
        {
            // 1. Validar que la venta no esté vacía ni sin productos
            if (nuevaVenta == null)
                throw new Exception("La venta no contiene información.");

            if (nuevaVenta.Detalles == null || nuevaVenta.Detalles.Count == 0)
                throw new Exception("Debe agregar al menos un producto a la venta.");

            if (nuevaVenta.TotalVenta <= 0)
                throw new Exception("El total de la venta debe ser mayor a 0.");

            // 2. Validar DNI de cliente
            if (string.IsNullOrWhiteSpace(nuevaVenta.DniCliente))
                throw new Exception("Debe especificar un DNI de cliente válido.");

            if (!_clienteDatos.ExisteClientePorDni(nuevaVenta.DniCliente))
                throw new Exception($"El cliente con DNI '{nuevaVenta.DniCliente}' no se encuentra registrado en la base de datos.");

            // 3. Validar DNI del cajero/vendedor
            if (nuevaVenta.DNIUsuario <= 0)
                throw new Exception("No se pudo identificar al cajero/vendedor que procesa la venta.");

            // 4. Registrar en la BD interceptando errores de SQL (Stock / Constraint)
            try
            {
                if (!VentaDatos.RegistrarVenta(nuevaVenta))
                    throw new Exception("No se pudo registrar la venta en la base de datos.");
            }
            catch (SqlException sqlEx)
            {
                // Número 547 = Violación de CHECK constraint en SQL Server
                if (sqlEx.Number == 547 || sqlEx.Message.Contains("CK_stockProducto"))
                {
                    throw new Exception("No hay suficiente stock disponible para uno o más productos seleccionados.");
                }

                throw new Exception("Error en la base de datos: " + sqlEx.Message);
            }
        }
    }
}