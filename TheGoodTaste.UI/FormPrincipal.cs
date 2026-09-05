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

            // Centra la imagen de bienvenida al cargar el formulario
            CentrarLogo();
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

        // Mantiene el logo centrado si la ventana cambia de tamaño o se maximiza
        private void CentrarLogo()
        {
            if (pbLogoInicio != null && pbLogoInicio.Visible)
            {
                pbLogoInicio.Left = (panelContenedor.ClientSize.Width - pbLogoInicio.Width) / 2;
                pbLogoInicio.Top = (panelContenedor.ClientSize.Height - pbLogoInicio.Height) / 2;
            }
        }

        // Método auxiliar para incrustar formularios dentro del panel
        private void AbrirFormularioEnPanel(Form formularioHijo)
        {
            // Oculta la imagen de bienvenida para mostrar el módulo
            if (pbLogoInicio != null)
            {
                pbLogoInicio.Visible = false;
            }

            // Cierra y libera memoria del formulario previo sin borrar el PictureBox
            for (int i = panelContenedor.Controls.Count - 1; i >= 0; i--)
            {
                Control control = panelContenedor.Controls[i];
                if (control is Form formPrevio)
                {
                    panelContenedor.Controls.RemoveAt(i);
                    formPrevio.Dispose();
                }
            }

            formularioHijo.TopLevel = false;
            formularioHijo.FormBorderStyle = FormBorderStyle.None;
            formularioHijo.Dock = DockStyle.Fill;

            // Vuelve a mostrar el logo si el formulario hijo se cierra
            formularioHijo.FormClosed += (s, args) =>
            {
                if (pbLogoInicio != null)
                {
                    pbLogoInicio.Visible = true;
                    CentrarLogo();
                }
            };

            this.panelContenedor.Controls.Add(formularioHijo);
            this.panelContenedor.Tag = formularioHijo;
            formularioHijo.BringToFront();
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

        private void panelContenedor_Resize(object sender, EventArgs e)
        {
            CentrarLogo();
        }
    }
}