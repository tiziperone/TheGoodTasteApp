using System;
using System.Windows.Forms;
using TheGoodTaste.Negocio;

namespace TheGoodTaste.UI
{
    public partial class FormClientes : Form
    {
        private readonly ClienteNegocio _negocio = new ClienteNegocio();

        public FormClientes()
        {
            InitializeComponent();
        }

        private void FormClientes_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            ConfigurarEventos();
            LimpiarCampos();
        }

        private void ConfigurarEventos()
        {
            txtNombre.KeyPress += SoloLetras_KeyPress;
            txtApellido.KeyPress += SoloLetras_KeyPress;
            textPais.KeyPress += SoloLetras_KeyPress;
            textLocalidad.KeyPress += SoloLetras_KeyPress;
            txtDni.KeyPress += SoloNumeros_KeyPress;
            txtTelefono.KeyPress += SoloNumeros_KeyPress;
            textNroAltura.KeyPress += SoloNumeros_KeyPress;

            txtDni.TextChanged += Control_Modificado;
            txtNombre.TextChanged += Control_Modificado;
            txtApellido.TextChanged += Control_Modificado;
            txtEmail.TextChanged += Control_Modificado;
            txtTelefono.TextChanged += Control_Modificado;
            textPais.TextChanged += Control_Modificado;
            textLocalidad.TextChanged += Control_Modificado;
            txtCalle.TextChanged += Control_Modificado;
            textNroAltura.TextChanged += Control_Modificado;
        }

        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar)) e.Handled = true; }
        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e) { if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true; }
        private void Control_Modificado(object sender, EventArgs e) => ActualizarEstadoBotones();

        private void ActualizarEstadoBotones()
        {
            btnLimpiar.Enabled = !string.IsNullOrWhiteSpace(txtDni.Text) || !string.IsNullOrWhiteSpace(txtNombre.Text) || !string.IsNullOrWhiteSpace(txtApellido.Text) || !string.IsNullOrWhiteSpace(txtEmail.Text) || !string.IsNullOrWhiteSpace(txtTelefono.Text) || !string.IsNullOrWhiteSpace(textPais.Text) || !string.IsNullOrWhiteSpace(textLocalidad.Text) || !string.IsNullOrWhiteSpace(txtCalle.Text) || !string.IsNullOrWhiteSpace(textNroAltura.Text);
            btnGuardar.Enabled = !string.IsNullOrWhiteSpace(txtDni.Text) && !string.IsNullOrWhiteSpace(txtNombre.Text) && !string.IsNullOrWhiteSpace(txtApellido.Text) && !string.IsNullOrWhiteSpace(txtEmail.Text) && !string.IsNullOrWhiteSpace(txtTelefono.Text);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                _negocio.GuardarCliente(txtDni.Text.Trim(), txtNombre.Text.Trim(), txtApellido.Text.Trim(), txtEmail.Text.Trim(), txtTelefono.Text.Trim());
                MessageBox.Show("Cliente guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Cliente modificado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
        }

        private void btnLimpiar_Click(object sender, EventArgs e) => LimpiarCampos();

        private void LimpiarCampos()
        {
            txtDni.Clear(); txtNombre.Clear(); txtApellido.Clear(); txtEmail.Clear(); txtTelefono.Clear(); textPais.Clear(); textLocalidad.Clear(); txtCalle.Clear(); textNroAltura.Clear();
            ActualizarEstadoBotones();
        }
    }
}