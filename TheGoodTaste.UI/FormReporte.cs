using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TheGoodTaste.Negocio;
using The_Good_Taste.Entidades; // Asegúrate de que aquí esté UsuarioSistema y RolUsuario

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

        // El menú principal DEBE llamar a este constructor pasándole el usuario logueado
        public FormReporte(UsuarioSistema usuario) : this()
        {
            _usuarioActual = usuario;
        }

        private void FormReporte_Load(object sender, EventArgs e)
        {
            // TemaVisual.AplicarEstilo(this); // Descomentar si usas tu clase de estilos

            ConstruirEstructuraVistas();
            CargarOpcionesPorRol();
            ConfigurarVistaInicialSegunRol();
        }

        private void ConstruirEstructuraVistas()
        {
            this.Controls.Clear();
            this.Padding = new Padding(15);
            this.BackColor = Color.FromArgb(35, 25, 20);

            // PANEL SUPERIOR
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

            // PANEL CENTRAL
            pnlContenido = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };

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

            // Inicializar la serie base
            Series s = new Series("Serie1");
            chartReportes.Series.Add(s);

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

            this.Controls.Add(pnlContenido);
            this.Controls.Add(pnlSuperior);
        }

        private void CargarOpcionesPorRol()
        {
            cmbTipoReporte.Items.Clear();

            if (_usuarioActual != null && _usuarioActual.Rol == RolUsuario.Vendedor)
            {
                cmbTipoReporte.Items.Add("Mis Ventas por Período");
                cmbTipoReporte.Items.Add("Mis Productos Más Vendidos");
                cmbTipoReporte.Items.Add("Mis Ventas por Categoría");
            }
            else
            {
                cmbTipoReporte.Items.Add("Recaudación Global");
                cmbTipoReporte.Items.Add("Ventas por Vendedor");
                cmbTipoReporte.Items.Add("Top Productos Más Vendidos");
            }

            if (cmbTipoReporte.Items.Count > 0)
                cmbTipoReporte.SelectedIndex = 0;
        }

        private void ConfigurarVistaInicialSegunRol()
        {
            if (_usuarioActual == null) return;

            if (_usuarioActual.Rol == RolUsuario.Vendedor)
                this.Text = $"Reportes - Vendedor: {_usuarioActual.NombreUsuario}";
            else
                this.Text = "Reportes Estratégicos - Gerencia";
        }

        private void BtnFiltrar_Click(object sender, EventArgs e)
        {
            if (cmbTipoReporte.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un tipo de reporte.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string reporteSeleccionado = cmbTipoReporte.SelectedItem.ToString();
            ReporteNegocio negocio = new ReporteNegocio();
            DataTable dtResultados;

            try
            {
                // 1. Obtener datos (Corrección aplicada aquí)
                if (_usuarioActual.Rol == RolUsuario.Vendedor)
                    dtResultados = negocio.GenerarReporteVendedor(reporteSeleccionado, _usuarioActual.IdUsuario);
                else
                    dtResultados = negocio.GenerarReporteGerente(reporteSeleccionado);

                // 2. Llenar la Grilla
                dgvReportes.DataSource = dtResultados;

                // 3. Renderizar Gráfico
                if (dtResultados.Rows.Count > 0)
                {
                    string columnaX = dtResultados.Columns[0].ColumnName;
                    string columnaY = dtResultados.Columns[1].ColumnName;

                    chartReportes.Series[0].Points.DataBindXY(
                        dtResultados.DefaultView, columnaX,
                        dtResultados.DefaultView, columnaY
                    );

                    // 4. Ajustar tipo de gráfico según lo que quede mejor a la vista
                    if (reporteSeleccionado.Contains("Categoría") || reporteSeleccionado == "Ventas por Vendedor")
                        chartReportes.Series[0].ChartType = SeriesChartType.Doughnut;
                    else if (reporteSeleccionado.Contains("Período") || reporteSeleccionado == "Recaudación Global")
                        chartReportes.Series[0].ChartType = SeriesChartType.Line;
                    else
                        chartReportes.Series[0].ChartType = SeriesChartType.Column;

                    chartReportes.Series[0].Color = Color.FromArgb(180, 130, 40); // Dorado del tema

                    chartReportes.Titles.Clear();
                    Title t = chartReportes.Titles.Add(reporteSeleccionado);
                    t.ForeColor = Color.White;
                    t.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                }
                else
                {
                    chartReportes.Series[0].Points.Clear();
                    chartReportes.Titles.Clear();
                    MessageBox.Show("No se encontraron datos para el reporte seleccionado.", "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el reporte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Eventos requeridos por el diseñador anterior para evitar error CS1061
        private void FormReporteGerente_Load(object sender, EventArgs e) => FormReporte_Load(sender, e);
        private void tituloDesde_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void fechaDesde_ValueChanged(object sender, EventArgs e) { }
        private void fechaHasta_ValueChanged(object sender, EventArgs e) { }
        private void tituloVendedor_Click(object sender, EventArgs e) { }
        private void textBoxBuscarVendedor_TextChanged(object sender, EventArgs e) { }
        private void panelVentasVendedor_Paint(object sender, PaintEventArgs e) { }
        private void panelReportes_Paint(object sender, PaintEventArgs e) { }
        private void botonVentasVendedor_Click(object sender, EventArgs e) { }
        private void botonProductoVendido_Click(object sender, EventArgs e) { }
        private void botonVentas_Click(object sender, EventArgs e) { }
        private void botonRecaudacion_Click(object sender, EventArgs e) { }
    }
}