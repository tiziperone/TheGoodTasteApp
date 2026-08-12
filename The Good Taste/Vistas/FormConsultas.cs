using System;
using System.Windows.Forms;

namespace The_Good_Taste.Vistas
{
    public partial class FormConsultas : Form
    {
        public FormConsultas()
        {
            InitializeComponent();
            CargarConsultas();
        }

        private void CargarConsultas()
        {
            // Carga la lista de consultas en dgvConsultas
            // dgvConsultas.DataSource = ConsultaDatos.ObtenerTodas();
        }

        private void btnMarcarLeido_Click(object sender, EventArgs e)
        {
            // Cambia el estado de leí do/no leído (estado = 1/0)
        }

        private void btnResponder_Click(object sender, EventArgs e)
        {
            // Envía la respuesta por correo o actualiza el registro en la BD
        }
    }
}