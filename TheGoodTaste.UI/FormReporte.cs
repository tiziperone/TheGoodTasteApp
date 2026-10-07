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
        private Label _lblInfoUsuario; // Label creado dinámicamente por código

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

            // Formatear el nombre para que se vea el Nombre y Apellido (si existen) o el Username
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
                    ForeColor = Color.FromArgb(230, 160, 50), // Tono dorado 
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

            // Búsqueda segura del título de la grilla (sin depender de si se llama label2)
            Label lblTituloGrilla = null;
            Control[] controlesLabel2 = Controls.Find("label2", true);
            if (controlesLabel2.Length > 0 && controlesLabel2[0] is Label l)
            {
                lblTituloGrilla = l;
            }

            if (_usuarioActual.Rol == RolUsuario.Vendedor)
            {
                this.Text = $"Mis Reportes - Vendedor: {_usuarioActual.Username}";

                botonRecaudacion.Visible = false;
                botonProductoVendido.Visible = false;
                panelVentasVendedor.Visible = false;

                // Ocultamos solo el botón de listar clientes (de gerencia)
                button1.Visible = false;

                // Dejamos el contenedor y la grilla visibles para que el vendedor vea sus ventas
                panelClientes.Visible = true;
                listaClientes.Visible = true;

                if (lblTituloGrilla != null)
                {
                    lblTituloGrilla.Text = "Detalle de Mis Ventas";
                }

                botonVentas.Text = "Mis Ventas";
            }
            else
            {
                this.Text = "Reportes Estratégicos - Gerencia";
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

        // Eventos del diseñador (se dejan vacíos para no romper referencias del archivo FormReportes.Designer.cs)
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