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
        private bool _esEdicion = false;
        private bool _viendoInactivos = false;

        // Variable para guardar el estado original del producto y comparar cambios
        private Producto _productoOriginalEdicion = null;

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
            textStockMin.KeyPress += SoloNumeros_KeyPress;
            txtCodigo.KeyPress += SoloNumeros_KeyPress;

            txtCodigo.TextChanged += Control_Modificado;
            txtNombre.TextChanged += Control_Modificado;
            txtDescripcion.TextChanged += Control_Modificado;
            txtPrecio.TextChanged += Control_Modificado;
            nudStock.ValueChanged += Control_Modificado;
            textStockMin.TextChanged += Control_Modificado;
            cboCategoria.SelectedIndexChanged += Control_Modificado;

            txtCodigo.KeyDown += NavegarConFlechas_KeyDown;
            txtNombre.KeyDown += NavegarConFlechas_KeyDown;
            txtDescripcion.KeyDown += NavegarConFlechas_KeyDown;
            txtPrecio.KeyDown += NavegarConFlechas_KeyDown;
            nudStock.KeyDown += NavegarConFlechas_KeyDown;
            textStockMin.KeyDown += NavegarConFlechas_KeyDown;
            cboCategoria.KeyDown += NavegarConFlechas_KeyDown;

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
                SendKeys.Send("+{TAB}");
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
            dgvProductos.Columns.Clear();

            if (_viendoInactivos)
            {
                dgvProductos.DataSource = _negocio.ObtenerProductosInactivos();
                AgregarBotonActivarGrilla();
            }
            else
            {
                dgvProductos.DataSource = _negocio.ObtenerProductos();
                AgregarBotonesGrilla();
            }

            if (dgvProductos.Columns["DeleteAt"] != null) dgvProductos.Columns["DeleteAt"].Visible = false;
            if (dgvProductos.Columns["CreateAt"] != null) dgvProductos.Columns["CreateAt"].Visible = false;

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

        private void AgregarBotonActivarGrilla()
        {
            DataGridViewButtonColumn colActivar = new DataGridViewButtonColumn
            {
                Name = "Activar",
                HeaderText = "Restaurar",
                Text = "Activar",
                UseColumnTextForButtonValue = true
            };
            dgvProductos.Columns.Insert(0, colActivar);
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
                    _productoOriginalEdicion = prod; // Guardamos el estado original para comparar después

                    txtCodigo.Text = prod.Codigo;
                    txtCodigo.Enabled = false;
                    txtNombre.Text = prod.Nombre;
                    txtDescripcion.Text = prod.Descripcion;
                    txtPrecio.Text = prod.Precio.ToString(CultureInfo.CurrentCulture);
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
                else if (columnName == "Activar")
                {
                    if (MessageBox.Show($"¿Desea volver a activar '{prod.Nombre}'?", "Confirmar Activación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            _negocio.ActivarProducto(prod);
                            CargarGrillaProductos();
                            MessageBox.Show("Producto reactivado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Error al activar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void SoloNumerosYDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            char decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                e.KeyChar = decSep;
                if (txtPrecio.Text.Contains(decSep.ToString())) e.Handled = true;
            }
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Control_Modificado(object sender, EventArgs e) => ActualizarEstadoBotones();

        private void ActualizarEstadoBotones()
        {
            btnLimpiar.Enabled = !string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                                 !string.IsNullOrWhiteSpace(txtNombre.Text) ||
                                 !string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                                 !string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                                 !string.IsNullOrWhiteSpace(textStockMin.Text) ||
                                 nudStock.Value > 0;

            btnGuardar.Enabled = !string.IsNullOrWhiteSpace(txtCodigo.Text) &&
                                 !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                                 !string.IsNullOrWhiteSpace(txtPrecio.Text) &&
                                 !string.IsNullOrWhiteSpace(textStockMin.Text) &&
                                 cboCategoria.SelectedIndex != -1;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string precioTexto = txtPrecio.Text.Trim().Replace(',', '.');
                decimal.TryParse(precioTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio);
                int.TryParse(textStockMin.Text.Trim(), out int stockMin);

                if (_esEdicion && _productoOriginalEdicion != null)
                {
                    // Lógica para detectar qué cambió exactamente
                    List<string> cambios = new List<string>();

                    if (txtNombre.Text.Trim() != _productoOriginalEdicion.Nombre)
                        cambios.Add($"- Nombre: '{_productoOriginalEdicion.Nombre}' -> '{txtNombre.Text.Trim()}'");

                    if (txtDescripcion.Text.Trim() != _productoOriginalEdicion.Descripcion)
                        cambios.Add($"- Descripción: '{_productoOriginalEdicion.Descripcion}' -> '{txtDescripcion.Text.Trim()}'");

                    if (precio != _productoOriginalEdicion.Precio)
                        cambios.Add($"- Precio: ${_productoOriginalEdicion.Precio} -> ${precio}");

                    if (nudStock.Value != _productoOriginalEdicion.Stock)
                        cambios.Add($"- Stock: {_productoOriginalEdicion.Stock} -> {nudStock.Value}");

                    if (stockMin != _productoOriginalEdicion.StockMinimo)
                        cambios.Add($"- Stock Mínimo: {_productoOriginalEdicion.StockMinimo} -> {stockMin}");

                    if ((int)cboCategoria.SelectedValue != _productoOriginalEdicion.IdCategoria)
                    {
                        string nombreCatOriginal = ((Categoria)cboCategoria.Items[cboCategoria.FindStringExact(_productoOriginalEdicion.IdCategoria.ToString())]).NombreCategoria; // Intenta buscar por texto si falla asume ID
                        cambios.Add($"- Categoría ID: {_productoOriginalEdicion.IdCategoria} -> {cboCategoria.SelectedValue}");
                    }

                    if (cambios.Count == 0)
                    {
                        MessageBox.Show("No se detectaron cambios en el producto.", "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        return;
                    }

                    // Armar mensaje con los cambios detectados
                    string mensajeConfirmacion = "¿Desea guardar los siguientes cambios?\n\n" + string.Join("\n", cambios);

                    if (MessageBox.Show(mensajeConfirmacion, "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        _negocio.ModificarProducto(txtCodigo.Text.Trim(), txtNombre.Text.Trim(), txtDescripcion.Text.Trim(), precio, (int)nudStock.Value, stockMin, (int)cboCategoria.SelectedValue);
                        MessageBox.Show("Producto modificado con éxito.", "Actualización Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LimpiarCampos();
                        CargarGrillaProductos();
                    }
                }
                else
                {
                    // Lógica para registrar un producto nuevo
                    _negocio.GuardarProducto(txtCodigo.Text.Trim(), txtNombre.Text.Trim(), txtDescripcion.Text.Trim(), precio, (int)nudStock.Value, stockMin, (int)cboCategoria.SelectedValue);
                    MessageBox.Show("Producto registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarCampos();
                    CargarGrillaProductos();
                }
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
            _productoOriginalEdicion = null;
            txtCodigo.Enabled = true;
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

        // 1. Solución para CS1503 en línea 165 (Pasar el código como string)
        private void EliminarProductoSeleccionado(string codigo)
        {
            var prod = new Producto { Codigo = codigo };
            _negocio.EliminarProducto(prod);
        }

        // 2. Solución para eventos faltantes de inactivos/activos del Diseñador (líneas 212 y 222)
        private void buttonInactivos_Click(object sender, EventArgs e)
        {
            // Lógica para mostrar inactivos si la usan, o vacío para no romper la compilación
        }

        private void buttonActivos_Click(object sender, EventArgs e)
        {
            // Lógica para mostrar activos si la usan, o vacío para no romper la compilación
        }
    }
}