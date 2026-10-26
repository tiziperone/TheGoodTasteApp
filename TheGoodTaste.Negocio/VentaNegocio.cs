using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using The_Good_Taste.Datos;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.Negocio
{
    public class VentaNegocio
    {
        private readonly ClienteDatos _clienteDatos = new ClienteDatos();

        public List<Producto> ObtenerProductosActivos()
        {
            return VentaDatos.ObtenerProductosActivos();
        }

        public int ObtenerStockActual(string codigo)
        {
            return VentaDatos.ObtenerStockActual(codigo);
        }

        public int RegistrarVenta(Venta nuevaVenta)
        {
            if (nuevaVenta == null)
                throw new Exception("La venta no contiene información.");

            if (nuevaVenta.Detalles == null || nuevaVenta.Detalles.Count == 0)
                throw new Exception("Debe agregar al menos un producto a la venta.");

            if (nuevaVenta.TotalVenta <= 0)
                throw new Exception("El total de la venta debe ser mayor a 0.");

            if (string.IsNullOrWhiteSpace(nuevaVenta.DniCliente))
                throw new Exception("Debe especificar un DNI de cliente válido.");

            if (!_clienteDatos.ExisteClientePorDni(nuevaVenta.DniCliente))
                throw new Exception($"El cliente con DNI '{nuevaVenta.DniCliente}' no se encuentra registrado en la base de datos.");

            if (nuevaVenta.DNIUsuario <= 0)
                throw new Exception("No se pudo identificar al cajero/vendedor que procesa la venta.");

            try
            {
                int idGenerado = VentaDatos.RegistrarVenta(nuevaVenta);
                if (idGenerado <= 0)
                    throw new Exception("No se pudo registrar la venta en la base de datos.");

                return idGenerado;
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 547 || sqlEx.Message.Contains("CK_stockProducto"))
                {
                    throw new Exception("No hay suficiente stock disponible para uno o más productos seleccionados.");
                }

                throw new Exception("Error en la base de datos: " + sqlEx.Message);
            }
        }
    }
}