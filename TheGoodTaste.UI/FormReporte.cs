using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TheGoodTaste.Negocio;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormReportes : Form
    {
        private readonly UsuarioSistema _usuarioActual;
        private Label _lblInfoUsuario; // Label informativo del usuario en pantalla

        // Botones dinámicos nuevos
        private Button _btnCierreCajaVendedor;
        private Button _btnVentasTipoPagoGlobal;
        private Button _btnVentasDiaSemana;

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

            CrearBotonesDinamicos();
            ConfigurarVistaInicialSegunRol();
            MostrarInfoUsuario();
        }

        private void CrearBotonesDinamicos()
        {
            // Buscar si tenés un panel contenedor para los botones (por ejemplo panelReportes)
            Control contenedor = this;
            Control[] paneles = Controls.Find("panelReportes", true);
            if (paneles.Length > 0)
            {
                contenedor = paneles[0]; // Se agregan dentro del panel lateral
            }

            Button CrearBotonEstilo(string nombre, string texto)
            {
                Button btn = new Button
                {
                    Name = nombre,
                    Text = texto,
                    Size = new Size(130, 45),
                    BackColor = Color.FromArgb(70, 50, 40),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;

                contenedor.Controls.Add(btn);
                btn.BringToFront(); // IMPORTANTE: Lo trae al frente para que no lo tape ningún Panel
                return btn;
            }

            // 1. Botón VENDEDOR: Cierre de Caja
            _btnCierreCajaVendedor = CrearBotonEstilo("btnCierreCajaVendedor", "Cierre de Caja\n(Formas Pago)");
            // Lo posicionamos justo debajo del botón "Mis Ventas"
            _btnCierreCajaVendedor.Location = new Point(botonVentas.Location.X, botonVentas.Location.Y + botonVentas.Height + 10);
            _btnCierreCajaVendedor.Click += btnCierreCajaVendedor_Click;

            // 2. Botón GERENTE: Formas de Pago Global
            _btnVentasTipoPagoGlobal = CrearBotonEstilo("btnVentasTipoPagoGlobal", "Formas de Pago");
            // Lo posicionamos debajo de Recaudación
            _btnVentasTipoPagoGlobal.Location = new Point(botonRecaudacion.Location.X, botonRecaudacion.Location.Y + botonRecaudacion.Height + 10);
            _btnVentasTipoPagoGlobal.Click += btnVentasTipoPagoGlobal_Click;

            // 3. Botón GERENTE: Día de la Semana (Pico)
            _btnVentasDiaSemana = CrearBotonEstilo("btnVentasDiaSemana", "Ventas por Día\n(Semana)");
            // Lo posicionamos debajo de Producto Más Vendido
            _btnVentasDiaSemana.Location = new Point(botonProductoVendido.Location.X, botonProductoVendido.Location.Y + botonProductoVendido.Height + 10);
            _btnVentasDiaSemana.Click += btnVentasDiaSemana_Click;
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

                // Ocultar botones exclusivos de Gerente
                botonRecaudacion.Visible = false;
                botonProductoVendido.Visible = false;
                panelVentasVendedor.Visible = false;
                button1.Visible = false; // Top clientes
                _btnVentasTipoPagoGlobal.Visible = false;
                _btnVentasDiaSemana.Visible = false;

                // Mostrar controles para el Vendedor
                panelClientes.Visible = true;
                listaClientes.Visible = true;
                _btnCierreCajaVendedor.Visible = true;

                if (lblTituloGrilla != null)
                {
                    lblTituloGrilla.Text = "Detalle de Mis Ventas";
                }

                botonVentas.Text = "Mis Ventas";
            }
            else
            {
                this.Text = "Reportes Estratégicos - Gerencia";

                // Ocultar botones de Vendedor e ingresar los de Gerente
                _btnCierreCajaVendedor.Visible = false;
                _btnVentasTipoPagoGlobal.Visible = true;
                _btnVentasDiaSemana.Visible = true;

                if (lblTituloGrilla != null)
                {
                    lblTituloGrilla.Text = "Listado de Clientes";
                }
            }
        }

        private void RenderizarGrafico(DataTable dt, string titulo, SeriesChartType tipoGrafico, bool mostrarAlertaVacio = true)
        {
            listaClientes.DataSource = dt;

            if (dt != null && dt.Rows.Count > 0)
            {
                string colX = dt.Columns[0].ColumnName;
                string colY = dt.Columns[1].ColumnName;

                chart1.Series[0].Points.Clear();
                chart1.Titles.Clear();

                // Arreglo automático de fechas en el eje X para evitar números de serie (ej. 46298)
                if (dt.Columns[0].DataType == typeof(DateTime))
                {
                    chart1.Series[0].XValueType = ChartValueType.Date;
                    chart1.ChartAreas[0].AxisX.LabelStyle.Format = "dd/MM/yyyy";
                    chart1.ChartAreas[0].AxisX.Interval = 1;
                }
                else
                {
                    chart1.Series[0].XValueType = ChartValueType.Auto;
                }

                chart1.Series[0].Points.DataBindXY(dt.DefaultView, colX, dt.DefaultView, colY);
                chart1.Series[0].ChartType = tipoGrafico;
                chart1.Series[0].IsValueShownAsLabel = true;

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

        // --- MANEJADORES DE EVENTOS DE BOTONES EXISTENTES ---

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

        // --- NUEVOS MANEJADORES DE EVENTOS PARA LOS NUEVOS REPORTES ---

        private void btnCierreCajaVendedor_Click(object sender, EventArgs e)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            if (_usuarioActual != null)
            {
                DataTable dt = negocio.GenerarMisVentasPorTipoPago(_usuarioActual.DNI, desde, hasta);
                RenderizarGrafico(dt, "Cierre de Caja - Ventas por Forma de Pago", SeriesChartType.Pie);
            }
        }

        private void btnVentasTipoPagoGlobal_Click(object sender, EventArgs e)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            DataTable dt = negocio.GenerarVentasPorTipoPagoGlobal(desde, hasta);
            RenderizarGrafico(dt, "Recaudación por Forma de Pago", SeriesChartType.Doughnut);
        }

        private void btnVentasDiaSemana_Click(object sender, EventArgs e)
        {
            ReporteNegocio negocio = new ReporteNegocio();
            DateTime desde = fechaDesde.Value.Date;
            DateTime hasta = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);

            DataTable dt = negocio.GenerarVentasPorDiaSemana(desde, hasta);
            RenderizarGrafico(dt, "Ventas Totales por Día de la Semana", SeriesChartType.Column);
        }

        // Eventos del diseñador (mantenidos para no romper la compilación)
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