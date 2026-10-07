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

        private readonly Color ColorFondoPanel = Color.FromArgb(35, 25, 20);   // Marrón oscuro
        private readonly Color ColorBotonActivo = Color.FromArgb(180, 130, 40); // Dorado
        private readonly Color ColorTextoBoton = Color.White;

        public FormPrincipal()
        {
            InitializeComponent();
            this.KeyPreview = true;
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
                this.Text = $"Bienvenido a The Good Taste - Usuario: {_usuarioActual.Username} [{_usuarioActual.Rol}]";
                ConfigurarPermisosPorRol();
            }

            // Solo aseguramos que el panel superior cubra todo el ancho al maximizar
            if (panelSuperior != null)
            {
                panelSuperior.BackColor = Color.FromArgb(25, 18, 14);
                panelSuperior.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }

            // Configuramos el logo para que luzca limpio, sin bordes negros y con zoom prolijo
            if (pbLogoInicio != null)
            {
                pbLogoInicio.BackColor = ColorFondoPanel;
                pbLogoInicio.SizeMode = PictureBoxSizeMode.Zoom;
            }

            CentrarYAmpliarLogo();
        }

        private void CentrarYAmpliarLogo()
        {
            if (pbLogoInicio != null && pbLogoInicio.Visible)
            {
                // Calculamos un tamaño proporcional y grande para que destaque en el centro
                int anchoDeseado = Math.Min(panelContenedor.ClientSize.Width - 80, 650);
                int altoDeseado = Math.Min(panelContenedor.ClientSize.Height - 80, 450);

                pbLogoInicio.Width = Math.Max(anchoDeseado, 300);
                pbLogoInicio.Height = Math.Max(altoDeseado, 200);

                pbLogoInicio.Left = (panelContenedor.ClientSize.Width - pbLogoInicio.Width) / 2;
                pbLogoInicio.Top = (panelContenedor.ClientSize.Height - pbLogoInicio.Height) / 2;
            }
        }

        private void DeshabilitarBoton(Button btn)
        {
            if (btn != null)
            {
                btn.Enabled = false;
                btn.BackColor = Color.FromArgb(25, 18, 14);
                btn.ForeColor = Color.DimGray;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
            }
        }

        private void ConfigurarPermisosPorRol()
        {
            if (_usuarioActual == null) return;

            switch (_usuarioActual.Rol)
            {
                case RolUsuario.Admin:
                    DeshabilitarBoton(btnProductos);
                    DeshabilitarBoton(btnClientes);
                    DeshabilitarBoton(btnVentas);
                    break;

                case RolUsuario.Gerente:
                    DeshabilitarBoton(btnUsuarios);
                    DeshabilitarBoton(btnVentas);
                    DeshabilitarBoton(btnClientes);
                    break;

                case RolUsuario.Vendedor:
                    DeshabilitarBoton(btnUsuarios);
                    DeshabilitarBoton(btnReportes);
                    break;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F1:
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
                    if (btnReportes != null && btnReportes.Enabled) btnReportes.PerformClick();
                    return true;
                case Keys.F7:
                    if (buttonCerrarSesion != null && buttonCerrarSesion.Enabled) buttonCerrarSesion.PerformClick();
                    return true;
                case Keys.Escape:
                    if (btnSalir != null && btnSalir.Enabled) btnSalir.PerformClick();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ResaltarBotonActivo(Button botonPresionado)
        {
            if (panelSuperior == null) return;

            _botonActivo = botonPresionado;
            foreach (Control control in panelSuperior.Controls)
            {
                if (control is Button btn && btn != btnSalir && btn.Enabled)
                {
                    if (btn == _botonActivo)
                    {
                        btn.BackColor = ColorBotonActivo; // Dorado al seleccionar
                        btn.ForeColor = Color.Black;
                    }
                    else
                    {
                        btn.BackColor = Color.FromArgb(50, 35, 28);
                        btn.ForeColor = Color.White;
                    }
                }
            }
        }

        private void AbrirFormularioEnPanel(Form formularioHijo, Button botonPresionado)
        {
            if (botonPresionado != null && !botonPresionado.Enabled) return;

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
                    CentrarYAmpliarLogo();
                }
                ResaltarBotonActivo(null);
            };

            formularioHijo.SuspendLayout();
            this.panelContenedor.Controls.Add(formularioHijo);
            this.panelContenedor.Tag = formularioHijo;
            formularioHijo.BringToFront();
            formularioHijo.ResumeLayout(true);
            formularioHijo.Show();
        }

        public void btnProductos_Click(object sender, EventArgs e) => AbrirFormularioEnPanel(new FormProductos(), (Button)sender);
        public void btnClientes_Click(object sender, EventArgs e) => AbrirFormularioEnPanel(new FormClientes(), (Button)sender);
        public void btnVentas_Click(object sender, EventArgs e) => AbrirFormularioEnPanel(new FormPuntoVenta(_usuarioActual.DNI), (Button)sender);
        public void btnUsuarios_Click(object sender, EventArgs e) => AbrirFormularioEnPanel(new FormUsuarios(), (Button)sender);
        public void btnReportes_Click(object sender, EventArgs e) => AbrirFormularioEnPanel(new FormReportes(_usuarioActual), (Button)sender);

        public void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de que desea salir del sistema?", "Salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void panelContenedor_Resize(object sender, EventArgs e) => CentrarYAmpliarLogo();
        private void panelContenedor_Paint(object sender, PaintEventArgs e) { }
        private void pbLogoInicio_Click(object sender, EventArgs e) { }

        private void buttonCerrarSesion_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro que desea cerrar la sesión actual?", "Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        private void panelSuperior_Paint(object sender, PaintEventArgs e) { }

        private void label2_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
    }
}