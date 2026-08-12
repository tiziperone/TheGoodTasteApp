using System;
using System.Drawing;
using System.Windows.Forms;

namespace The_Good_Taste.Vistas
{
    public partial class FormMenuPrincipal : Form
    {
        private Form formularioActivo = null;

        public FormMenuPrincipal()
        {
            InitializeComponent();
            ConfigurarDiseñoDark();
        }

        private void ConfigurarDiseñoDark()
        {
            this.BackColor = Color.FromArgb(33, 37, 41); // #212529 (bg-dark)
            this.ForeColor = Color.White;
            this.Text = "The Good Taste - Sistema Punto de Venta (POS)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
        }

        // Método para abrir formularios secundarios dentro del panel contenedor
        private void AbrirFormularioHijo(Form formHijo)
        {
            if (formularioActivo != null)
                formularioActivo.Close();

            formularioActivo = formHijo;
            formHijo.TopLevel = false;
            formHijo.FormBorderStyle = FormBorderStyle.None;
            formHijo.Dock = DockStyle.Fill;
            panelContenedor.Controls.Add(formHijo);
            panelContenedor.Tag = formHijo;
            formHijo.BringToFront();
            formHijo.Show();
        }

        private void btnGestionClientes_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FormClientes());
        }

        private void btnGestionProductos_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FormProductos());
        }

        private void btnNuevasVentas_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FormPuntoVenta());
        }

        private void btnGestionConsultas_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FormConsultas());
        }
    }
}
