using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        private Producto _productoOriginalEdicion = null;

        // Caché en memoria para filtrar sin ir a la base de datos constantemente
        private List<Producto> _productosActuales = new List<Producto>();

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

        // Se encarga EXCLUSIVAMENTE de traer los datos desde la BD a la memoria
        private void CargarGrillaProductos()
        {
            if (_viendoInactivos)
            {
                _productosActuales = _negocio.ObtenerProductosInactivos();
            }
            else
            {
                _productosActuales = _negocio.ObtenerProductos();
            }

            // Al recargar la grilla, aplicamos el filtro actual (o vacío si no hay nada escrito)
            Control txtBuscar = Controls.Find("textBoxBuscarProducto", true).FirstOrDefault();
            string filtroActual = txtBuscar != null ? txtBuscar.Text.Trim() : "";

            FiltrarGrilla(filtroActual);
        }

        // Se encarga de mostrar la lista filtrada y armar las columnas
        private void FiltrarGrilla(string filtro)
        {
            dgvProductos.DataSource = null;
            dgvProductos.Columns.Clear();

            string f = filtro.ToLower();

            // LINQ para filtrar la lista en memoria (Código, Nombre o Descripción)
            var listaFiltrada = _productosActuales.Where(p =>
                (p.Codigo != null && p.Codigo.ToLower().Contains(f)) ||
                (p.Nombre != null && p.Nombre.ToLower().Contains(f)) ||
                (p.Descripcion != null && p.Descripcion.ToLower().Contains(f))
            ).ToList();

            dgvProductos.DataSource = listaFiltrada;

            if (_viendoInactivos)
            {
                AgregarBotonActivarGrilla();
            }
            else
            {
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
                    _productoOriginalEdicion = prod;

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
                    if (MessageBox.Show($"¿Desea dar de baja '{prod.Nombre}'?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            _negocio.EliminarProducto(prod);
                            LimpiarCampos();
                            CargarGrillaProductos(); // Refresca y aplica el filtro si quedó escrito
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
                            MessageBox.Show("Producto reactivado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            CargarGrillaProductos();
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
                        cambios.Add($"- Categoría ID: {_productoOriginalEdicion.IdCategoria} -> {cboCategoria.SelectedValue}");
                    }

                    if (cambios.Count == 0)
                    {
                        MessageBox.Show("No se detectaron cambios en el producto.", "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCampos();
                        return;
                    }

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

        // EVENTOS DE LOS BOTONES DE FILTRO ESTADO 
        private void buttonActivos_Click(object sender, EventArgs e)
        {
            _viendoInactivos = false;
            CargarGrillaProductos();
        }

        private void buttonInactivos_Click(object sender, EventArgs e)
        {
            _viendoInactivos = true;
            CargarGrillaProductos();
        }

        // EVENTO DEL BUSCADOR DE PRODUCTOS
        private void textBoxBuscarProducto_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox txt)
            {
                FiltrarGrilla(txt.Text.Trim());
            }
        }

        private void LPrecio_Click(object sender, EventArgs e)
        {

        }
    }
}