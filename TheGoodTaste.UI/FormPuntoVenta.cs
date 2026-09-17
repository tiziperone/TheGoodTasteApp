using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using TheGoodTaste.Negocio;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormPuntoVenta : Form
    {
        private readonly VentaNegocio _negocio = new VentaNegocio();

        public FormPuntoVenta()
        {
            InitializeComponent();
            ConfigurarEventos();
        }

        private void FormPuntoVenta_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            InicializarTablaDetalles();
            LimpiarTodo();
        }

        private void ConfigurarEventos()
        {
            txtPrecio.KeyPress += SoloDecimales_KeyPress;
            cboCliente.SelectedIndexChanged += Control_Modificado;
            cboTipoFactura.SelectedIndexChanged += Control_Modificado;
            cboProducto.SelectedIndexChanged += Control_Modificado;
            nudCantidad.ValueChanged += Control_Modificado;
            txtPrecio.TextChanged += Control_Modificado;
        }

        private void InicializarTablaDetalles()
        {
            if (dgvDetalles.Columns.Count == 0)
            {
                dgvDetalles.Columns.Add("ID", "ID");
                dgvDetalles.Columns.Add("Producto", "Producto");
                dgvDetalles.Columns.Add("Precio", "Precio Unit.");
                dgvDetalles.Columns.Add("Cantidad", "Cantidad");
                dgvDetalles.Columns.Add("Subtotal", "Subtotal");
            }
        }

        private void SoloDecimales_KeyPress(object sender, KeyPressEventArgs e)
        {
            char decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',') { e.Handled = true; return; }
            if (e.KeyChar == '.' || e.KeyChar == ',') { e.KeyChar = decSep; if (txtPrecio.Text.Contains(decSep.ToString())) e.Handled = true; }
        }

        private void Control_Modificado(object sender, EventArgs e) => ActualizarEstadoBotones();

        private void ActualizarEstadoBotones()
        {
            bool precioValido = decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio) && precio > 0;
            btnAgregar.Enabled = cboProducto.SelectedIndex != -1 && nudCantidad.Value > 0 && precioValido;
            btnGuardarVenta.Enabled = cboCliente.SelectedIndex != -1 && cboTipoFactura.SelectedIndex != -1 && dgvDetalles.Rows.Count > 0;
            btnLimpiar.Enabled = cboCliente.SelectedIndex != -1 || cboTipoFactura.SelectedIndex != -1 || cboProducto.SelectedIndex != -1 || !string.IsNullOrWhiteSpace(txtPrecio.Text) || dgvDetalles.Rows.Count > 0;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Ingrese un precio válido mayor a 0.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return;
            }

            int cantidad = (int)nudCantidad.Value;
            decimal subtotal = precio * cantidad;
            dgvDetalles.Rows.Add((cboProducto.SelectedIndex + 1).ToString(), cboProducto.Text, precio.ToString("N2"), cantidad, subtotal.ToString("N2"));

            cboProducto.SelectedIndex = -1; nudCantidad.Value = 1; txtPrecio.Clear();
            CalcularTotalVenta();
            ActualizarEstadoBotones();
        }

        private void btnGuardarVenta_Click(object sender, EventArgs e)
        {
            try
            {
                Venta nuevaVenta = new Venta
                {
                    Fecha = dtpFechaVenta.Value,
                    IdCliente = cboCliente.SelectedIndex != -1 ? Convert.ToInt32(cboCliente.SelectedValue) : 1,
                    MetodoEnvio = "Local",
                    DireccionEnvio = "Retiro en sucursal",
                    Total = CalcularTotalVenta(),
                    Detalles = new List<VentaDetalle>()
                };

                foreach (DataGridViewRow row in dgvDetalles.Rows)
                {
                    if (row.IsNewRow) continue;
                    nuevaVenta.Detalles.Add(new VentaDetalle
                    {
                        IdProducto = Convert.ToInt32(row.Cells["ID"].Value),
                        Cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value),
                        PrecioUnitario = Convert.ToDecimal(row.Cells["Precio"].Value)
                    });
                }

                _negocio.RegistrarVenta(nuevaVenta);
                MessageBox.Show("Venta registrada con éxito.", "Venta Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e) => LimpiarTodo();

        private decimal CalcularTotalVenta()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvDetalles.Rows)
                if (row.Cells["Subtotal"].Value != null && decimal.TryParse(row.Cells["Subtotal"].Value.ToString(), out decimal sub)) total += sub;
            return total;
        }

        private void LimpiarTodo()
        {
            cboCliente.SelectedIndex = -1; cboTipoFactura.SelectedIndex = -1; dtpFechaVenta.Value = DateTime.Today; cboProducto.SelectedIndex = -1; nudCantidad.Value = 1; txtPrecio.Clear(); dgvDetalles.Rows.Clear();
            CalcularTotalVenta(); ActualizarEstadoBotones();
        }
    }
}