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
        private readonly CategoriaNegocio _categoriaNegocio = new CategoriaNegocio();
        private bool _esEdicion = false; // Bandera para saber si estamos guardando o modificando

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
            // Validaciones de ingreso
            txtPrecio.KeyPress += SoloNumerosYDecimal_KeyPress;
            textStockMin.KeyPress += SoloNumeros_KeyPress;
            txtCodigo.KeyPress += SoloNumeros_KeyPress; // Evita el ingreso de letras en el código

            // Eventos de modificación para habilitar botones
            txtCodigo.TextChanged += Control_Modificado;
            txtNombre.TextChanged += Control_Modificado;
            txtDescripcion.TextChanged += Control_Modificado;
            txtPrecio.TextChanged += Control_Modificado;
            nudStock.ValueChanged += Control_Modificado;
            textStockMin.TextChanged += Control_Modificado;
            cboCategoria.SelectedIndexChanged += Control_Modificado;

            // Navegación con flechas Arriba/Abajo
            txtCodigo.KeyDown += NavegarConFlechas_KeyDown;
            txtNombre.KeyDown += NavegarConFlechas_KeyDown;
            txtDescripcion.KeyDown += NavegarConFlechas_KeyDown;
            txtPrecio.KeyDown += NavegarConFlechas_KeyDown;
            nudStock.KeyDown += NavegarConFlechas_KeyDown;
            textStockMin.KeyDown += NavegarConFlechas_KeyDown;
            cboCategoria.KeyDown += NavegarConFlechas_KeyDown;

            // Evento click dentro de la grilla para los botones Modificar/Eliminar
            dgvProductos.CellContentClick += DgvProductos_CellContentClick;
        }

        private void NavegarConFlechas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                SendKeys.Send("{TAB}");
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                SendKeys.Send("+{TAB}"); // Shift + Tab para retroceder
            }
        }

        private void CargarCategorias()
        {
            cboCategoria.DataSource = null;
            cboCategoria.DataSource = _categoriaNegocio.ObtenerCategorias();
            cboCategoria.DisplayMember = "NombreCategoria";
            cboCategoria.ValueMember = "IdCategoria";
        }

        private void CargarGrillaProductos()
        {
            dgvProductos.DataSource = null;
            dgvProductos.Columns.Clear(); // Limpiamos para evitar duplicar columnas de botones

            dgvProductos.DataSource = _negocio.ObtenerProductos();

            // Ocultamos columnas de auditoría
            if (dgvProductos.Columns["DeleteAt"] != null) dgvProductos.Columns["DeleteAt"].Visible = false;
            if (dgvProductos.Columns["CreateAt"] != null) dgvProductos.Columns["CreateAt"].Visible = false;

            AgregarBotonesGrilla();
            dgvProductos.ClearSelection();
        }

        private void AgregarBotonesGrilla()
        {
            DataGridViewButtonColumn colEditar = new DataGridViewButtonColumn
            {
                Name = "Editar",
                HeaderText = "Editar",
                Text = "Modificar",
                UseColumnTextForButtonValue = true
            };
            dgvProductos.Columns.Insert(0, colEditar);

            DataGridViewButtonColumn colBaja = new DataGridViewButtonColumn
            {
                Name = "Baja",
                HeaderText = "Baja",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true
            };
            dgvProductos.Columns.Insert(1, colBaja);
        }

        private void DgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvProductos.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                string columnName = dgvProductos.Columns[e.ColumnIndex].Name;
                Producto prod = (Producto)dgvProductos.Rows[e.RowIndex].DataBoundItem;

                if (columnName == "Editar")
                {
                    _esEdicion = true;
                    txtCodigo.Text = prod.Codigo;
                    txtCodigo.Enabled = false; // Deshabilitamos el código porque es clave primaria
                    txtNombre.Text = prod.Nombre;
                    txtDescripcion.Text = prod.Descripcion;
                    txtPrecio.Text = prod.Precio.ToString();
                    nudStock.Value = prod.Stock;
                    textStockMin.Text = prod.StockMinimo.ToString();
                    cboCategoria.SelectedValue = prod.IdCategoria;

                    ActualizarEstadoBotones();
                }
                else if (columnName == "Baja")
                {
                    if (MessageBox.Show($"¿Desea eliminar '{prod.Nombre}'?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            _negocio.EliminarProducto(prod);
                            CargarGrillaProductos();
                            LimpiarCampos();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void SoloNumerosYDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            char decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',') { e.Handled = true; return; }
            if (e.KeyChar == '.' || e.KeyChar == ',') { e.KeyChar = decSep; if (txtPrecio.Text.Contains(decSep.ToString())) e.Handled = true; }
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) { e.Handled = true; }
        }

        private void Control_Modificado(object sender, EventArgs e) => ActualizarEstadoBotones();

        private void ActualizarEstadoBotones()
        {
            btnLimpiar.Enabled = !string.IsNullOrWhiteSpace(txtCodigo.Text) || !string.IsNullOrWhiteSpace(txtNombre.Text) || !string.IsNullOrWhiteSpace(txtDescripcion.Text) || !string.IsNullOrWhiteSpace(txtPrecio.Text) || !string.IsNullOrWhiteSpace(textStockMin.Text) || nudStock.Value > 0;
            btnGuardar.Enabled = !string.IsNullOrWhiteSpace(txtCodigo.Text) && !string.IsNullOrWhiteSpace(txtNombre.Text) && !string.IsNullOrWhiteSpace(txtPrecio.Text) && !string.IsNullOrWhiteSpace(textStockMin.Text) && cboCategoria.SelectedIndex != -1;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string precioTexto = txtPrecio.Text.Trim().Replace(',', '.');
                decimal.TryParse(precioTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio);
                int.TryParse(textStockMin.Text.Trim(), out int stockMin);

                if (_esEdicion)
                {
                    _negocio.ModificarProducto(txtCodigo.Text.Trim(), txtNombre.Text.Trim(), txtDescripcion.Text.Trim(), precio, (int)nudStock.Value, stockMin, (int)cboCategoria.SelectedValue);

                    MessageBox.Show($"Cambios realizados con éxito:\n\nProducto: {txtNombre.Text.Trim()}\nCategoría ID: {cboCategoria.SelectedValue}\nPrecio Actualizado: ${precio}\nStock Actualizado: {nudStock.Value}\nStock Mínimo: {stockMin}",
                                    "Actualización Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _negocio.GuardarProducto(txtCodigo.Text.Trim(), txtNombre.Text.Trim(), txtDescripcion.Text.Trim(), precio, (int)nudStock.Value, stockMin, (int)cboCategoria.SelectedValue);
                    MessageBox.Show("Producto registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarCampos();
                CargarGrillaProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e) => LimpiarCampos();

        private void LimpiarCampos()
        {
            _esEdicion = false;
            txtCodigo.Enabled = true; // Volvemos a habilitar el código para nuevos ingresos
            txtCodigo.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            textStockMin.Clear();
            nudStock.Value = 0;
            if (cboCategoria.Items.Count > 0) cboCategoria.SelectedIndex = 0;
            dgvProductos.ClearSelection();
            ActualizarEstadoBotones();
            txtCodigo.Focus();
        }

        

        private void buttonInactivos_Click(object sender, EventArgs e)
        {

        }

        private void buttonActivos_Click(object sender, EventArgs e)
        {

        }
    }
}