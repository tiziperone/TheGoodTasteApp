using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using TheGoodTaste.Negocio;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormProductos : Form
    {
        private readonly ProductoNegocio _negocio = new ProductoNegocio();

        public FormProductos()
        {
            InitializeComponent();
            ConfigurarEventos();
        }

        private void FormProductos_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            CargarCategorias();
            CargarGrillaProductos();
            ActualizarEstadoBotones();
        }

        private void ConfigurarEventos()
        {
            txtPrecio.KeyPress += SoloNumerosYDecimal_KeyPress;
            txtCodigo.TextChanged += Control_Modificado;
            txtNombre.TextChanged += Control_Modificado;
            txtDescripcion.TextChanged += Control_Modificado;
            txtPrecio.TextChanged += Control_Modificado;
            nudStock.ValueChanged += Control_Modificado;
            cboCategoria.SelectedIndexChanged += Control_Modificado;
            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;
        }

        private void CargarCategorias()
        {
            var categorias = new Dictionary<int, string> { { 1, "Pastas" }, { 2, "Bondiolas" }, { 3, "Milanesas" } };
            cboCategoria.DataSource = new BindingSource(categorias, null);
            cboCategoria.DisplayMember = "Value";
            cboCategoria.ValueMember = "Key";
        }

        private void CargarGrillaProductos()
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = _negocio.ObtenerProductos();

            if (dgvProductos.Columns["IdProducto"] != null) dgvProductos.Columns["IdProducto"].Visible = false;
            if (dgvProductos.Columns["DeletedAt"] != null) dgvProductos.Columns["DeletedAt"].Visible = false;
            dgvProductos.ClearSelection();
        }

        private void SoloNumerosYDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            char decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',') { e.Handled = true; return; }
            if (e.KeyChar == '.' || e.KeyChar == ',') { e.KeyChar = decSep; if (txtPrecio.Text.Contains(decSep.ToString())) e.Handled = true; }
        }

        private void Control_Modificado(object sender, EventArgs e) => ActualizarEstadoBotones();
        private void DgvProductos_SelectionChanged(object sender, EventArgs e) => btnEliminar.Enabled = dgvProductos.SelectedRows.Count > 0;

        private void ActualizarEstadoBotones()
        {
            btnLimpiar.Enabled = !string.IsNullOrWhiteSpace(txtCodigo.Text) || !string.IsNullOrWhiteSpace(txtNombre.Text) || !string.IsNullOrWhiteSpace(txtDescripcion.Text) || !string.IsNullOrWhiteSpace(txtPrecio.Text) || nudStock.Value > 0;
            btnGuardar.Enabled = !string.IsNullOrWhiteSpace(txtCodigo.Text) && !string.IsNullOrWhiteSpace(txtNombre.Text) && !string.IsNullOrWhiteSpace(txtPrecio.Text) && cboCategoria.SelectedIndex != -1;
            btnEliminar.Enabled = dgvProductos.SelectedRows.Count > 0;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string precioTexto = txtPrecio.Text.Trim().Replace(',', '.');
                decimal.TryParse(precioTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio);

                _negocio.GuardarProducto(txtCodigo.Text.Trim(), txtNombre.Text.Trim(), txtDescripcion.Text.Trim(), precio, (int)nudStock.Value, (int)cboCategoria.SelectedValue);
                MessageBox.Show("Producto registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrillaProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0) return;
            Producto productoSeleccionado = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;
            if (MessageBox.Show($"¿Desea eliminar '{productoSeleccionado.Nombre}'?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _negocio.EliminarProducto(productoSeleccionado);
                CargarGrillaProductos();
                LimpiarCampos();
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e) => LimpiarCampos();

        private void LimpiarCampos()
        {
            txtCodigo.Clear(); txtNombre.Clear(); txtDescripcion.Clear(); txtPrecio.Clear(); nudStock.Value = 0;
            if (cboCategoria.Items.Count > 0) cboCategoria.SelectedIndex = 0;
            dgvProductos.ClearSelection();
            ActualizarEstadoBotones();
            txtCodigo.Focus();
        }
    }
}