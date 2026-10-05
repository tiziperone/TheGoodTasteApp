using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using TheGoodTaste.Negocio;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormPuntoVenta : Form
    {
        private readonly VentaNegocio _ventaNegocio = new VentaNegocio();
        private readonly ClienteNegocio _clienteNegocio = new ClienteNegocio();
        private readonly ProductoNegocio _productoNegocio = new ProductoNegocio();
        private List<Producto> _listaProductos;

        public FormPuntoVenta()
        {
            InitializeComponent();
            ConfigurarEventos();
        }

        private void FormPuntoVenta_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            InicializarTablaDetalles();
            CargarCombos();
            LimpiarTodo();
        }

        private void ConfigurarEventos()
        {
            txtPrecio.KeyPress += SoloDecimales_KeyPress;
            cboCliente.SelectedIndexChanged += Control_Modificado;
            cboProducto.SelectedIndexChanged += CboProducto_SelectedIndexChanged;
            nudCantidad.ValueChanged += Control_Modificado;
            txtPrecio.TextChanged += Control_Modificado;

            btnAgregar.Click += btnAgregar_Click;
            btnGuardarVenta.Click += btnGuardarVenta_Click;
            btnLimpiar.Click += btnLimpiar_Click;
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

            dgvDetalles.AllowUserToAddRows = false;
            dgvDetalles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void CargarCombos()
        {
            try
            {
                // 1. Obtener clientes activos pasando el booleano 'true'
                var clientes = _clienteNegocio.ObtenerClientes(true);
                if (cboCliente != null && clientes != null)
                {
                    cboCliente.DataSource = clientes;
                    cboCliente.DisplayMember = "NombreCompleto"; // Ajustar si en la entidad Cliente es Nombre o RazonSocial
                    cboCliente.ValueMember = "dniCliente";
                }

                // 2. Obtener productos activos en lista
                _listaProductos = _productoNegocio.ObtenerProductos();
                if (cboProducto != null && _listaProductos != null)
                {
                    cboProducto.DataSource = _listaProductos;
                    cboProducto.DisplayMember = "Nombre";
                    cboProducto.ValueMember = "Codigo";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar combos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CboProducto_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProducto.SelectedIndex != -1 && _listaProductos != null && cboProducto.SelectedValue != null)
            {
                string codigo = cboProducto.SelectedValue.ToString();
                var prod = _listaProductos.FirstOrDefault(p => p.Codigo == codigo);
                if (prod != null)
                {
                    txtPrecio.Text = prod.Precio.ToString("N2");
                }
            }
            else
            {
                txtPrecio.Clear();
            }
            ActualizarEstadoBotones();
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
            btnGuardarVenta.Enabled = cboCliente.SelectedIndex != -1 && dgvDetalles.Rows.Count > 0;
            btnLimpiar.Enabled = cboCliente.SelectedIndex != -1 || cboProducto.SelectedIndex != -1 || !string.IsNullOrWhiteSpace(txtPrecio.Text) || dgvDetalles.Rows.Count > 0;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cboProducto.SelectedIndex == -1 || cboProducto.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un producto válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Ingrese un precio válido mayor a 0.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return;
            }

            string codigoProd = cboProducto.SelectedValue.ToString();
            string nombreProd = cboProducto.Text;
            int cantidad = (int)nudCantidad.Value;

            // Agrupar por producto si ya fue agregado al DataGridView
            bool existe = false;
            foreach (DataGridViewRow row in dgvDetalles.Rows)
            {
                if (row.Cells["ID"].Value?.ToString() == codigoProd)
                {
                    int cantExistente = Convert.ToInt32(row.Cells["Cantidad"].Value);
                    int nuevaCant = cantExistente + cantidad;
                    decimal nuevoSubtotal = nuevaCant * precio;

                    row.Cells["Precio"].Value = precio.ToString("N2");
                    row.Cells["Cantidad"].Value = nuevaCant;
                    row.Cells["Subtotal"].Value = nuevoSubtotal.ToString("N2");
                    existe = true;
                    break;
                }
            }

            if (!existe)
            {
                decimal subtotal = precio * cantidad;
                dgvDetalles.Rows.Add(codigoProd, nombreProd, precio.ToString("N2"), cantidad, subtotal.ToString("N2"));
            }

            cboProducto.SelectedIndex = -1;
            nudCantidad.Value = 1;
            txtPrecio.Clear();

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
                    IdCliente = cboCliente.SelectedValue != null ? Convert.ToInt32(cboCliente.SelectedValue) : 1,
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

                // Llamada a la capa de Negocio
                _ventaNegocio.RegistrarVenta(nuevaVenta);
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
            {
                if (row.Cells["Subtotal"].Value != null && decimal.TryParse(row.Cells["Subtotal"].Value.ToString(), out decimal sub))
                    total += sub;
            }

            Control[] controles = Controls.Find("lblTotal", true);
            if (controles.Length > 0 && controles[0] is Label labelTotal)
            {
                labelTotal.Text = total.ToString("N2");
            }

            return total;
        }

        private void LimpiarTodo()
        {
            if (cboCliente != null) cboCliente.SelectedIndex = -1;
            if (dtpFechaVenta != null) dtpFechaVenta.Value = DateTime.Today;
            if (cboProducto != null) cboProducto.SelectedIndex = -1;
            if (nudCantidad != null) nudCantidad.Value = 1;
            if (txtPrecio != null) txtPrecio.Clear();

            dgvDetalles.Rows.Clear();
            CalcularTotalVenta();
            ActualizarEstadoBotones();
        }
    }
}