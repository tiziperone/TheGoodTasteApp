using System;
using System.Drawing;
using System.Windows.Forms;

namespace The_Good_Taste.Vistas
{
	public partial class FormPrincipal : Form
	{
		private Form formularioActivo = null;

		public FormPrincipal()
		{
			InitializeComponent();
			ConfigurarDisenoBase();
		}

		private void FormPrincipal_Load(object sender, EventArgs e)
		{
			MostrarInicio();
		}

		private void ConfigurarDisenoBase()
		{
			this.Text = "The Good Taste - Sistema de Gestión";
			this.StartPosition = FormStartPosition.CenterScreen;
			this.MinimumSize = new Size(1100, 650);
			this.WindowState = FormWindowState.Maximized;
			this.BackColor = Color.FromArgb(33, 37, 41);
		}

		private void AbrirFormularioEnPanel(Form formularioHijo)
		{
			if (formularioActivo != null)
			{
				formularioActivo.Close();
			}

			formularioActivo = formularioHijo;

			formularioHijo.TopLevel = false;
			formularioHijo.FormBorderStyle = FormBorderStyle.None;
			formularioHijo.Dock = DockStyle.Fill;

			pnlContenedor.Controls.Clear();
			pnlContenedor.Controls.Add(formularioHijo);
			pnlContenedor.Tag = formularioHijo;
			formularioHijo.Show();
		}

		private void MostrarInicio()
		{
			if (formularioActivo != null)
			{
				formularioActivo.Close();
				formularioActivo = null;
			}

			pnlContenedor.Controls.Clear();
			pnlContenedor.Controls.Add(pnlInicioDashboard);
			pnlInicioDashboard.Dock = DockStyle.Fill;
			pnlInicioDashboard.Visible = true;
		}

		// ==========================================
		// EVENTOS DE NAVEGACIÓN
		// ==========================================

		private void btnMenuInicio_Click(object sender, EventArgs e)
		{
			MostrarInicio();
		}

		private void btnMenuProductos_Click(object sender, EventArgs e)
		{
			AbrirFormularioEnPanel(new FormProductos());
		}

		private void btnMenuPedidos_Click(object sender, EventArgs e)
		{
			AbrirFormularioEnPanel(new FormPuntoVenta());
		}

		private void btnMenuClientes_Click(object sender, EventArgs e)
		{
			AbrirFormularioEnPanel(new FormClientes());
		}

		private void btnMenuConsultas_Click(object sender, EventArgs e)
		{
			AbrirFormularioEnPanel(new FormConsultas());
		}

		private void btnCerrarSesion_Click(object sender, EventArgs e)
		{
			var respuesta = MessageBox.Show(
				"¿Desea cerrar sesión y salir del sistema?",
				"Confirmar Salida",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Question
			);

			if (respuesta == DialogResult.Yes)
			{
				Application.Exit();
			}
		}
	}
}