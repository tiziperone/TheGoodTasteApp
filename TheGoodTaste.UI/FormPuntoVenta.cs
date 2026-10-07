using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
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

        // --- NUEVO: Instancia del negocio de Tipos de Pago ---
        private readonly TipoPagoNegocio _tipoPagoNegocio = new TipoPagoNegocio();

        private List<Producto> _listaProductos = new List<Producto>();
        private DataTable _tablaClientes = new DataTable();

        private Producto _productoSeleccionado = null;
        private DataRow _clienteSeleccionado = null;

        private bool _bloquearEventos = false;
        private readonly int _dniVendedorActivo;

        private readonly UsuarioNegocio _usuarioNegocio = new UsuarioNegocio();

        public FormPuntoVenta(int dniVendedor)
        {
            InitializeComponent();
            _dniVendedorActivo = dniVendedor;
            this.KeyPreview = true;
            this.KeyDown += FormPuntoVenta_KeyDown;
        }

        private void FormPuntoVenta_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            InicializarTablaDetalles();
            CargarDatosIniciales();
            MostrarInfoVendedor();
            LimpiarTodo();
        }

        #region --- CONFIGURACIÓN E INICIALIZACIÓN ---

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
            dgvDetalles.MultiSelect = false;
        }

        private void MostrarInfoVendedor()
        {
            Control[] controles = Controls.Find("lblVendedor", true);
            if (controles.Length > 0 && controles[0] is Label lblVendedor)
            {
                DataRow rowUsuario = _usuarioNegocio.ObtenerUsuarioPorDNI(_dniVendedorActivo);

                if (rowUsuario != null)
                {
                    string nombre = rowUsuario["Nombre"].ToString();
                    string apellido = rowUsuario["Apellido"].ToString();
                    lblVendedor.Text = $"Cajero: {nombre} {apellido} | DNI: {_dniVendedorActivo}";
                }
                else
                {
                    lblVendedor.Text = $"Cajero DNI: {_dniVendedorActivo}";
                }
            }
        }

        private void CargarDatosIniciales()
        {
            try
            {
                _tablaClientes = _clienteNegocio.ObtenerClientes(true) ?? new DataTable();
                _listaProductos = _productoNegocio.ObtenerProductos() ?? new List<Producto>();

                // --- NUEVO: Cargar los tipos de pago al ComboBox ---
                List<TipoPago> tipos = _tipoPagoNegocio.ObtenerTiposPago();
                cboTipoPago.DataSource = tipos;
                cboTipoPago.DisplayMember = "NombreTipoPago"; // Lo que el usuario ve
                cboTipoPago.ValueMember = "IdTipoPago";        // El ID oculto (1, 2, 3...)

                if (cboTipoPago.Items.Count > 0)
                    cboTipoPago.SelectedIndex = 0; // Seleccionar el primero por defecto
                // ----------------------------------------------------

                ConfigurarBuscadoresAutocompletado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarBuscadoresAutocompletado()
        {
            Control[] ctrlCliente = Controls.Find("txtBuscarCliente", true);
            if (ctrlCliente.Length > 0 && ctrlCliente[0] is TextBox txtCliente)
            {
                txtCliente.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtCliente.AutoCompleteSource = AutoCompleteSource.CustomSource;
                AutoCompleteStringCollection colClientes = new AutoCompleteStringCollection();

                foreach (DataRow row in _tablaClientes.Rows)
                {
                    string dni = row["dniCliente"].ToString();
                    string nombre = row["nombreCliente"].ToString();
                    colClientes.Add($"{dni} - {nombre}");
                    colClientes.Add(nombre);
                }

                txtCliente.AutoCompleteCustomSource = colClientes;
                txtCliente.KeyDown -= txtBuscarCliente_KeyDown;
                txtCliente.Leave -= txtBuscarCliente_Leave;
                txtCliente.KeyDown += txtBuscarCliente_KeyDown;
                txtCliente.Leave += txtBuscarCliente_Leave;
            }

            Control[] ctrlProd = Controls.Find("txtBuscarProducto", true);
            if (ctrlProd.Length > 0 && ctrlProd[0] is TextBox txtProd)
            {
                txtProd.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtProd.AutoCompleteSource = AutoCompleteSource.CustomSource;
                AutoCompleteStringCollection colProds = new AutoCompleteStringCollection();

                foreach (var p in _listaProductos)
                {
                    colProds.Add(p.Nombre);
                    colProds.Add($"{p.Codigo} - {p.Nombre}");
                }

                txtProd.AutoCompleteCustomSource = colProds;
                txtProd.KeyDown -= txtBuscarProducto_KeyDown;
                txtProd.Leave -= txtBuscarProducto_Leave;
                txtProd.KeyDown += txtBuscarProducto_KeyDown;
                txtProd.Leave += txtBuscarProducto_Leave;
            }
        }

        #endregion

        #region --- LÓGICA DE BÚSQUEDA Y AUTOCOMPLETADO ---

        private void txtBuscarProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcesarSeleccionProducto();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtBuscarProducto_Leave(object sender, EventArgs e) => ProcesarSeleccionProducto();

        private void ProcesarSeleccionProducto()
        {
            if (_bloquearEventos) return;

            Control[] ctrlProd = Controls.Find("txtBuscarProducto", true);
            if (ctrlProd.Length == 0 || !(ctrlProd[0] is TextBox txtProd)) return;

            string texto = txtProd.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                _productoSeleccionado = null;
                txtPrecio.Clear();
                ActualizarEstadoBotones();
                return;
            }

            _productoSeleccionado = _listaProductos.FirstOrDefault(p =>
                p.Nombre.Equals(texto, StringComparison.OrdinalIgnoreCase) ||
                $"{p.Codigo} - {p.Nombre}".Equals(texto, StringComparison.OrdinalIgnoreCase) ||
                p.Nombre.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0 ||
                p.Codigo.Equals(texto, StringComparison.OrdinalIgnoreCase));

            if (_productoSeleccionado != null)
            {
                _bloquearEventos = true;
                txtProd.Text = _productoSeleccionado.Nombre;
                txtPrecio.Text = _productoSeleccionado.Precio.ToString("N2");
                _bloquearEventos = false;

                nudCantidad.Focus();
            }
            else
            {
                txtPrecio.Clear();
            }

            ActualizarEstadoBotones();
        }

        private void txtBuscarCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcesarSeleccionCliente();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtBuscarCliente_Leave(object sender, EventArgs e) => ProcesarSeleccionCliente();

        private void ProcesarSeleccionCliente()
        {
            if (_bloquearEventos) return;

            Control[] ctrlCliente = Controls.Find("txtBuscarCliente", true);
            if (ctrlCliente.Length == 0 || !(ctrlCliente[0] is TextBox txtCliente)) return;

            string texto = txtCliente.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                _clienteSeleccionado = null;
                ActualizarInfoClientePanel(null);
                ActualizarEstadoBotones();
                return;
            }

            _clienteSeleccionado = _tablaClientes.AsEnumerable().FirstOrDefault(row =>
                row["dniCliente"].ToString().Equals(texto, StringComparison.OrdinalIgnoreCase) ||
                row["nombreCliente"].ToString().Equals(texto, StringComparison.OrdinalIgnoreCase) ||
                $"{row["dniCliente"]} - {row["nombreCliente"]}".Equals(texto, StringComparison.OrdinalIgnoreCase));

            if (_clienteSeleccionado != null)
            {
                _bloquearEventos = true;
                txtCliente.Text = $"{_clienteSeleccionado["dniCliente"]} - {_clienteSeleccionado["nombreCliente"]}";
                _bloquearEventos = false;
            }

            ActualizarInfoClientePanel(_clienteSeleccionado);
            ActualizarEstadoBotones();
        }

        private void ActualizarInfoClientePanel(DataRow cliente)
        {
            Control[] lblInfo = Controls.Find("lblInfoCliente", true);
            if (lblInfo.Length > 0 && lblInfo[0] is Label label)
            {
                if (cliente != null)
                {
                    string nombre = cliente["nombreCliente"].ToString();
                    string dni = cliente["dniCliente"].ToString();
                    string tel = cliente.Table.Columns.Contains("telefonoCliente") ? cliente["telefonoCliente"].ToString() : "-";

                    label.Text = $"Cliente: {nombre} | DNI: {dni} | Tel: {tel}";
                    label.ForeColor = Color.Green;
                }
                else
                {
                    label.Text = "Cliente no seleccionado (se usará Consumidor Final)";
                    label.ForeColor = Color.DarkGray;
                }
            }
        }

        #endregion

        #region --- OPERACIONES DE VENTA Y DETALLES ---

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            ProcesarSeleccionProducto();

            if (_productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione o busque un producto válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal precio = ConvertirADecimal(txtPrecio.Text);
            if (precio <= 0)
            {
                MessageBox.Show("Ingrese un precio válido mayor a 0.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return;
            }

            int cantidad = (int)nudCantidad.Value;
            if (cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad mayor a 0.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codigoProd = _productoSeleccionado.Codigo;
            string nombreProd = _productoSeleccionado.Nombre;

            int stockReal = _ventaNegocio.ObtenerStockActual(codigoProd);
            _productoSeleccionado.Stock = stockReal;

            int cantidadExistenteEnGrilla = 0;
            DataGridViewRow filaExistente = null;

            foreach (DataGridViewRow row in dgvDetalles.Rows)
            {
                if (row.Cells["ID"].Value?.ToString() == codigoProd)
                {
                    cantidadExistenteEnGrilla = Convert.ToInt32(row.Cells["Cantidad"].Value);
                    filaExistente = row;
                    break;
                }
            }

            int cantidadTotalAVender = cantidadExistenteEnGrilla + cantidad;

            if (cantidadTotalAVender > _productoSeleccionado.Stock)
            {
                MessageBox.Show($"Stock insuficiente. Quedan {_productoSeleccionado.Stock} unidades disponibles en stock " +
                                $"(ya agregó {cantidadExistenteEnGrilla} a la grilla).",
                                "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_productoNegocio.ValidarStockMinimo(_productoSeleccionado, cantidadTotalAVender, out string mensajeAlerta))
            {
                MessageBox.Show(mensajeAlerta, "Alerta de Stock Mínimo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (filaExistente != null)
            {
                decimal nuevoSubtotal = cantidadTotalAVender * precio;
                filaExistente.Cells["Precio"].Value = precio.ToString("N2");
                filaExistente.Cells["Cantidad"].Value = cantidadTotalAVender;
                filaExistente.Cells["Subtotal"].Value = nuevoSubtotal.ToString("N2");
            }
            else
            {
                decimal subtotal = precio * cantidad;
                dgvDetalles.Rows.Add(codigoProd, nombreProd, precio.ToString("N2"), cantidad, subtotal.ToString("N2"));
            }

            LimpiarSeleccionProducto();
            CalcularTotalVenta();
            ActualizarEstadoBotones();
        }

        private decimal CalcularTotalVenta()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvDetalles.Rows)
            {
                if (row.IsNewRow) continue;
                decimal sub = ConvertirADecimal(row.Cells["Subtotal"].Value?.ToString());
                total += sub;
            }

            string totalFormateado = total.ToString("N2");
            Control[] ctrls = Controls.Find("lblTotalMonto", true);
            if (ctrls.Length > 0 && ctrls[0] is Label labelTotal)
            {
                labelTotal.Text = totalFormateado;
            }
            else
            {
                try { lblTotalMonto.Text = totalFormateado; } catch { }
            }
            return total;
        }

        private void btnGuardarVenta_Click(object sender, EventArgs e)
        {
            if (dgvDetalles.Rows.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto a la lista de venta.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- NUEVO: Validar que hayan seleccionado un tipo de pago ---
            if (cboTipoPago.SelectedValue == null)
            {
                MessageBox.Show("Por favor, seleccione un Tipo de Pago.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // -------------------------------------------------------------

            try
            {
                int DniCliente = _clienteSeleccionado != null ? Convert.ToInt32(_clienteSeleccionado["dniCliente"]) : 1;

                Venta nuevaVenta = new Venta
                {
                    FechaVenta = dtpFechaVenta.Value,
                    DniCliente = DniCliente.ToString(),
                    DNIUsuario = _dniVendedorActivo,
                    IdTipoPago = Convert.ToInt32(cboTipoPago.SelectedValue), // ASIGNAR FK
                    TotalVenta = CalcularTotalVenta(),
                    Detalles = new List<VentaDetalle>()
                };

                foreach (DataGridViewRow row in dgvDetalles.Rows)
                {
                    if (row.IsNewRow) continue;

                    nuevaVenta.Detalles.Add(new VentaDetalle
                    {
                        Codigo = row.Cells["ID"].Value.ToString(),
                        Cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value),
                        PrecioUnitario = ConvertirADecimal(row.Cells["Precio"].Value.ToString())
                    });
                }

                _ventaNegocio.RegistrarVenta(nuevaVenta);

                string rutaTicket = GenerarTicketPDF(nuevaVenta);

                MessageBox.Show("Venta registrada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (!string.IsNullOrEmpty(rutaTicket) && File.Exists(rutaTicket))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = rutaTicket,
                        UseShellExecute = true
                    });
                }

                LimpiarTodo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al Procesar Venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region --- GENERACIÓN DE TICKET ---

        private string GenerarTicketPDF(Venta venta)
        {
            try
            {
                string carpetaTickets = Path.Combine(Application.StartupPath, "Tickets");
                if (!Directory.Exists(carpetaTickets))
                {
                    Directory.CreateDirectory(carpetaTickets);
                }

                string nombreArchivo = $"Ticket_Venta_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string rutaCompleta = Path.Combine(carpetaTickets, nombreArchivo);

                using (StreamWriter writer = new StreamWriter(rutaCompleta))
                {
                    writer.WriteLine("==========================================");
                    writer.WriteLine("            THE GOOD TASTE POS            ");
                    writer.WriteLine("==========================================");
                    writer.WriteLine($"Fecha: {venta.FechaVenta:dd/MM/yyyy HH:mm:ss}");
                    writer.WriteLine($"Cliente ID/DNI: {venta.DniCliente}");
                    writer.WriteLine($"Vendedor DNI: {venta.DNIUsuario}");

                    // --- NUEVO: Extraer nombre del pago del ComboBox para el ticket ---
                    string metodoPago = cboTipoPago.Text;
                    writer.WriteLine($"Forma de Pago: {metodoPago}");
                    // ------------------------------------------------------------------

                    writer.WriteLine("------------------------------------------");
                    writer.WriteLine(string.Format("{0,-20} {1,5} {2,12}", "Producto", "Cant", "Subtotal"));
                    writer.WriteLine("------------------------------------------");

                    foreach (var det in venta.Detalles)
                    {
                        var prod = _listaProductos.FirstOrDefault(p => p.Codigo == det.Codigo);
                        string nombre = prod != null ? prod.Nombre : det.Codigo;
                        if (nombre.Length > 19) nombre = nombre.Substring(0, 19);

                        decimal subtotal = det.Cantidad * det.PrecioUnitario;
                        writer.WriteLine(string.Format("{0,-20} {1,5} {2,12:N2}", nombre, det.Cantidad, subtotal));
                    }

                    writer.WriteLine("------------------------------------------");
                    writer.WriteLine($"TOTAL: ${venta.TotalVenta:N2}");
                    writer.WriteLine("==========================================");
                    writer.WriteLine("       ¡Gracias por su compra!            ");
                }

                return rutaCompleta;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Venta guardada pero hubo un problema con el comprobante: " + ex.Message, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
        }

        #endregion

        #region --- EVENTOS AUXILIARES Y TECLAS ---

        private void FormPuntoVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                btnGuardarVenta.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F12)
            {
                btnLimpiar.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete && dgvDetalles.Focused && dgvDetalles.CurrentRow != null)
            {
                dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);
                CalcularTotalVenta();
                ActualizarEstadoBotones();
                e.Handled = true;
            }
        }

        private void LimpiarSeleccionProducto()
        {
            _bloquearEventos = true;
            Control[] ctrlProd = Controls.Find("txtBuscarProducto", true);
            if (ctrlProd.Length > 0 && ctrlProd[0] is TextBox txtProd)
            {
                txtProd.Clear();
                txtProd.Focus();
            }
            _productoSeleccionado = null;
            nudCantidad.Value = 1;
            txtPrecio.Clear();
            _bloquearEventos = false;
        }

        private void LimpiarTodo()
        {
            _bloquearEventos = true;
            Control[] ctrlCliente = Controls.Find("txtBuscarCliente", true);
            if (ctrlCliente.Length > 0 && ctrlCliente[0] is TextBox txtCliente)
            {
                txtCliente.Clear();
            }

            _clienteSeleccionado = null;
            ActualizarInfoClientePanel(null);

            if (dtpFechaVenta != null) dtpFechaVenta.Value = DateTime.Today;

            dgvDetalles.Rows.Clear();
            LimpiarSeleccionProducto();

            // Volver a seleccionar el primer tipo de pago al limpiar
            if (cboTipoPago.Items.Count > 0) cboTipoPago.SelectedIndex = 0;

            _bloquearEventos = false;
            CalcularTotalVenta();
            ActualizarEstadoBotones();
        }

        private void btnLimpiar_Click(object sender, EventArgs e) => LimpiarTodo();

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
            string textoLimpio = texto.Replace("$", "").Trim();
            textoLimpio = textoLimpio.Replace(".", "").Replace(",", ".");
            decimal.TryParse(textoLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado);
            return resultado;
        }

        private void ActualizarEstadoBotones()
        {
            decimal precio = ConvertirADecimal(txtPrecio.Text);
            btnAgregar.Enabled = _productoSeleccionado != null && nudCantidad.Value > 0 && precio > 0;
            btnGuardarVenta.Enabled = dgvDetalles.Rows.Count > 0;
            btnLimpiar.Enabled = _clienteSeleccionado != null || _productoSeleccionado != null || dgvDetalles.Rows.Count > 0;
        }

        private void lblTotalMonto_Click(object sender, EventArgs e) { }
        private void lblVendedor_Click(object sender, EventArgs e) { }
        private void cboTipoPago_SelectedIndexChanged(object sender, EventArgs e) { }
        private void labelTipoPago_Click(object sender, EventArgs e) { }

        #endregion
    }
}