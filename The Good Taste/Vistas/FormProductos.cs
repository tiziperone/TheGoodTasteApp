using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace The_Good_Taste.Vistas
{
    public partial class FormProductos : Form
    {
        public FormProductos()
        {
            InitializeComponent();
            CargarCategorias();
            CargarTablaProductos();
            CargarTablaEliminados();
        }

        private void CargarCategorias()
        {
            // Llenar ComboBox de categorías: 1 - Bondiolas, 2 - Milanesas, 3 - Pastas
            cboCategoria.Items.Add(new { Text = "Bondiolas", Value = 1 });
            cboCategoria.Items.Add(new { Text = "Milanesas", Value = 2 });
            cboCategoria.Items.Add(new { Text = "Pastas", Value = 3 });
            cboCategoria.DisplayMember = "Text";
            cboCategoria.ValueMember = "Value";
        }

        private void CargarTablaProductos()
        {
            // Asigna los productos activos a dgvProductos (DataGridView)
            // dgvProductos.DataSource = ProductoDatos.ObtenerActivos();

            // Formato visual para alertar Stock Bajo (Igual a tu badge bg-warning/bg-danger de Laravel)
            dgvProductos.CellFormatting += (s, e) =>
            {
                if (dgvProductos.Columns[e.ColumnIndex].Name == "Stock")
                {
                    int stock = Convert.ToInt32(e.Value);
                    if (stock <= 0)
                    {
                        e.CellStyle.BackColor = Color.IndianRed;
                        e.CellStyle.ForeColor = Color.White;
                    }
                    else if (stock <= 5) // stock_minimo
                    {
                        e.CellStyle.BackColor = Color.Orange;
                        e.CellStyle.ForeColor = Color.Black;
                    }
                }
            };
        }

        private void CargarTablaEliminados()
        {
            // Asigna el historial de eliminados (deleted_at)
            // dgvProductosEliminados.DataSource = ProductoDatos.ObtenerEliminados();
        }

        private void btnGuardarProducto_Click(object sender, EventArgs e)
        {
            // Lógica para guardar un nuevo producto (Modal Agregar Producto en Laravel)
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            // Lógica para reactivar un producto (Soft Delete restaurar)
        }
    }
}