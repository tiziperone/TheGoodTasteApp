using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Linq;
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

            // Cargar automáticamente los datos al abrir si es VENDEDOR
            if (_usuarioActual != null && _usuarioActual.Rol == RolUsuario.Vendedor)
            {
                CargarReporteVendedor();
            }
        }

        // Método auxiliar para buscar controles dentro de Paneles o Contenedores anidados
        private Control BuscarControlEnTodoElFormulario(string nombreControl)
        {
            Control[] controles = Controls.Find(nombreControl, true);
            if (controles.Length > 0)
            {
                return controles[0];
            }
            return null;
        }

        private void MostrarInfoUsuario()
        {
            if (_usuarioActual == null) return;

            string nombreAMostrar = !string.IsNullOrWhiteSpace(_usuarioActual.Nombre)
                ? $"{_usuarioActual.Nombre} {_usuarioActual.Apellido}".Trim()
                : _usuarioActual.Username;

            string texto = $"Vendedor: {nombreAMostrar} (DNI: {_usuarioActual.DNI})";

            Control lbl = BuscarControlEnTodoElFormulario("lblVendedor");
            if (lbl is Label lblCast)
            {
                lblCast.Text = texto;
                lblCast.Visible = true;
                lblCast.BringToFront();
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

            Control lblTituloGrilla = BuscarControlEnTodoElFormulario("label2");
            Control btnRecaudacion = BuscarControlEnTodoElFormulario("botonRecaudacion");
            Control btnProductoVendido = BuscarControlEnTodoElFormulario("botonProductoVendido");
            Control btnVentas = BuscarControlEnTodoElFormulario("botonVentas");
            Control panelVendedorSearch = BuscarControlEnTodoElFormulario("panelVentasVendedor");
            Control btnTopClientes = BuscarControlEnTodoElFormulario("button1");
            Control pnlClientes = BuscarControlEnTodoElFormulario("panelClientes");

            // Inicializamos la barra/desplegable para AMBOS roles
            InicializarSelectorReportes();

            if (_usuarioActual.Rol == RolUsuario.Vendedor)
            {
                this.Text = $"Mis Reportes - Vendedor: {_usuarioActual.Username}";

                // Ocultar botones laterales de Gerente
                if (btnRecaudacion != null) btnRecaudacion.Visible = false;
                if (btnProductoVendido != null) btnProductoVendido.Visible = false;
                if (panelVendedorSearch != null) panelVendedorSearch.Visible = false;
                if (btnTopClientes != null) btnTopClientes.Visible = false;
                if (btnVentas != null) btnVentas.Visible = false;

                if (pnlClientes != null) pnlClientes.Visible = true;
                if (listaClientes != null) listaClientes.Visible = true;

                if (lblTituloGrilla is Label l)
                {
                    l.Text = "Detalle de Mis Ventas";
                }
            }
            else
            {
                this.Text = "Reportes Estratégicos - Gerencia";

                if (btnRecaudacion != null) btnRecaudacion.Visible = true;
                if (btnProductoVendido != null) btnProductoVendido.Visible = true;
                if (panelVendedorSearch != null) panelVendedorSearch.Visible = true;
                if (btnTopClientes != null) btnTopClientes.Visible = true;
                if (btnVentas != null) btnVentas.Visible = true;

                if (lblTituloGrilla is Label l)
                {
                    l.Text = "Listado de Datos";
                }
            }
        }

        private void CargarReporteVendedor()
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            DataTable dt = negocio.GenerarMisVentas(_usuarioActual.DNI, desde, hasta);
            RenderizarGrafico(dt, "Mis Ventas por Día", SeriesChartType.Column, false);
        }


        private void InicializarSelectorReportes()
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

                if (_usuarioActual != null && _usuarioActual.Rol == RolUsuario.Vendedor)
                {
                    // Opciones disponibles para el VENDEDOR
                    _cmbReportesGerencia.Items.AddRange(new string[]
                    {
                "Mis Ventas por Día",
                "Cierre de Caja (Formas de Pago)"
                    });
                }
                else
                {
                    // Opciones disponibles para GERENCIA
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
                }

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

            if (_usuarioActual != null && _usuarioActual.Rol == RolUsuario.Vendedor)
            {
                switch (_cmbReportesGerencia.SelectedIndex)
                {
                    case 0: // Mis Ventas por Día
                        dt = negocio.GenerarMisVentas(_usuarioActual.DNI, desde, hasta);
                        RenderizarGrafico(dt, "Mis Ventas por Día", SeriesChartType.Column);
                        break;
                    case 1: // Cierre de Caja / Formas de Pago del vendedor
                        dt = negocio.GenerarVentasPorMedioPago(desde, hasta);
                        RenderizarGrafico(dt, "Cierre de Caja - Formas de Pago", SeriesChartType.Doughnut);
                        break;
                }
            }
            else
            {
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
                        Control txtVendedor = BuscarControlEnTodoElFormulario("textBoxBuscarVendedor");
                        string filtroVendedor = txtVendedor != null ? txtVendedor.Text.Trim() : "";
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

        // --- MANEJADORES DE EVENTOS ---

        private void botonVentas_Click(object sender, EventArgs e)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            if (_usuarioActual != null && _usuarioActual.Rol == RolUsuario.Vendedor)
            {
                CargarReporteVendedor();
            }
            else
            {
                DataTable dt = negocio.GenerarVentasGenerales(desde, hasta);
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

            Control txt = BuscarControlEnTodoElFormulario("textBoxBuscarVendedor");
            string vendedorFiltro = txt != null ? txt.Text.Trim() : "";

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