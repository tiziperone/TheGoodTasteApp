using System;
using The_Good_Taste.Datos;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.Negocio
{
    public class VentaNegocio
    {
        public void RegistrarVenta(Venta nuevaVenta)
        {
            if (nuevaVenta.Detalles == null || nuevaVenta.Detalles.Count == 0)
                throw new Exception("Debe agregar al menos un producto a la venta.");

            if (nuevaVenta.Total <= 0)
                throw new Exception("El total de la venta debe ser mayor a 0.");

            if (!VentaDatos.RegistrarVenta(nuevaVenta))
                throw new Exception("No se pudo registrar la venta en la base de datos.");
        }
    }
}