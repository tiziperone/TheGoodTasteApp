using System;
using System.Drawing;
using System.Windows.Forms;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormPrincipal : Form //Clase principal del formulario que maneja la navegación y la interfaz de usuario
    {
        private readonly UsuarioSistema _usuarioActual;
        private Button _botonActivo = null;

        //Colores para el tema visual
        private readonly Color ColorFondoPanel = Color.FromArgb(35, 25, 20);   // Marrón oscuro
        private readonly Color ColorBotonBase = Color.FromArgb(60, 42, 33);    // Marrón café
        private readonly Color ColorBotonActivo = Color.FromArgb(180, 130, 40); // Dorado
        private readonly Color ColorTextoBoton = Color.White;

        public FormPrincipal()
        {
            InitializeComponent();
            this.KeyPreview = true; // Activa la escucha de atajos globales (F1-F6 y ESC)
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

        private void DeshabilitarBoton(Button btn)
        {
            if (btn != null)
            {
                btn.Enabled = false; // Bloquea los clics

                // Efecto visual de apagado/transparencia
                btn.BackColor = ColorFondoPanel; // Se funde con el fondo del panel
                btn.ForeColor = Color.DimGray;   // Letras grises
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
                    // El Admin solo ve Reportes y Usuarios
                    DeshabilitarBoton(btnProductos);
                    DeshabilitarBoton(btnClientes);
                    DeshabilitarBoton(btnVentas);
                    break;

                case RolUsuario.Gerente:
                    // Bloqueamos los módulos que no debe usar
                    DeshabilitarBoton(btnUsuarios);
                    DeshabilitarBoton(btnVentas);
                    DeshabilitarBoton(btnProductos);
                    DeshabilitarBoton(btnClientes);
                    // Reportes queda habilitado
                    break;

                case RolUsuario.Vendedor:
                    // Bloqueamos Usuarios y Reportes
                    DeshabilitarBoton(btnUsuarios);
                    DeshabilitarBoton(btnReportes);
                    // Productos, Clientes y Ventas quedan habilitados
                    break;
            }
        }

        // Atajos de teclado (F1 a F6 y ESC)
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
                case Keys.Escape:
                    if (btnSalir != null && btnSalir.Enabled) btnSalir.PerformClick();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ResaltarBotonActivo(Button botonPresionado)
        {
            if (panelSuperior == null) return;

            panelSuperior.BackColor = ColorFondoPanel;

            foreach (Control control in panelSuperior.Controls)
            {
                if (control is Button btn && btn != btnSalir)
                {
                    if (btn.Enabled)
                    {
                        btn.BackColor = ColorBotonBase;
                        btn.ForeColor = ColorTextoBoton;
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderSize = 0;
                        btn.Font = new Font(btn.Font, FontStyle.Regular);
                    }
                }
            }

            // Estilo sutil para el botón Salir
            if (btnSalir != null && btnSalir.Enabled)
            {
                btnSalir.BackColor = Color.FromArgb(140, 40, 40);
                btnSalir.ForeColor = ColorTextoBoton;
                btnSalir.FlatStyle = FlatStyle.Flat;
                btnSalir.FlatAppearance.BorderSize = 0;
            }

            // Marca la pestaña activa (si tiene permiso)
            _botonActivo = botonPresionado;
            if (_botonActivo != null && _botonActivo.Enabled)
            {
                _botonActivo.BackColor = ColorBotonActivo;
                _botonActivo.Font = new Font(_botonActivo.Font, FontStyle.Bold);
            }
        }

        private void AbrirFormularioEnPanel(Form formularioHijo, Button botonPresionado)
        {
            // Si el botón no está habilitado, cancelamos la apertura
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