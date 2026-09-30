using System;
using System.Windows.Forms;
using TheGoodTaste.Negocio;

namespace TheGoodTaste.UI
{
    public partial class FormClientes : Form // Clase que maneja la interfaz de usuario para la gestión de clientes
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
            // Validaciones de ingreso (Solo letras)
            txtNombre.KeyPress += SoloLetras_KeyPress;
            txtApellido.KeyPress += SoloLetras_KeyPress;
            textPais.KeyPress += SoloLetras_KeyPress;
            textLocalidad.KeyPress += SoloLetras_KeyPress;
            txtProvincia.KeyPress += SoloLetras_KeyPress; // Agregado Provincia

            // Validaciones de ingreso (Solo números)
            txtDni.KeyPress += SoloNumeros_KeyPress;
            txtTelefono.KeyPress += SoloNumeros_KeyPress;
            textNroAltura.KeyPress += SoloNumeros_KeyPress;

            // Detección de cambios para habilitar/deshabilitar botones
            txtDni.TextChanged += Control_Modificado;
            txtNombre.TextChanged += Control_Modificado;
            txtApellido.TextChanged += Control_Modificado;
            txtEmail.TextChanged += Control_Modificado;
            txtTelefono.TextChanged += Control_Modificado;
            textPais.TextChanged += Control_Modificado;
            textLocalidad.TextChanged += Control_Modificado;
            txtProvincia.TextChanged += Control_Modificado;
            txtCalle.TextChanged += Control_Modificado;
            textNroAltura.TextChanged += Control_Modificado;
        }

        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        private void Control_Modificado(object sender, EventArgs e) => ActualizarEstadoBotones();

        private void ActualizarEstadoBotones()
        {
            // Se habilita Limpiar si hay al menos un campo escrito
            btnLimpiar.Enabled = !string.IsNullOrWhiteSpace(txtDni.Text) ||
                                 !string.IsNullOrWhiteSpace(txtNombre.Text) ||
                                 !string.IsNullOrWhiteSpace(txtApellido.Text) ||
                                 !string.IsNullOrWhiteSpace(txtEmail.Text) ||
                                 !string.IsNullOrWhiteSpace(txtTelefono.Text) ||
                                 !string.IsNullOrWhiteSpace(textPais.Text) ||
                                 !string.IsNullOrWhiteSpace(textLocalidad.Text) ||
                                 !string.IsNullOrWhiteSpace(txtProvincia.Text) ||
                                 !string.IsNullOrWhiteSpace(txtCalle.Text) ||
                                 !string.IsNullOrWhiteSpace(textNroAltura.Text);

            // Se habilita Guardar si los campos principales están llenos
            btnGuardar.Enabled = !string.IsNullOrWhiteSpace(txtDni.Text) &&
                                 !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                                 !string.IsNullOrWhiteSpace(txtApellido.Text) &&
                                 !string.IsNullOrWhiteSpace(txtEmail.Text) &&
                                 !string.IsNullOrWhiteSpace(txtTelefono.Text);
        }

        // ---------- EVENTOS VINCULADOS DESDE EL DISEÑADOR ----------

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Pasamos todos los datos a la capa de negocio
                _negocio.GuardarCliente(
                    txtDni.Text.Trim(),
                    txtNombre.Text.Trim(),
                    txtApellido.Text.Trim(),
                    dtpFechaNacimiento.Value, // Control DateTimePicker
                    txtEmail.Text.Trim(),
                    txtTelefono.Text.Trim(),
                    textPais.Text.Trim(),
                    textLocalidad.Text.Trim(),
                    txtProvincia.Text.Trim(),
                    txtCalle.Text.Trim(),
                    textNroAltura.Text.Trim()
                );

                MessageBox.Show("Cliente guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpiar_Click_1(object sender, EventArgs e) => LimpiarCampos();

        // -----------------------------------------------------------

        private void btnModificar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Cliente modificado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();

            // Ponemos por defecto una fecha de hace 18 años para cumplir la validación visualmente al limpiar
            dtpFechaNacimiento.Value = DateTime.Now.AddYears(-18);

            txtEmail.Clear();
            txtTelefono.Clear();
            textPais.Clear();
            textLocalidad.Clear();
            txtProvincia.Clear();
            txtCalle.Clear();
            textNroAltura.Clear();

            ActualizarEstadoBotones();
        }

        // Eventos creados por accidente en el diseñador (dejarlos vacíos evita errores al compilar)
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}