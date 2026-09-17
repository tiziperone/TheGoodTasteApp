using System;
using System.Drawing;
using System.Windows.Forms;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormPrincipal : Form
    {
        private readonly UsuarioSistema _usuarioActual;
        private Button _botonActivo = null;

        public FormPrincipal()
        {
            InitializeComponent();
            this.KeyPreview = true; // Activa la escucha de atajos globales (F1-F5 y ESC)
        }

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

            CentrarLogo();
        }

        private void ConfigurarPermisosPorRol()
        {
            if (_usuarioActual == null) return;

            switch (_usuarioActual.Rol)
            {
                case RolUsuario.Admin:
                    // El Admin tiene acceso total por defecto
                    break;

                case RolUsuario.Gerente:
                    // Ocultamos Usuarios
                    if (btnUsuarios != null) btnUsuarios.Visible = false;
                    break;

                case RolUsuario.Vendedor:
                    // Ocultamos Usuarios y Reportes
                    if (btnUsuarios != null) btnUsuarios.Visible = false;
                    if (btnReportes != null) btnReportes.Visible = false;
                    if (btnProductos != null) btnProductos.Visible = true;
                    break;
            }
        }

        // Atajos de teclado (F1 a F5 y ESC)
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F1:
                    // Verificamos .Enabled en lugar de .Visible para bloquear el atajo si no hay permisos
                    if (btnProductos != null && btnProductos.Enabled) btnProductos.PerformClick();
                    return true;
                case Keys.F2:
                    if (btnClientes != null && btnClientes.Enabled) btnClientes.PerformClick();
                    return true;
                case Keys.F3:
                    if (btnVentas != null && btnVentas.Enabled) btnVentas.PerformClick();
                    return true;
                case Keys.F4:
                    if (btnUsuarios != null && btnUsuarios.Enabled) btnUsuarios.PerformClick();
                    return true;
                case Keys.F6:
                    if (btnReportes != null && btnReportes.Visible) btnReportes.PerformClick();
                    return true;
                case Keys.Escape:
                    if (btnSalir != null && btnSalir.Enabled) btnSalir.PerformClick();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // CONSTANTES DE COLOR
        private readonly Color ColorFondoPanel = Color.FromArgb(35, 25, 20);   // Marrón oscuro
        private readonly Color ColorBotonBase = Color.FromArgb(60, 42, 33);    // Marrón café
        private readonly Color ColorBotonActivo = Color.FromArgb(180, 130, 40); // Dorado
        private readonly Color ColorTextoBoton = Color.White;

        private void ResaltarBotonActivo(Button botonPresionado)
        {
            if (panelSuperior == null) return;

            panelSuperior.BackColor = ColorFondoPanel;

            // Restablece el estilo base de los botones del panel
            foreach (Control control in panelSuperior.Controls)
            {
                if (control is Button btn && btn != btnSalir)
                {
                    btn.BackColor = ColorBotonBase;
                    btn.ForeColor = ColorTextoBoton;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Font = new Font(btn.Font, FontStyle.Regular);
                }
            }

            // Estilo sutil para el botón Salir
            if (btnSalir != null)
            {
                btnSalir.BackColor = Color.FromArgb(140, 40, 40);
                btnSalir.ForeColor = ColorTextoBoton;
                btnSalir.FlatStyle = FlatStyle.Flat;
                btnSalir.FlatAppearance.BorderSize = 0;
            }

            // Marca la pestaña activa
            _botonActivo = botonPresionado;
            if (_botonActivo != null)
            {
                _botonActivo.BackColor = ColorBotonActivo;
                _botonActivo.Font = new Font(_botonActivo.Font, FontStyle.Bold);
            }
        }

        private void AbrirFormularioEnPanel(Form formularioHijo, Button botonPresionado)
        {
            ResaltarBotonActivo(botonPresionado);

            if (pbLogoInicio != null)
            {
                pbLogoInicio.Visible = false;
            }

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

            formularioHijo.FormClosed += (s, args) =>
            {
                if (pbLogoInicio != null)
                {
                    pbLogoInicio.Visible = true;
                    CentrarLogo();
                }
                ResaltarBotonActivo(null);
            };

            this.panelContenedor.Controls.Add(formularioHijo);
            this.panelContenedor.Tag = formularioHijo;
            formularioHijo.BringToFront();
            formularioHijo.Show();
        }

        // Métodos vinculados a cada botón
        public void btnProductos_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FormProductos(), (Button)sender);
        }

        public void btnClientes_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FormClientes(), (Button)sender);
        }

        public void btnVentas_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FormPuntoVenta(), (Button)sender);
        }

        public void btnUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FormUsuarios(), (Button)sender);
        }

        public void btnReportes_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new FormReporteGerente(), (Button)sender);
        }

        public void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de que desea cerrar la sesión?", "Cerrar Sesión",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void CentrarLogo()
        {
            if (pbLogoInicio != null && pbLogoInicio.Visible)
            {
                pbLogoInicio.Left = (panelContenedor.ClientSize.Width - pbLogoInicio.Width) / 2;
                pbLogoInicio.Top = (panelContenedor.ClientSize.Height - pbLogoInicio.Height) / 2;
            }
        }

        private void panelContenedor_Resize(object sender, EventArgs e)
        {
            CentrarLogo();
        }

        private void panelContenedor_Paint(object sender, PaintEventArgs e) { }
        private void pbLogoInicio_Click(object sender, EventArgs e) { }
    }
}