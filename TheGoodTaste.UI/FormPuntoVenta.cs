using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using The_Good_Taste.Datos;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormPuntoVenta : Form
    {
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
            // Restricción: el precio solo acepta números y coma/punto decimal
            txtPrecio.KeyPress += SoloDecimales_KeyPress;

            // Detección de cambios para actualizar el estado de los botones
            cboCliente.SelectedIndexChanged += Control_Modificado;
            cboTipoFactura.SelectedIndexChanged += Control_Modificado;
            cboProducto.SelectedIndexChanged += Control_Modificado;
            nudCantidad.ValueChanged += Control_Modificado;
            txtPrecio.TextChanged += Control_Modificado;

            // Eventos de clic
            btnAgregar.Click += btnAgregar_Click;
            btnGuardarVenta.Click += btnGuardarVenta_Click;
            btnLimpiar.Click += btnLimpiar_Click;
        }

        private void InicializarTablaDetalles()
        {
            // Configurar columnas si no fueron creadas desde el diseñador
            if (dgvDetalles.Columns.Count == 0)
            {
                dgvDetalles.Columns.Add("ID", "ID");
                dgvDetalles.Columns.Add("Producto", "Producto");
                dgvDetalles.Columns.Add("Precio", "Precio Unit.");
                dgvDetalles.Columns.Add("Cantidad", "Cantidad");
                dgvDetalles.Columns.Add("Subtotal", "Subtotal");
            }
        }

        // =======================
        // FILTRADO DE ENTRADA
        // =======================
        private void SoloDecimales_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite números, retroceso y separador decimal (. o ,)
            char decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
                return;
            }

            // Normaliza punto y coma para que coincida con el separador local
            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                e.KeyChar = decSep;
                if (txtPrecio.Text.Contains(decSep.ToString()))
                {
                    e.Handled = true;
                }
            }
        }

        // =======================
        // ESTADO DE BOTONES
        // =======================
        private void Control_Modificado(object sender, EventArgs e)
        {
            ActualizarEstadoBotones();
        }

        private void ActualizarEstadoBotones()
        {
            // Habilitar botón "Agregar Producto" si se seleccionó producto, cantidad > 0 y precio válido
            decimal precio = 0;
            bool precioValido = decimal.TryParse(txtPrecio.Text.Trim(), out precio) && precio > 0;
            bool productoListo = cboProducto.SelectedIndex != -1 && nudCantidad.Value > 0 && precioValido;
            btnAgregar.Enabled = productoListo;

            // Habilitar botón "Confirmar Venta" (btnGuardarVenta) si hay cliente, tipo factura y al menos 1 producto en la grilla
            bool cabeceraLista = cboCliente.SelectedIndex != -1 && cboTipoFactura.SelectedIndex != -1;
            bool tieneItems = dgvDetalles.Rows.Count > 0;
            btnGuardarVenta.Enabled = cabeceraLista && tieneItems;

            // Habilitar botón "Limpiar" si hay cualquier dato cargado
            bool hayDatos = cboCliente.SelectedIndex != -1 ||
                            cboTipoFactura.SelectedIndex != -1 ||
                            cboProducto.SelectedIndex != -1 ||
                            !string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                            dgvDetalles.Rows.Count > 0;
            btnLimpiar.Enabled = hayDatos;
        }

        // =======================
        // ACCIONES DE BOTONES
        // =======================
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
            string nombreProducto = cboProducto.Text;
            string idProducto = (cboProducto.SelectedIndex + 1).ToString(); // O el ID real según tu DataSource

            // Agregar fila al DataGridView
            dgvDetalles.Rows.Add(idProducto, nombreProducto, precio.ToString("N2"), cantidad, subtotal.ToString("N2"));

            // Limpiar controles de carga de producto
            cboProducto.SelectedIndex = -1;
            nudCantidad.Value = 1;
            txtPrecio.Clear();

            CalcularTotalVenta();
            ActualizarEstadoBotones();
        }

        private void btnGuardarVenta_Click(object sender, EventArgs e)
        {
            if (dgvDetalles.Rows.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto a la venta.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. Calcular el total 
                decimal totalVenta = 0;
                foreach (DataGridViewRow row in dgvDetalles.Rows)
                {
                    if (row.Cells["Subtotal"].Value != null)
                        totalVenta += Convert.ToDecimal(row.Cells["Subtotal"].Value);
                }

                // 2. Instanciar la cabecera de la venta
                Venta nuevaVenta = new Venta
                {
                    Fecha = dtpFechaVenta.Value,
                  
                    IdCliente = cboCliente.SelectedIndex != -1 ? Convert.ToInt32(cboCliente.SelectedValue) : 1,
                    MetodoEnvio = "Local",
                    DireccionEnvio = "Retiro en sucursal",
                    Total = totalVenta,
                    Detalles = new List<VentaDetalle>() // Inicializamos la lista de detalles
                };

                // 3. Llenar los detalles recorriendo las filas del DataGridView
                foreach (DataGridViewRow row in dgvDetalles.Rows)
                {
                    if (row.IsNewRow) continue;

                    VentaDetalle detalle = new VentaDetalle
                    {
                        IdProducto = Convert.ToInt32(row.Cells["ID"].Value),
                        Cantidad = Convert.ToInt32(row.Cells["Cantidad"].Value),
                        PrecioUnitario = Convert.ToDecimal(row.Cells["Precio"].Value)
                    };

                    nuevaVenta.Detalles.Add(detalle);
                }

                // 4. Enviar a la base de datos usando tu clase VentaDatos
                bool exito = VentaDatos.RegistrarVenta(nuevaVenta);

                if (exito)
                {
                    MessageBox.Show("Venta registrada con éxito en la base de datos.", "Venta Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarTodo(); // Vaciamos la pantalla para la siguiente venta
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al registrar la venta: " + ex.Message, "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarTodo();
        }

       
        // =======================
        private void CalcularTotalVenta()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvDetalles.Rows)
            {
                if (row.Cells["Subtotal"].Value != null)
                {
                    if (decimal.TryParse(row.Cells["Subtotal"].Value.ToString(), out decimal sub))
                    {
                        total += sub;
                    }
                }
            }

  
        }

        private void LimpiarTodo()
        {
            cboCliente.SelectedIndex = -1;
            cboTipoFactura.SelectedIndex = -1;
            dtpFechaVenta.Value = DateTime.Today;
            cboProducto.SelectedIndex = -1;
            nudCantidad.Value = 1;
            txtPrecio.Clear();
            dgvDetalles.Rows.Clear();

            CalcularTotalVenta();
            ActualizarEstadoBotones();
        }
    }
}