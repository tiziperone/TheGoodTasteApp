using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormReporte : Form
    {
        private readonly UsuarioSistema _usuarioActual;

        // Controles de interfaz
        private Panel pnlSuperior;
        private Panel pnlContenido;
        private ComboBox cmbTipoReporte;
        private Button btnFiltrar;
        private DataGridView dgvReportes;
        private Chart chartReportes;


        public FormReporte()
        {
            InitializeComponent();
        }

        public FormReporte(UsuarioSistema usuario) : this()
        {
            _usuarioActual = usuario;
        }

        private void FormReporteGerente_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);

            // 1. Maquetado de la vista por código
            ConstruirEstructuraVistas();

            // 2. Configurar opciones y gráficos según el rol
            CargarOpcionesPorRol();
            ConfigurarVistaSegunRol();
        }

        private void ConstruirEstructuraVistas()
        {
            this.Controls.Clear();
            this.Padding = new Padding(15);
            this.BackColor = Color.FromArgb(35, 25, 20); // Fondo del tema

            // --- PANEL DE FILTROS SUPERIOR ---
            pnlSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(45, 32, 26),
                Padding = new Padding(10)
            };

            Label lblFiltro = new Label
            {
                Text = "Tipo de Reporte:",
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 15),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            cmbTipoReporte = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 250,
                Location = new Point(130, 12),
                Font = new Font("Segoe UI", 9.5f)
            };

            btnFiltrar = new Button
            {
                Text = "Generar Reporte",
                BackColor = Color.FromArgb(180, 130, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Width = 140,
                Height = 28,
                Location = new Point(390, 10),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            };
            btnFiltrar.FlatAppearance.BorderSize = 0;
            btnFiltrar.Click += BtnFiltrar_Click;

            pnlSuperior.Controls.Add(lblFiltro);
            pnlSuperior.Controls.Add(cmbTipoReporte);
            pnlSuperior.Controls.Add(btnFiltrar);

            // --- PANEL CENTRAL (Grilla a la Izquierda + Gráfico a la Derecha) ---
            pnlContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };

            // Gráfico (Chart)
            chartReportes = new Chart
            {
                Dock = DockStyle.Right,
                Width = 450,
                BackColor = Color.FromArgb(45, 32, 26)
            };

            ChartArea ca = new ChartArea("AreaReporte") { BackColor = Color.Transparent };
            ca.AxisX.LabelStyle.ForeColor = Color.White;
            ca.AxisY.LabelStyle.ForeColor = Color.White;
            ca.AxisX.LineColor = Color.LightGray;
            ca.AxisY.LineColor = Color.LightGray;
            chartReportes.ChartAreas.Add(ca);

            // DataGridView (Tabla vacía lista para la BD)
            dgvReportes = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(45, 32, 26),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            pnlContenido.Controls.Add(dgvReportes);
            pnlContenido.Controls.Add(chartReportes);

            // Agregar al Formulario
            this.Controls.Add(pnlContenido);
            this.Controls.Add(pnlSuperior);
        }

        private void CargarOpcionesPorRol()
        {
            cmbTipoReporte.Items.Clear();

            if (_usuarioActual?.Rol == RolUsuario.Vendedor)
            {
                cmbTipoReporte.Items.Add("Mis Ventas por Período");
                cmbTipoReporte.Items.Add("Mis Productos Más Vendidos");
            }
            else // Gerente / Admin
            {
                cmbTipoReporte.Items.Add("Recaudación Global");
                cmbTipoReporte.Items.Add("Ventas por Vendedor");
                cmbTipoReporte.Items.Add("Top Productos Más Vendidos");
            }

            if (cmbTipoReporte.Items.Count > 0)
                cmbTipoReporte.SelectedIndex = 0;
        }

        private void ConfigurarVistaSegunRol()
        {
            if (_usuarioActual == null) return;

            if (_usuarioActual.Rol == RolUsuario.Vendedor)
            {
                this.Text = $"Reportes - Vendedor: {_usuarioActual.NombreUsuario}";
                ConfigurarGraficoVendedor();
            }
            else // Gerente / Admin
            {
                this.Text = "Reportes Estratégicos - Gerencia";
                ConfigurarGraficoGerente();
            }
        }

        private void ConfigurarGraficoGerente()
        {
            chartReportes.Series.Clear();
            chartReportes.Titles.Clear();

            Title t = chartReportes.Titles.Add("Recaudación General");
            t.ForeColor = Color.White;
            t.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            // Estructura de Barras/Columnas para Gerente
            Series s = new Series("SerieGerente")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.FromArgb(180, 130, 40) // Dorado
            };
            chartReportes.Series.Add(s);
        }

        private void ConfigurarGraficoVendedor()
        {
            chartReportes.Series.Clear();
            chartReportes.Titles.Clear();

            Title t = chartReportes.Titles.Add("Mis Ventas por Categoría");
            t.ForeColor = Color.White;
            t.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            // Estructura de Anillo/Torta para Vendedor
            Series s = new Series("SerieVendedor")
            {
                ChartType = SeriesChartType.Doughnut
            };
            chartReportes.Series.Add(s);
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            // Aquí irá la consulta a la base de datos cuando la conectemos
        }

        // Compatibilidad con eventos del diseñador previo
        private void tituloDesde_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void fechaDesde_ValueChanged(object sender, EventArgs e) { }
        private void fechaHasta_ValueChanged(object sender, EventArgs e) { }
        private void tituloVendedor_Click(object sender, EventArgs e) { }
        private void textBoxBuscarVendedor_TextChanged(object sender, EventArgs e) { }
        private void panelVentasVendedor_Paint(object sender, PaintEventArgs e) { }
        private void panelReportes_Paint(object sender, PaintEventArgs e) { }

        // Métodos de eventos requeridos por el Diseñador para evitar el error CS1061
        private void botonVentasVendedor_Click(object sender, EventArgs e) { }
        private void botonProductoVendido_Click(object sender, EventArgs e) { }
        private void botonVentas_Click(object sender, EventArgs e) { }
        private void botonRecaudacion_Click(object sender, EventArgs e) { }
    }
}