using System;
using System.Windows.Forms;

namespace TheGoodTaste.UI
{
    public partial class FormReporteGerente : Form //Clase que representa el formulario de reportes para el gerente
    {
        public FormReporteGerente()
        {
            InitializeComponent();
        }

        private void FormReporteGerente_Load(object sender, EventArgs e)
        {
            //Aplica los colores marrones/dorados
            TemaVisual.AplicarEstilo(this);

            // Rango por defecto para las fechas
            fechaDesde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            fechaHasta.Value = DateTime.Today;
            fechaHasta.MaxDate = DateTime.Today;
        }

        //Métodos vacíos requeridos por el Diseñador para que no dé error al compilar
        private void tituloDesde_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void fechaDesde_ValueChanged(object sender, EventArgs e) { }
        private void fechaHasta_ValueChanged(object sender, EventArgs e) { }
        private void botonRecaudacion_Click(object sender, EventArgs e) { }
        private void botonVentas_Click(object sender, EventArgs e) { }
        private void botonProductoVendido_Click(object sender, EventArgs e) { }
        private void tituloVendedor_Click(object sender, EventArgs e) { }
        private void textBoxBuscarVendedor_TextChanged(object sender, EventArgs e) { }
        private void botonVentasVendedor_Click(object sender, EventArgs e) { }
        private void panelVentasVendedor_Paint(object sender, PaintEventArgs e) { }
        private void panelReportes_Paint(object sender, PaintEventArgs e) { }
    }
}