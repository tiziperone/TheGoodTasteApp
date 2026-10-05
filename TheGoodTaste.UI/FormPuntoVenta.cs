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

        // Variable para guardar al vendedor que está usando la caja
        private readonly int _dniVendedorActivo;

        // Modificamos el constructor para que exija el DNI al abrirse
        public FormPuntoVenta(int dniVendedor)
        {
            InitializeComponent();
            ConfigurarEventos();
            _dniVendedorActivo = dniVendedor;
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

            nudCantidad.ValueChanged += ActualizarSubtotal_Modificado;
            txtPrecio.TextChanged += ActualizarSubtotal_Modificado;

            btnAgregar.Click += btnAgregar_Click;
            btnGuardarVenta.Click += btnGuardarVenta_Click;
            btnLimpiar.Click += btnLimpiar_Click;
        }

        private void InicializarTablaDetalles()
        {
            dgvDetalles.Columns.Clear();
            dgvDetalles.Columns.Add("ID", "Código");
            dgvDetalles.Columns.Add("Producto", "Producto");
            dgvDetalles.Columns.Add("Precio", "Precio Unit.");
            dgvDetalles.Columns.Add("Cantidad", "Cantidad");
            dgvDetalles.Columns.Add("Subtotal", "Subtotal");
            dgvDetalles.AllowUserToAddRows = false;
            dgvDetalles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void CargarCombos()
        {
            try
            {
                var clientes = _clienteNegocio.ObtenerClientes(true);
                if (cboCliente != null && clientes != null)
                {
                    cboCliente.DataSource = clientes;
                    cboCliente.DisplayMember = "nombreCliente";
                    cboCliente.ValueMember = "dniCliente";
                }

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
                    txtPrecio.Text = prod.Precio.ToString("0.00");
                }
            }
            else
            {
                txtPrecio.Clear();
            }
            ActualizarSubtotal_Modificado(sender, e);
        }

        private void SoloDecimales_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.') e.KeyChar = ',';
            if (e.KeyChar == ',' && txtPrecio.Text.Contains(",")) e.Handled = true;
        }

        private decimal ConvertirADecimal(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0;
            string textoLimpio = texto.Replace(".", "").Replace(",", ".");
            decimal.TryParse(textoLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado);
            return resultado;
        }

        private void Control_Modificado(object sender, EventArgs e) => ActualizarEstadoBotones();

        private void ActualizarSubtotal_Modificado(object sender, EventArgs e)
        {
            ActualizarEstadoBotones();
        }

        private void ActualizarEstadoBotones()
        {
            decimal precio = ConvertirADecimal(txtPrecio.Text);
            btnAgregar.Enabled = cboProducto.SelectedIndex != -1 && nudCantidad.Value > 0 && precio > 0;
            btnGuardarVenta.Enabled = cboCliente.SelectedIndex != -1 && dgvDetalles.Rows.Count > 0;
            btnLimpiar.Enabled = cboCliente.SelectedIndex != -1 || cboProducto.SelectedIndex != -1 || !string.IsNullOrWhiteSpace(txtPrecio.Text) || dgvDetalles.Rows.Count > 0;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            btnAgregar.Enabled = false;

            if (cboProducto.SelectedIndex == -1 || cboProducto.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un producto válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ActualizarEstadoBotones();
                return;
            }

            decimal precio = ConvertirADecimal(txtPrecio.Text);
            if (precio <= 0)
            {
                MessageBox.Show("Ingrese un precio válido mayor a 0.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                ActualizarEstadoBotones();
                return;
            }

            string codigoProd = cboProducto.SelectedValue.ToString();
            string nombreProd = cboProducto.Text;
            int cantidad = (int)nudCantidad.Value;

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

                    // USAMOS LA VARIABLE DE LA SESIÓN ACTIVA QUE NOS PASARON DESDE EL MENÚ
                    DNIUsuario = _dniVendedorActivo,

                    Total = CalcularTotalVenta(),
                    Detalles = new List<VentaDetalle>()
                };

                foreach (DataGridViewRow row in dgvDetalles.Rows)
                {
                    if (row.IsNewRow) continue;

                    decimal precioUnitario = ConvertirADecimal(row.Cells["Precio"].Value.ToString());

                    nuevaVenta.Detalles.Add(new VentaDetalle
                    {
                        Codigo = row.Cells["ID"].Value.ToString(),
                        Cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value),
                        PrecioUnitario = precioUnitario
                    });
                }

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
                if (row.IsNewRow) continue;

                decimal sub = ConvertirADecimal(row.Cells["Subtotal"].Value?.ToString());
                total += sub;
            }

            Control[] controles = Controls.Find("lblTotal", true);
            if (controles.Length > 0 && controles[0] is Label labelTotal)
            {
                labelTotal.Text = total.ToString("N2");
            }
            else
            {
                Control[] altControles = Controls.Find("labelTotal", true);
                if (altControles.Length > 0 && altControles[0] is Label altLabelTotal)
                {
                    altLabelTotal.Text = total.ToString("N2");
                }
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