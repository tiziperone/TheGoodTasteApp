using System;
using System.Collections.Generic;
using System.Windows.Forms;
using The_Good_Taste.Entidades;

namespace The_Good_Taste.Vistas
{
    public partial class FormPuntoVenta : Form
    {
        private Cliente clienteSeleccionado;
        private List<VentaDetalle> carrito = new List<VentaDetalle>();

        public FormPuntoVenta()
        {
            InitializeComponent();
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            if (cboProductos.SelectedItem is Producto prod)
            {
                int cantidad = (int)nudCantidad.Value;

                VentaDetalle detalle = new VentaDetalle
                {
                    IdProducto = prod.IdProducto,
                    Producto = prod,
                    Cantidad = cantidad,
                    PrecioUnitario = prod.Precio // Corregido a 'Precio'
                };

                carrito.Add(detalle);
                ActualizarGrillaDetalles();
            }
        }

        private void ActualizarGrillaDetalles()
        {
            dgvCarrito.DataSource = null;
            dgvCarrito.DataSource = carrito;

            decimal total = 0;
            foreach (var item in carrito)
            {
                total += item.Subtotal;
            }
            lblTotal.Text = $"$ {total:N2}";
        }
    }
}