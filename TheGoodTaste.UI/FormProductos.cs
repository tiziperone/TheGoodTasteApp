using System;
using System.Collections.Generic;
using System.Windows.Forms;
using The_Good_Taste.Datos;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormProductos : Form
    {
        public FormProductos()
        {
            InitializeComponent();
        }

        private void FormProductos_Load(object sender, EventArgs e)
        {
            CargarCategorias();
            CargarGrillaProductos();
            TemaVisual.AplicarEstilo(this);
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

        private void CargarGrillaProductos()
        {
            try
            {
                List<Producto> lista = ProductoDatos.ObtenerActivos();
                dgvProductos.DataSource = null;
                dgvProductos.DataSource = lista;

                if (dgvProductos.Columns["IdProducto"] != null)
                    dgvProductos.Columns["IdProducto"].Visible = false;

                if (dgvProductos.Columns["UrlImagen"] != null)
                    dgvProductos.Columns["UrlImagen"].Visible = false;

                if (dgvProductos.Columns["DeletedAt"] != null)
                    dgvProductos.Columns["DeletedAt"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Por favor complete al menos el código y el nombre del producto.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("Ingrese un precio válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Producto nuevoProducto = new Producto
            {
                Codigo = txtCodigo.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Descripcion = txtDescripcion.Text.Trim(),
                Precio = precio,
                Stock = (int)nudStock.Value,
                StockMinimo = 5,
                IdCategoria = (int)cboCategoria.SelectedValue
            };

            try
            {
                bool resultado = ProductoDatos.Insertar(nuevoProducto);

                if (resultado)
                {
                    MessageBox.Show("Producto registrado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrillaProductos();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el producto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtCodigo.Focus();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void LNroCategoria_Click(object sender, EventArgs e)
        {

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

        }
    }
}