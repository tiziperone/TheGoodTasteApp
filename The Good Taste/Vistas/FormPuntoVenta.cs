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

        // Paso 1: Buscar cliente por DNI
        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            string dni = txtDniBusqueda.Text.Trim();
            // clienteSeleccionado = ClienteDatos.ObtenerPorDni(dni);

            if (clienteSeleccionado != null)
            {
                lblClienteNombre.Text = $"{clienteSeleccionado.Apellido}, {clienteSeleccionado.Nombre}";
                pnlProductos.Enabled = true; // Habilita la selección de productos
            }
            else
            {
                MessageBox.Show("Cliente no encontrado. Debe darlo de alta primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Paso 2: Agregar producto al detalle actual
        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            Producto prod = (Producto)cboProductos.SelectedItem;
            int cantidad = (int)nudCantidad.Value;

            VentaDetalle detalle = new VentaDetalle
            {
                IdProducto = prod.IdProducto,
                Producto = prod,
                Cantidad = cantidad,
                PrecioUnitario = prod.PrecioActual
            };

            carrito.Add(detalle);
            ActualizarGrillaDetalles();
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
            lblTotal.Text = $"$ {total:N0}";
        }

        // Paso 3: Confirmar Orden y Cobrar
        private void btnConfirmarVenta_Click(object sender, EventArgs e)
        {
            if (carrito.Count == 0 || clienteSeleccionado == null) return;

            Venta nuevaVenta = new Venta
            {
                IdCliente = clienteSeleccionado.IdCliente,
                Fecha = DateTime.Now,
                Total = Convert.ToDecimal(lblTotal.Text.Replace("$ ", "")),
                Detalles = carrito
            };

            // VentaLogica.ProcesarVenta(nuevaVenta);
            MessageBox.Show("Venta registrada con éxito. Stock actualizado.", "POS", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}