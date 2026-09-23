using System;
using System.Collections.Generic;
using System.Linq;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.Negocio
{
    public class ProductoNegocio//Clase que representa la lógica de negocio para la gestión de productos en el sistema
    {
        private static List<Producto> _listaProductosEnMemoria = new List<Producto>();
        private static int _contadorId = 1;

        public ProductoNegocio()
        {
            if (_listaProductosEnMemoria.Count == 0) CargarIniciales();
        }

        private void CargarIniciales()
        {
            _listaProductosEnMemoria.Add(new Producto { IdProducto = _contadorId++, Codigo = "PROD01", Nombre = "Bondiola Artesanal", Descripcion = "Bondiola curada al vacío", Precio = 6500.00m, Stock = 12, StockMinimo = 5, IdCategoria = 2 });
            _listaProductosEnMemoria.Add(new Producto { IdProducto = _contadorId++, Codigo = "PROD02", Nombre = "Ravioles Caseros", Descripcion = "Plancha de 24 unidades", Precio = 3800.00m, Stock = 20, StockMinimo = 5, IdCategoria = 1 });
        }

        public List<Producto> ObtenerProductos() => _listaProductosEnMemoria.ToList();

        public void GuardarProducto(string codigo, string nombre, string descripcion, decimal precio, int stock, int idCategoria)
        {
            if (precio <= 0)
                throw new Exception("Ingrese un precio válido mayor a 0.");

            if (_listaProductosEnMemoria.Any(p => p.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Ya existe un producto registrado con ese código.");

            _listaProductosEnMemoria.Add(new Producto
            {
                IdProducto = _contadorId++,
                Codigo = codigo.ToUpper(),
                Nombre = nombre,
                Descripcion = descripcion,
                Precio = precio,
                Stock = stock,
                StockMinimo = 5,
                IdCategoria = idCategoria
            });
        }

        public void EliminarProducto(Producto producto)
        {
            if (producto != null) _listaProductosEnMemoria.Remove(producto);
        }
    }
}