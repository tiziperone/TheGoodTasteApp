using System;
using System.Windows.Forms;
using The_Good_Taste.Entidades;

namespace TheGoodTaste.UI
{
    public partial class FormReporteGerente : Form // Clase dinámica para reportes de Gerente y Vendedor
    {
        private readonly UsuarioSistema _usuarioActual;

        // Constructor por defecto (necesario para el Diseñador de Visual Studio)
        public FormReporteGerente()
        {
            InitializeComponent();
        }

        // Constructor sobrecargado que recibe el usuario activo desde FormPrincipal
        public FormReporteGerente(UsuarioSistema usuario) : this()
        {
            _usuarioActual = usuario;
        }

        private void FormReporteGerente_Load(object sender, EventArgs e)
        {
            // Aplica el tema visual marrón/dorado
            TemaVisual.AplicarEstilo(this);

            // Rango por defecto para las fechas
            fechaDesde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            fechaHasta.Value = DateTime.Today;
            fechaHasta.MaxDate = DateTime.Today;

            // Configura qué elementos ve cada perfil
            ConfigurarVistaSegunRol();
        }

        private void ConfigurarVistaSegunRol()
        {
            if (_usuarioActual == null) return;

            if (_usuarioActual.Rol == RolUsuario.Vendedor)
            {
                // --- VISTA PARA VENDEDOR ---
                this.Text = $"Reportes de Ventas - Vendedor: {_usuarioActual.NombreUsuario}";

                // Ocultamos la búsqueda de otros vendedores (el vendedor solo ve lo suyo)
                if (tituloVendedor != null) tituloVendedor.Visible = false;
                if (textBoxBuscarVendedor != null) textBoxBuscarVendedor.Visible = false;
                if (botonVentasVendedor != null) botonVentasVendedor.Visible = false;
                if (panelVentasVendedor != null) panelVentasVendedor.Visible = false;

                // Ocultamos botones de recaudación global si los tenés divididos
                // (O podés dejar botonVentas habilitado para sus propias ventas)
            }
            else if (_usuarioActual.Rol == RolUsuario.Gerente || _usuarioActual.Rol == RolUsuario.Admin)
            {
                // --- VISTA PARA GERENTE / ADMIN ---
                this.Text = "Reportes Generales de Dirección";

                // Mostramos todos los paneles de auditoría, búsquedas de vendedores y recaudación global
                if (tituloVendedor != null) tituloVendedor.Visible = true;
                if (textBoxBuscarVendedor != null) textBoxBuscarVendedor.Visible = true;
                if (botonVentasVendedor != null) botonVentasVendedor.Visible = true;
                if (panelVentasVendedor != null) panelVentasVendedor.Visible = true;
            }
        }

        // --- Eventos de Botones de Reportes ---

        private void botonVentas_Click(object sender, EventArgs e)
        {
            if (_usuarioActual?.Rol == RolUsuario.Vendedor)
            {
                // Lógica de consulta filtrada ÚNICAMENTE por _usuarioActual.IdUsuario
            }
            else
            {
                // Lógica de consulta para TODAS las ventas globales
            }
        }

        private void botonRecaudacion_Click(object sender, EventArgs e) { }
        private void botonProductoVendido_Click(object sender, EventArgs e) { }
        private void botonVentasVendedor_Click(object sender, EventArgs e) { }

        // --- Métodos del Diseñador ---
        private void tituloDesde_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void fechaDesde_ValueChanged(object sender, EventArgs e) { }
        private void fechaHasta_ValueChanged(object sender, EventArgs e) { }
        private void tituloVendedor_Click(object sender, EventArgs e) { }
        private void textBoxBuscarVendedor_TextChanged(object sender, EventArgs e) { }
        private void panelVentasVendedor_Paint(object sender, PaintEventArgs e) { }
        private void panelReportes_Paint(object sender, PaintEventArgs e) { }
    }
}