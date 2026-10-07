using System.Collections.Generic;
using System.Linq;
using The_Good_Taste.Datos;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.Negocio
{
    public class TipoPagoNegocio
    {
        private readonly TipoPagoDatos _tipoPagoDatos = new TipoPagoDatos();

        public List<TipoPago> ObtenerTiposPago()
        {
            List<TipoPago> tipos = _tipoPagoDatos.ObtenerTiposPago();

            // Si la base de datos está vacía, evitamos que rompa devolviendo una lista inicializada
            return tipos ?? new List<TipoPago>();
        }
    }
}