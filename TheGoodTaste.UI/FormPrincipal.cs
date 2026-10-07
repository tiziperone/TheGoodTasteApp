using System;
using System.Drawing;
using System.Drawing.Imaging;
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

        // Color de la barra superior para usarlo de fondo en los botones
        private readonly Color ColorBarraSuperior = Color.FromArgb(25, 18, 14);

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

            if (panelSuperior != null)
            {
                panelSuperior.BackColor = ColorBarraSuperior;
                panelSuperior.Dock = DockStyle.Top;
                panelSuperior.Height = 125;

                // CLAVE: Quitamos el fondo cuadrado a todos los botones habilitados desde el inicio
                foreach (Control control in panelSuperior.Controls)
                {
                    if (control is Button btn && btn.Enabled)
                    {
                        btn.BackColor = ColorBarraSuperior; // Se funde con la barra
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderSize = 0;
                        btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 30, 24); // Leve brillo al pasar el mouse
                    }
                }
            }

            if (panelContenedor != null)
            {
                panelContenedor.Dock = DockStyle.Fill;
                panelContenedor.BringToFront();
            }

            if (pbLogoInicio != null)
            {
                pbLogoInicio.BackColor = ColorFondoPanel;
                pbLogoInicio.SizeMode = PictureBoxSizeMode.Zoom;
            }

            CentrarYAmpliarLogo();
        }

        private Image HacerImagenTransparente(Image imagenOriginal, float opacidad)
        {
            if (imagenOriginal == null) return null;

            Bitmap bmp = new Bitmap(imagenOriginal.Width, imagenOriginal.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMatrix matrizColor = new ColorMatrix();
                matrizColor.Matrix33 = opacidad;

                using (ImageAttributes atributos = new ImageAttributes())
                {
                    atributos.SetColorMatrix(matrizColor, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    g.DrawImage(imagenOriginal, new Rectangle(0, 0, bmp.Width, bmp.Height),
                                0, 0, imagenOriginal.Width, imagenOriginal.Height,
                                GraphicsUnit.Pixel, atributos);
                }
            }
            return bmp;
        }

        private void DeshabilitarBoton(Button btn)
        {
            if (btn != null)
            {
                btn.Enabled = false;

                btn.BackColor = ColorBarraSuperior;
                btn.ForeColor = Color.FromArgb(90, 90, 90);
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;

                if (btn.Image != null)
                {
                    btn.Image = HacerImagenTransparente(btn.Image, 0.3f);
                }

                if (btn.BackgroundImage != null)
                {
                    btn.BackgroundImage = HacerImagenTransparente(btn.BackgroundImage, 0.3f);
                }
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
                        btn.BackColor = ColorBotonActivo;
                        btn.ForeColor = Color.Black;
                    }
                    else
                    {
                        btn.BackColor = ColorBarraSuperior;
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
            // Se quitó el DockStyle.Fill de aquí

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

            // SOLUCIÓN AL ERROR: 
            // 1. Mostrar el formulario primero para que dibuje el DataGridView
            formularioHijo.Show();
            // 2. Acoplar al panel después de mostrar
            formularioHijo.Dock = DockStyle.Fill;
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

        private void CentrarYAmpliarLogo()
        {
            if (pbLogoInicio != null && pbLogoInicio.Visible)
            {
                int anchoDeseado = Math.Min(panelContenedor.ClientSize.Width - 80, 650);
                int altoDeseado = Math.Min(panelContenedor.ClientSize.Height - 80, 450);

                pbLogoInicio.Width = Math.Max(anchoDeseado, 300);
                pbLogoInicio.Height = Math.Max(altoDeseado, 200);

                pbLogoInicio.Left = (panelContenedor.ClientSize.Width - pbLogoInicio.Width) / 2;
                pbLogoInicio.Top = (panelContenedor.ClientSize.Height - pbLogoInicio.Height) / 2;
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