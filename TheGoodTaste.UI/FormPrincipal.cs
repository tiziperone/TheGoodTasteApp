using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormPrincipal : Form
    {
        private readonly UsuarioSistema _usuarioActual;

        // Constructor por defecto (requerido por el diseñador de Windows Forms)
        public FormPrincipal()
        {
            InitializeComponent();
        }

        // Constructor que recibe la sesión activa desde Program.cs
        public FormPrincipal(UsuarioSistema usuario) : this()
        {
            _usuarioActual = usuario;
        }

        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);

            if (_usuarioActual != null)
            {
                this.Text = $"Bienvenido a The Good Taste - Usuario: {_usuarioActual.NombreUsuario} [{_usuarioActual.Rol}]";
                ConfigurarPermisosPorRol();
            }
        }

        private void ConfigurarPermisosPorRol()
        {
            switch (_usuarioActual.Rol)
            {
                case RolUsuario.Admin:
                    // Admin ve todo (no se oculta nada)
                    break;

                case RolUsuario.Gerente:
                    // Gerente ve catálogo, clientes y ventas, pero no gestiona usuarios
                    usuariosToolStripMenuItem.Visible = false;
                    break;

                case RolUsuario.Vendedor:
                    // Vendedor solo atiende clientes y registra ventas
                    usuariosToolStripMenuItem.Visible = false;
                    productosToolStripMenuItem.Visible = false;
                    break;
            }
        }

        // Método auxiliar para incrustar formularios dentro del panel
        private void AbrirFormularioEnPanel(Form formularioHijo)
        {
            // Cierra y libera recursos del formulario anterior si existe
            if (this.panelContenedor.Controls.Count > 0)
            {
                Control controlActual = this.panelContenedor.Controls[0];
                this.panelContenedor.Controls.RemoveAt(0);
                controlActual.Dispose(); // Libera la memoria del form anterior
            }

            formularioHijo.TopLevel = false;
            formularioHijo.FormBorderStyle = FormBorderStyle.None;
            formularioHijo.Dock = DockStyle.Fill;

            this.panelContenedor.Controls.Add(formularioHijo);
            this.panelContenedor.Tag = formularioHijo;
            formularioHijo.Show();
        }

        private void productosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FormProductos());
        }

        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FormClientes());
        }

        private void ventasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FormPuntoVenta());
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FormUsuarios());
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panelContenedor_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}