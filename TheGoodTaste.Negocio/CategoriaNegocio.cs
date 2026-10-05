using System.Collections.Generic;
using The_Good_Taste.Entidades;
using The_Good_Taste.Datos;

namespace TheGoodTaste.Negocio
{
    public class CategoriaNegocio
    {
        public List<Categoria> ObtenerCategorias()
        {
            return CategoriaDatos.ObtenerTodas();
        }
    }
}