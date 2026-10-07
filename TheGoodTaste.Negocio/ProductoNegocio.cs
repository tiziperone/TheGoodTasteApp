using System;
using System.Collections.Generic;
using System.Linq;
using The_Good_Taste.Entidades;
using The_Good_Taste.Datos;

namespace TheGoodTaste.Negocio
{
    public class ProductoNegocio
    {
        public List<Producto> ObtenerProductos()
        {
            return ProductoDatos.ObtenerActivos();
        }

        public List<Producto> ObtenerProductosInactivos()
        {
            return ProductoDatos.ObtenerInactivos();
        }

        public void GuardarProducto(string codigo, string nombre, string descripcion, decimal precio, int stock, int stockMinimo, int idCategoria)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new Exception("El código es obligatorio.");
            if (string.IsNullOrWhiteSpace(nombre)) throw new Exception("El nombre es obligatorio.");
            if (precio <= 0) throw new Exception("Ingrese un precio válido mayor a 0.");
            if (stock < 0) throw new Exception("El stock no puede ser negativo.");
            if (stockMinimo < 0) throw new Exception("El stock mínimo no puede ser negativo.");

            var productosExistentes = ProductoDatos.ObtenerActivos();
            if (productosExistentes.Any(p => p.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase)))
                throw new Exception("Ya existe un producto registrado con ese código.");

            Producto nuevoProducto = new Producto
            {
                Codigo = codigo.Trim().ToUpper(),
                Nombre = nombre.Trim(),
                Descripcion = descripcion?.Trim(),
                Precio = precio,
                Stock = stock,
                StockMinimo = stockMinimo,
                IdCategoria = idCategoria
            };

            if (!ProductoDatos.Insertar(nuevoProducto))
                throw new Exception("Ocurrió un error al guardar el producto en la base de datos.");
        }

        public void ModificarProducto(string codigo, string nombre, string descripcion, decimal precio, int stock, int stockMinimo, int idCategoria)
        {
            if (string.IsNullOrWhiteSpace(codigo)) throw new Exception("El código es obligatorio.");
            if (string.IsNullOrWhiteSpace(nombre)) throw new Exception("El nombre es obligatorio.");
            if (precio <= 0) throw new Exception("Ingrese un precio válido mayor a 0.");
            if (stock < 0) throw new Exception("El stock no puede ser negativo.");
            if (stockMinimo < 0) throw new Exception("El stock mínimo no puede ser negativo.");

            Producto productoModificado = new Producto
            {
                Codigo = codigo.ToUpper(),
                Nombre = nombre,
                Descripcion = descripcion,
                Precio = precio,
                Stock = stock,
                StockMinimo = stockMinimo,
                IdCategoria = idCategoria
            };

            if (!ProductoDatos.Actualizar(productoModificado))
                throw new Exception("Ocurrió un error al actualizar el producto en la base de datos.");
        }

        public void EliminarProducto(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.Codigo)) throw new Exception("Debe seleccionar un producto válido.");

            if (!ProductoDatos.Eliminar(producto.Codigo))
                throw new Exception("Ocurrió un error al intentar eliminar el producto de la base de datos.");
        }

        public void ActivarProducto(Producto producto)
        {
            if (producto == null) throw new Exception("Debe seleccionar un producto válido.");

            if (!ProductoDatos.Activar(producto.Codigo))
                throw new Exception("Ocurrió un error al intentar activar el producto en la base de datos.");
        }

        
        public bool ValidarStockMinimo(Producto producto, int cantidadAVender, out string mensajeAlerta)
            {
                mensajeAlerta = string.Empty;
                int stockRestante = producto.Stock - cantidadAVender;

                // Si el stock restante cae al límite o por debajo del stock mínimo
                if (stockRestante <= producto.StockMinimo)
                {
                    mensajeAlerta = $"Atención: Al agregar este producto, el stock disponible ({stockRestante}) quedará por debajo o igual al stock mínimo permitido ({producto.StockMinimo}).";
                    return true; // Retorna true para indicar que hay una alerta
                }

                return false;
         }
        
    }
}