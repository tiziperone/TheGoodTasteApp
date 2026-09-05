using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormProductos : Form
    {
        // Lista estática en memoria para mantener los datos mientras no haya base de datos
        private static List<Producto> listaProductosEnMemoria = new List<Producto>();
        private static int contadorId = 1;

        public FormProductos()
        {
            InitializeComponent();
            ConfigurarEventos();
        }

        private void FormProductos_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            CargarCategorias();
            CargarProductosIniciales();
            CargarGrillaProductos();
            ActualizarEstadoBotones();
        }

        private void ConfigurarEventos()
        {
            // Restricción de entrada
            txtPrecio.KeyPress += SoloNumerosYDecimal_KeyPress;

            // Control de habilitación de botones
            txtCodigo.TextChanged += Control_Modificado;
            txtNombre.TextChanged += Control_Modificado;
            txtDescripcion.TextChanged += Control_Modificado;
            txtPrecio.TextChanged += Control_Modificado;
            nudStock.ValueChanged += Control_Modificado;
            cboCategoria.SelectedIndexChanged += Control_Modificado;

            // Selección en la grilla para habilitar eliminación
            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;
        }

        private void CargarCategorias()
        {
            var categorias = new Dictionary<int, string>
            {
                { 1, "Pastas" },
                { 2, "Bondiolas" },
                { 3, "Milanesas" }
            };

            cboCategoria.DataSource = new BindingSource(categorias, null);
            cboCategoria.DisplayMember = "Value";
            cboCategoria.ValueMember = "Key";
        }

        private void CargarProductosIniciales()
        {
            // Solo carga datos de prueba la primera vez que se abre la pantalla
            if (listaProductosEnMemoria.Count == 0)
            {
                listaProductosEnMemoria.Add(new Producto
                {
                    IdProducto = contadorId++,
                    Codigo = "PROD01",
                    Nombre = "Bondiola Artesanal",
                    Descripcion = "Bondiola curada al vacío",
                    Precio = 6500.00m,
                    Stock = 12,
                    StockMinimo = 5,
                    IdCategoria = 2
                });

                listaProductosEnMemoria.Add(new Producto
                {
                    IdProducto = contadorId++,
                    Codigo = "PROD02",
                    Nombre = "Ravioles Caseros",
                    Descripcion = "Plancha de 24 unidades",
                    Precio = 3800.00m,
                    Stock = 20,
                    StockMinimo = 5,
                    IdCategoria = 1
                });
            }
        }

        private void CargarGrillaProductos()
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = listaProductosEnMemoria.ToList();

            // Ocultar columnas internas si existen en la clase Producto
            if (dgvProductos.Columns["IdProducto"] != null)
                dgvProductos.Columns["IdProducto"].Visible = false;

            if (dgvProductos.Columns["DeletedAt"] != null)
                dgvProductos.Columns["DeletedAt"].Visible = false;

            dgvProductos.ClearSelection();
        }

        // =======================
        // FILTRADO Y ESTADOS
        // =======================
        private void SoloNumerosYDecimal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
                return;
            }

            if ((e.KeyChar == '.' || e.KeyChar == ',') && (txtPrecio.Text.Contains(".") || txtPrecio.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }

        private void Control_Modificado(object sender, EventArgs e)
        {
            ActualizarEstadoBotones();
        }

        private void DgvProductos_SelectionChanged(object sender, EventArgs e)
        {
            btnEliminar.Enabled = dgvProductos.SelectedRows.Count > 0;
        }

        private void ActualizarEstadoBotones()
        {
            bool hayTexto = !string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                            !string.IsNullOrWhiteSpace(txtNombre.Text) ||
                            !string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                            !string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                            nudStock.Value > 0;

            bool obligatoriosCompletos = !string.IsNullOrWhiteSpace(txtCodigo.Text) &&
                                         !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                                         !string.IsNullOrWhiteSpace(txtPrecio.Text) &&
                                         cboCategoria.SelectedIndex != -1;

            btnLimpiar.Enabled = hayTexto;
            btnGuardar.Enabled = obligatoriosCompletos;
            btnEliminar.Enabled = dgvProductos.SelectedRows.Count > 0;
        }

        // =======================
        // ACCIONES DE BOTONES
        // =======================
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string precioTexto = txtPrecio.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(precioTexto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Ingrese un precio válido mayor a 0.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrecio.Focus();
                return;
            }

            // Validar que el código no esté duplicado en memoria
            if (listaProductosEnMemoria.Any(p => p.Codigo.Equals(txtCodigo.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Ya existe un producto registrado con ese código.", "Código Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigo.Focus();
                return;
            }

            Producto nuevoProducto = new Producto
            {
                IdProducto = contadorId++,
                Codigo = txtCodigo.Text.Trim().ToUpper(),
                Nombre = txtNombre.Text.Trim(),
                Descripcion = txtDescripcion.Text.Trim(),
                Precio = precio,
                Stock = (int)nudStock.Value,
                StockMinimo = 5,
                IdCategoria = (int)cboCategoria.SelectedValue
            };

            listaProductosEnMemoria.Add(nuevoProducto);

            MessageBox.Show("Producto registrado con éxito (en memoria).", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
            CargarGrillaProductos();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un producto de la grilla para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Producto productoSeleccionado = (Producto)dgvProductos.SelectedRows[0].DataBoundItem;

            DialogResult confirmacion = MessageBox.Show($"¿Desea eliminar el producto '{productoSeleccionado.Nombre}'?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion == DialogResult.Yes)
            {
                listaProductosEnMemoria.Remove(productoSeleccionado);
                CargarGrillaProductos();
                LimpiarCampos();
                MessageBox.Show("Producto eliminado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            nudStock.Value = 0;
            if (cboCategoria.Items.Count > 0)
                cboCategoria.SelectedIndex = 0;

            dgvProductos.ClearSelection();
            ActualizarEstadoBotones();
            txtCodigo.Focus();
        }
    }
}