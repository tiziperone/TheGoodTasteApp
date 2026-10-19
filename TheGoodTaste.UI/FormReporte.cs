using System;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
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

        // Panel de Búsqueda de Facturas
        private Panel _pnlBuscarFactura;
        private ComboBox _cmbFacturas;
        private Button _btnBuscarFactura;

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
            AgrandarGraficoMaximo();

            if (_usuarioActual != null && _usuarioActual.Rol == RolUsuario.Vendedor)
            {
                CargarReporteVendedor();
            }
        }

        private void AgrandarGraficoMaximo()
        {
            if (chart1 != null && chart1.ChartAreas.Count > 0)
            {
                var area = chart1.ChartAreas[0];
                area.Position.Auto = false;
                area.Position.X = 1;
                area.Position.Y = 4;
                area.Position.Width = 98;
                area.Position.Height = 92;

                area.InnerPlotPosition.Auto = true;
            }
        }

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
                    Location = new Point(25, 12),
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

            if (pnlClientes != null) pnlClientes.Visible = true;
            if (listaClientes != null) listaClientes.Visible = true;

            InicializarSelectorReportes();
            InicializarListadoFacturasBuscador();

            if (_usuarioActual.Rol == RolUsuario.Vendedor)
            {
                this.Text = $"Mis Reportes - Vendedor: {_usuarioActual.Username}";

                if (btnRecaudacion != null) btnRecaudacion.Visible = false;
                if (btnProductoVendido != null) btnProductoVendido.Visible = false;
                if (panelVendedorSearch != null) panelVendedorSearch.Visible = false;
                if (btnTopClientes != null) btnTopClientes.Visible = false;
                if (btnVentas != null) btnVentas.Visible = false;

                // Ocultar el buscador de facturas al rol VENDEDOR
                if (_pnlBuscarFactura != null) _pnlBuscarFactura.Visible = false;

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

                // Mostrar el buscador de facturas únicamente a GERENCIA
                if (_pnlBuscarFactura != null) _pnlBuscarFactura.Visible = true;

                if (lblTituloGrilla is Label l)
                {
                    l.Text = "Listado de Datos / Clientes";
                }
            }
        }

        private void InicializarListadoFacturasBuscador()
        {
            if (_pnlBuscarFactura == null)
            {
                _pnlBuscarFactura = new Panel
                {
                    Name = "pnlBuscarFactura",
                    Location = new Point(540, 12),
                    Size = new Size(380, 35),
                    BackColor = Color.Transparent
                };

                Label lblFactura = new Label
                {
                    Text = "Buscar Factura:",
                    Location = new Point(0, 6),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold)
                };

                _cmbFacturas = new ComboBox
                {
                    Name = "cmbFacturas",
                    Location = new Point(100, 3),
                    Width = 170,
                    DropDownStyle = ComboBoxStyle.DropDown,
                    Font = new Font("Segoe UI", 9f)
                };

                _cmbFacturas.TextUpdate += (s, e) =>
                {
                    string busqueda = _cmbFacturas.Text;
                    if (busqueda.Length >= 1)
                    {
                        CargarListaFacturasDesplegable(busqueda);
                        _cmbFacturas.Text = busqueda;
                        _cmbFacturas.SelectionStart = busqueda.Length;
                        _cmbFacturas.DroppedDown = true;
                    }
                };

                _btnBuscarFactura = new Button
                {
                    Name = "btnBuscarFactura",
                    Text = "🔍 Consultar",
                    Location = new Point(275, 2),
                    Width = 100,
                    Height = 26,
                    BackColor = Color.FromArgb(180, 130, 40),
                    ForeColor = Color.Black,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                _btnBuscarFactura.FlatAppearance.BorderSize = 0;
                _btnBuscarFactura.Click += BtnBuscarFactura_Click;

                _pnlBuscarFactura.Controls.Add(lblFactura);
                _pnlBuscarFactura.Controls.Add(_cmbFacturas);
                _pnlBuscarFactura.Controls.Add(_btnBuscarFactura);

                this.Controls.Add(_pnlBuscarFactura);
                _pnlBuscarFactura.BringToFront();
            }

            CargarListaFacturasDesplegable();
        }

        private void CargarListaFacturasDesplegable(string filtro = "")
        {
            try
            {
                ReporteNegocio negocio = new ReporteNegocio();
                DataTable dtFacturas = negocio.ObtenerHistorialFacturas(filtro);

                if (dtFacturas != null && dtFacturas.Rows.Count > 0)
                {
                    _cmbFacturas.DataSource = dtFacturas;
                    _cmbFacturas.DisplayMember = "Descripcion";
                    _cmbFacturas.ValueMember = "NroFactura";
                    _cmbFacturas.SelectedIndex = -1;
                    _cmbFacturas.Text = string.Empty;
                }
            }
            catch
            {
                // Protección
            }
        }

        private void BtnBuscarFactura_Click(object sender, EventArgs e)
        {
            int idFactura = 0;

            try
            {
                if (_cmbFacturas.SelectedIndex != -1 && _cmbFacturas.SelectedValue != null)
                {
                    int.TryParse(_cmbFacturas.SelectedValue.ToString(), out idFactura);
                }

                if (idFactura <= 0 && !string.IsNullOrWhiteSpace(_cmbFacturas.Text))
                {
                    Match m = Regex.Match(_cmbFacturas.Text, @"\d+");
                    if (m.Success)
                    {
                        int.TryParse(m.Value, out idFactura);
                    }
                }
            }
            catch
            {
                idFactura = 0;
            }

            if (idFactura <= 0)
            {
                MessageBox.Show("Seleccione o ingrese un número de factura válido.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ReporteNegocio negocio = new ReporteNegocio();
            DataTable dtFactura = negocio.BuscarFacturaPorId(idFactura);

            if (dtFactura != null && dtFactura.Rows.Count > 0)
            {
                listaClientes.DataSource = dtFactura;
                listaClientes.Visible = true;

                if (listaClientes.Columns["Precio Unitario"] != null)
                    listaClientes.Columns["Precio Unitario"].DefaultCellStyle.Format = "$#,##0.00";
                if (listaClientes.Columns["Subtotal"] != null)
                    listaClientes.Columns["Subtotal"].DefaultCellStyle.Format = "$#,##0.00";
                if (listaClientes.Columns["Total Factura"] != null)
                    listaClientes.Columns["Total Factura"].DefaultCellStyle.Format = "$#,##0.00";

                chart1.Series[0].Points.Clear();
                chart1.Series[0].Points.DataBindXY(dtFactura.DefaultView, "Producto", dtFactura.DefaultView, "Cantidad");
                chart1.Series[0].ChartType = SeriesChartType.Column;
                chart1.Series[0].IsValueShownAsLabel = true;
                chart1.Series[0].LabelFormat = "#,##0 \"un.\"";

                chart1.Titles.Clear();
                chart1.Titles.Add($"Detalle de Productos - Factura N° {idFactura}").Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);

                Control lblTitulo = BuscarControlEnTodoElFormulario("label2");
                if (lblTitulo is Label l)
                {
                    l.Text = $"Historial de Factura N° {idFactura}";
                }
            }
            else
            {
                MessageBox.Show($"No se encontró la factura N° {idFactura} en la base de datos.", "Sin Resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    Location = new Point(25, 52),
                    Width = 320,
                    Font = new Font("Segoe UI", 9.5f)
                };

                if (_usuarioActual != null && _usuarioActual.Rol == RolUsuario.Vendedor)
                {
                    _cmbReportesGerencia.Items.AddRange(new string[]
                    {
                        "Mis Ventas por Día",
                        "Cierre de Caja (Formas de Pago)"
                    });
                }
                else
                {
                    _cmbReportesGerencia.Items.AddRange(new string[]
                    {
                        "Ventas Globales",
                        "Recaudación Global",
                        "Ventas por Medio de Pago",
                        "Ticket Promedio Diario",
                        "Top 5 Productos Más Vendidos",
                        "Stock Crítico / Faltantes",
                        "Día de Mayor Venta por Producto",
                        "Top 5 Clientes"
                    });
                }

                _cmbReportesGerencia.SelectedIndex = 0;

                _btnGenerarReporte = new Button
                {
                    Name = "btnGenerarReporte",
                    Text = "Generar Reporte",
                    Location = new Point(355, 50),
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
                    case 0:
                        dt = negocio.GenerarMisVentas(_usuarioActual.DNI, desde, hasta);
                        RenderizarGrafico(dt, "Mis Ventas por Día", SeriesChartType.Column);
                        break;
                    case 1:
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
                        RenderizarVentasGenerales(dt, "Ventas Globales");
                        break;
                    case 1:
                        dt = negocio.GenerarRecaudacionGlobal(desde, hasta);
                        RenderizarGrafico(dt, "Recaudación Global", SeriesChartType.Line);
                        break;
                    case 2:
                        dt = negocio.GenerarVentasPorMedioPago(desde, hasta);
                        RenderizarGrafico(dt, "Ventas por Medio de Pago", SeriesChartType.Doughnut);
                        break;
                    case 3:
                        dt = negocio.GenerarTicketPromedio(desde, hasta);
                        RenderizarGrafico(dt, "Ticket Promedio Diario ($)", SeriesChartType.Line);
                        break;
                    case 4:
                        dt = negocio.GenerarTopProductos(desde, hasta);
                        RenderizarGrafico(dt, "Top 5 Productos Más Vendidos", SeriesChartType.Bar);
                        break;
                    case 5:
                        dt = negocio.GenerarStockCritico();
                        RenderizarGrafico(dt, "Productos con Stock Crítico", SeriesChartType.Column);
                        break;
                    case 6:
                        dt = negocio.GenerarDiaMasVendidoPorProducto(desde, hasta);
                        RenderizarGraficoPersonalizado(dt, "Día de Mayor Venta por Producto", SeriesChartType.Column);
                        break;
                    case 7:
                        dt = negocio.GenerarTopClientes(desde, hasta);
                        RenderizarTopClientes(dt, "Top 5 Clientes");
                        break;
                }
            }
        }

        private void RenderizarVentasGenerales(DataTable dt, string titulo)
        {
            listaClientes.DataSource = dt;
            listaClientes.Visible = true;

            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count >= 3)
            {
                if (listaClientes.Columns["CantidadVentas"] != null)
                    listaClientes.Columns["CantidadVentas"].DefaultCellStyle.Format = "#,##0";
                if (listaClientes.Columns["Total"] != null)
                    listaClientes.Columns["Total"].DefaultCellStyle.Format = "$#,##0.00";

                string colX = dt.Columns[0].ColumnName;
                string colY = dt.Columns[1].ColumnName;

                chart1.Series[0].Points.Clear();
                chart1.Series[0].Points.DataBindXY(dt.DefaultView, colX, dt.DefaultView, colY);
                chart1.Series[0].ChartType = SeriesChartType.Line;
                chart1.Series[0].IsValueShownAsLabel = true;
                chart1.Series[0].LabelFormat = "#,##0 \"ventas\"";

                chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -35;
                chart1.ChartAreas[0].AxisX.Interval = 1;

                chart1.Titles.Clear();
                chart1.Titles.Add(titulo + " (Cantidad de Operaciones)").Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            }
            else
            {
                chart1.Series[0].Points.Clear();
                chart1.Titles.Clear();
                MessageBox.Show("No se encontraron registros en el período seleccionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RenderizarGrafico(DataTable dt, string titulo, SeriesChartType tipoGrafico, bool mostrarAlertaVacio = true)
        {
            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count >= 2)
            {
                DataTable dtClon = new DataTable();
                string nombrePrimeraColumna = dt.Columns[0].ColumnName;

                dtClon.Columns.Add(nombrePrimeraColumna, typeof(string));
                dtClon.Columns.Add(dt.Columns[1].ColumnName, dt.Columns[1].DataType);

                System.Globalization.CultureInfo culturaEs = new System.Globalization.CultureInfo("es-AR");

                foreach (DataRow row in dt.Rows)
                {
                    if (row[0] != DBNull.Value)
                    {
                        string valorStr = row[0].ToString();
                        string etiquetaFinal = valorStr;

                        DateTime fechaDetectada = default;
                        bool esFecha = false;

                        if (double.TryParse(valorStr, out double numSerial) && numSerial > 30000 && numSerial < 60000)
                        {
                            try
                            {
                                fechaDetectada = DateTime.FromOADate(numSerial);
                                esFecha = true;
                            }
                            catch { esFecha = false; }
                        }
                        else if (DateTime.TryParse(valorStr, out fechaDetectada))
                        {
                            esFecha = true;
                        }

                        if (esFecha)
                        {
                            string diaNombre = culturaEs.TextInfo.ToTitleCase(fechaDetectada.ToString("ddd", culturaEs));
                            etiquetaFinal = $"{diaNombre} {fechaDetectada:dd/MM}";
                        }

                        dtClon.Rows.Add(etiquetaFinal, row[1]);
                    }
                }

                listaClientes.DataSource = dtClon;
                listaClientes.Visible = true;

                string colX = dtClon.Columns[0].ColumnName;
                string colY = dtClon.Columns[1].ColumnName;

                chart1.Series[0].Points.Clear();
                chart1.Series[0].Points.DataBindXY(dtClon.DefaultView, colX, dtClon.DefaultView, colY);
                chart1.Series[0].ChartType = tipoGrafico;
                chart1.Series[0].IsValueShownAsLabel = true;

                if (titulo.Contains("Recaudación") || titulo.Contains("Ticket") || titulo.Contains("Total") || titulo.Contains("$"))
                {
                    chart1.Series[0].LabelFormat = "$#,##0.00";

                    if (listaClientes.Columns.Count > 1)
                    {
                        listaClientes.Columns[1].DefaultCellStyle.Format = "$#,##0.00";
                    }
                }
                else
                {
                    chart1.Series[0].LabelFormat = "#,##0";
                }

                chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -35;
                chart1.ChartAreas[0].AxisX.Interval = 1;
                chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
                chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;

                chart1.Titles.Clear();
                chart1.Titles.Add(titulo).Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            }
            else
            {
                listaClientes.DataSource = dt;
                listaClientes.Visible = true;
                chart1.Series[0].Points.Clear();
                chart1.Titles.Clear();

                if (mostrarAlertaVacio)
                {
                    MessageBox.Show("No se encontraron registros en el período seleccionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void RenderizarTopClientes(DataTable dt, string titulo)
        {
            listaClientes.DataSource = dt;
            listaClientes.Visible = true;

            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count >= 4)
            {
                if (listaClientes.Columns["Total Gastado"] != null)
                {
                    listaClientes.Columns["Total Gastado"].DefaultCellStyle.Format = "$#,##0.00";
                }

                string colX = dt.Columns[0].ColumnName;
                string colY = dt.Columns[3].ColumnName;

                chart1.Series[0].Points.Clear();
                chart1.Series[0].Points.DataBindXY(dt.DefaultView, colX, dt.DefaultView, colY);
                chart1.Series[0].ChartType = SeriesChartType.Pie;
                chart1.Series[0].IsValueShownAsLabel = true;
                chart1.Series[0].LabelFormat = "$#,##0.00";

                chart1.Titles.Clear();
                chart1.Titles.Add(titulo).Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            }
            else
            {
                chart1.Series[0].Points.Clear();
                chart1.Titles.Clear();
                MessageBox.Show("No se encontraron registros de clientes en el período seleccionado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RenderizarGraficoPersonalizado(DataTable dt, string titulo, SeriesChartType tipoGrafico, bool mostrarAlertaVacio = true)
        {
            listaClientes.DataSource = dt;
            listaClientes.Visible = true;

            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count >= 3)
            {
                string colX = dt.Columns[0].ColumnName;
                string colY = dt.Columns[2].ColumnName;

                chart1.Series[0].Points.Clear();
                chart1.Series[0].Points.DataBindXY(dt.DefaultView, colX, dt.DefaultView, colY);
                chart1.Series[0].ChartType = tipoGrafico;
                chart1.Series[0].IsValueShownAsLabel = true;
                chart1.Series[0].LabelFormat = "#,##0 \"un.\"";

                chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -35;
                chart1.ChartAreas[0].AxisX.Interval = 1;

                chart1.Titles.Clear();
                chart1.Titles.Add(titulo).Font = new Font("Segoe UI", 11f, FontStyle.Bold);
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
                RenderizarVentasGenerales(dt, "Ventas Globales");
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
            RenderizarTopClientes(dt, "Top 5 Clientes con más compras");
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