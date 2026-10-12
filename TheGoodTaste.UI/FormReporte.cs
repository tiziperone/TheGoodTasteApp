using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using The_Good_Taste.Entidades;
using TheGoodTaste.Negocio;

namespace TheGoodTaste.UI
{
    public partial class FormReportes : Form
    {
        private readonly UsuarioSistema _usuarioActual;
        private Label _lblInfoUsuario;
        private ComboBox _cmbReportesGerencia;
        private Button _btnGenerarReporte;

        public FormReportes()
        {
            InitializeComponent();
        }

        public FormReportes(UsuarioSistema usuario) : this()
        {
            _usuarioActual = usuario;
        }

        private void FormReportes_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);

            if (chart1.Series.Count > 0)
            {
                chart1.Series[0].Points.Clear();
            }
            chart1.Titles.Clear();
            listaClientes.DataSource = null;

            fechaDesde.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            fechaHasta.Value = DateTime.Now;

            ConfigurarVistaInicialSegunRol();
            MostrarInfoUsuario();
        }

        private void MostrarInfoUsuario()
        {
            if (_usuarioActual == null) return;

            string nombreAMostrar = !string.IsNullOrWhiteSpace(_usuarioActual.Nombre)
                ? $"{_usuarioActual.Nombre} {_usuarioActual.Apellido}".Trim()
                : _usuarioActual.Username;

            string texto = $"{_usuarioActual.Rol}: {nombreAMostrar} (DNI: {_usuarioActual.DNI})";

            Control[] controles = Controls.Find("lblVendedor", true);
            if (controles.Length > 0 && controles[0] is Label lbl)
            {
                lbl.Text = texto;
                return;
            }

            if (_lblInfoUsuario == null)
            {
                _lblInfoUsuario = new Label
                {
                    Name = "lblInfoUsuarioDinamico",
                    AutoSize = true,
                    Location = new Point(25, 20),
                    Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(230, 160, 50),
                    BackColor = Color.Transparent
                };
                this.Controls.Add(_lblInfoUsuario);
                _lblInfoUsuario.BringToFront();
            }

            _lblInfoUsuario.Text = texto;
        }

        private void ConfigurarVistaInicialSegunRol()
        {
            if (_usuarioActual == null) return;

            Label lblTituloGrilla = null;
            Control[] controlesLabel2 = Controls.Find("label2", true);
            if (controlesLabel2.Length > 0 && controlesLabel2[0] is Label l)
            {
                lblTituloGrilla = l;
            }

            if (_usuarioActual.Rol == RolUsuario.Vendedor)
            {
                this.Text = $"Mis Reportes - Vendedor: {_usuarioActual.Username}";
                panelVentasVendedor.Visible = false;
                button1.Visible = false;

                if (_cmbReportesGerencia != null) _cmbReportesGerencia.Visible = false;
                if (_btnGenerarReporte != null) _btnGenerarReporte.Visible = false;

                panelClientes.Visible = true;
                listaClientes.Visible = true;

                if (lblTituloGrilla != null)
                {
                    lblTituloGrilla.Text = "Detalle de Mis Ventas";
                }

            }
            else
            {
                this.Text = "Reportes Estratégicos - Gerencia";
                if (lblTituloGrilla != null)
                {
                    lblTituloGrilla.Text = "Listado de Datos";
                }

                InicializarSelectorReportesGerencia();
            }
        }

        private void InicializarSelectorReportesGerencia()
        {
            if (_cmbReportesGerencia == null)
            {
                _cmbReportesGerencia = new ComboBox
                {
                    Name = "cmbReportesGerencia",
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(25, 60),
                    Width = 320,
                    Font = new Font("Segoe UI", 9.5f)
                };

                _cmbReportesGerencia.Items.AddRange(new string[]
                {
                    "Ventas Globales",
                    "Recaudación Global",
                    "Ventas por Vendedor",
                    "Ventas por Medio de Pago",
                    "Ticket Promedio Diario",
                    "Top 5 Productos Más Vendidos",
                    "Stock Crítico / Faltantes",
                    "Día de Mayor Venta por Producto",
                    "Top 5 Clientes",
                    "Clientes Fieles (Múltiples Compras)"
                });

                _cmbReportesGerencia.SelectedIndex = 0;

                _btnGenerarReporte = new Button
                {
                    Name = "btnGenerarReporte",
                    Text = "Generar Reporte",
                    Location = new Point(355, 58),
                    Height = 28,
                    Width = 120,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold)
                };

                _btnGenerarReporte.Click += BtnGenerarReporte_Click;

                this.Controls.Add(_cmbReportesGerencia);
                this.Controls.Add(_btnGenerarReporte);
                _cmbReportesGerencia.BringToFront();
                _btnGenerarReporte.BringToFront();
            }
        }

        private void BtnGenerarReporte_Click(object sender, EventArgs e)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);
            DataTable dt;

            switch (_cmbReportesGerencia.SelectedIndex)
            {
                case 0:
                    dt = negocio.GenerarVentasGenerales(desde, hasta);
                    RenderizarGrafico(dt, "Ventas Globales", SeriesChartType.Line);
                    break;
                case 1:
                    dt = negocio.GenerarRecaudacionGlobal(desde, hasta);
                    RenderizarGrafico(dt, "Recaudación Global", SeriesChartType.Line);
                    break;
                case 2:
                    string filtroVendedor = textBoxBuscarVendedor != null ? textBoxBuscarVendedor.Text.Trim() : "";
                    dt = negocio.GenerarVentasPorVendedor(desde, hasta, filtroVendedor);
                    RenderizarGrafico(dt, "Ventas por Vendedor", SeriesChartType.Doughnut);
                    break;
                case 3:
                    dt = negocio.GenerarVentasPorMedioPago(desde, hasta);
                    RenderizarGrafico(dt, "Ventas por Medio de Pago", SeriesChartType.Doughnut);
                    break;
                case 4:
                    dt = negocio.GenerarTicketPromedio(desde, hasta);
                    RenderizarGrafico(dt, "Ticket Promedio Diario ($)", SeriesChartType.Line);
                    break;
                case 5:
                    dt = negocio.GenerarTopProductos(desde, hasta);
                    RenderizarGrafico(dt, "Top 5 Productos Más Vendidos", SeriesChartType.Bar);
                    break;
                case 6:
                    dt = negocio.GenerarStockCritico();
                    RenderizarGrafico(dt, "Productos con Stock Crítico", SeriesChartType.Column);
                    break;
                case 7:
                    dt = negocio.GenerarDiaMasVendidoPorProducto(desde, hasta);
                    RenderizarGrafico(dt, "Día de Mayor Venta por Producto", SeriesChartType.Column);
                    break;
                case 8:
                    dt = negocio.GenerarTopClientes(desde, hasta);
                    RenderizarGrafico(dt, "Top 5 Clientes", SeriesChartType.Pie);
                    break;
                case 9:
                    dt = negocio.GenerarClientesFieles(desde, hasta);
                    RenderizarGrafico(dt, "Clientes Fieles (Múltiples Compras)", SeriesChartType.Column);
                    break;
            }
        }

        private void RenderizarGrafico(DataTable dt, string titulo, SeriesChartType tipoGrafico, bool mostrarAlertaVacio = true)
        {
            listaClientes.DataSource = dt;

            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count >= 2)
            {
                string colX = dt.Columns[0].ColumnName;
                string colY = dt.Columns[1].ColumnName;

                chart1.Series[0].Points.DataBindXY(dt.DefaultView, colX, dt.DefaultView, colY);
                chart1.Series[0].ChartType = tipoGrafico;
                chart1.Series[0].IsValueShownAsLabel = true;

                chart1.Titles.Clear();
                chart1.Titles.Add(titulo).Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            }
            else
            {
                chart1.Series[0].Points.Clear();
                chart1.Titles.Clear();

                if (mostrarAlertaVacio)
                {
                    MessageBox.Show("No se encontraron registros en el período seleccionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // --- BOTONES ---

        private void botonVentas_Click(object sender, EventArgs e)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            DataTable dt;
            if (_usuarioActual != null && _usuarioActual.Rol == RolUsuario.Vendedor)
            {
                dt = negocio.GenerarMisVentas(_usuarioActual.DNI, desde, hasta);
                RenderizarGrafico(dt, "Mis Ventas por Día", SeriesChartType.Column);
            }
            else
            {
                dt = negocio.GenerarVentasGenerales(desde, hasta);
                RenderizarGrafico(dt, "Ventas Globales", SeriesChartType.Line);
            }
        }

        private void botonRecaudacion_Click(object sender, EventArgs e)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            DataTable dt = negocio.GenerarRecaudacionGlobal(desde, hasta);
            RenderizarGrafico(dt, "Recaudación Global", SeriesChartType.Line);
        }

        private void botonProductoVendido_Click(object sender, EventArgs e)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            DataTable dt = negocio.GenerarTopProductos(desde, hasta);
            RenderizarGrafico(dt, "Top Productos Vendidos", SeriesChartType.Bar);
        }

        private void botonVentasVendedor_Click(object sender, EventArgs e)
        {
            EjecutarBusquedaVendedor(true);
        }

        private void textBoxBuscarVendedor_TextChanged(object sender, EventArgs e)
        {
            EjecutarBusquedaVendedor(false);
        }

        private void EjecutarBusquedaVendedor(bool mostrarAlerta)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);
            string vendedorFiltro = textBoxBuscarVendedor.Text.Trim();

            DataTable dt = negocio.GenerarVentasPorVendedor(desde, hasta, vendedorFiltro);
            RenderizarGrafico(dt, "Ventas por Vendedor", SeriesChartType.Doughnut, mostrarAlerta);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            DataTable dt = negocio.GenerarTopClientes(desde, hasta);
            RenderizarGrafico(dt, "Top 5 Clientes con más compras", SeriesChartType.Pie);
        }

        // Eventos del diseñador
        private void FormReporte_Load(object sender, EventArgs e) => FormReportes_Load(sender, e);
        private void tituloDesde_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void fechaDesde_ValueChanged(object sender, EventArgs e) { }
        private void fechaHasta_ValueChanged(object sender, EventArgs e) { }
        private void tituloVendedor_Click(object sender, EventArgs e) { }
        private void panelVentasVendedor_Paint(object sender, PaintEventArgs e) { }
        private void panelReportes_Paint(object sender, PaintEventArgs e) { }
        private void listaClientes_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void chart1_Click(object sender, EventArgs e) { }
    }
}