using System;
using System.Data;
using System.Windows.Forms;
using TheGoodTaste.Negocio;

namespace TheGoodTaste.UI
{
    public partial class FormClientes : Form
    {
        private readonly ClienteNegocio _negocio = new ClienteNegocio();
        private string _dniSeleccionado = ""; // Guardará el DNI del cliente que se seleccione en la grilla para modificarlo

        public FormClientes()
        {
            InitializeComponent();
        }

        private void FormClientes_Load(object sender, EventArgs e)
        {
            TemaVisual.AplicarEstilo(this);
            ConfigurarEventos();
            LimpiarCampos();
            CargarGrilla(true); // Carga los clientes activos por defecto al iniciar
        }

        private void ConfigurarEventos()
        {
            txtNombre.KeyPress += SoloLetras_KeyPress;
            txtApellido.KeyPress += SoloLetras_KeyPress;
            textPais.KeyPress += SoloLetras_KeyPress;
            textLocalidad.KeyPress += SoloLetras_KeyPress;
            txtProvincia.KeyPress += SoloLetras_KeyPress;

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
            txtProvincia.TextChanged += Control_Modificado;
            txtCalle.TextChanged += Control_Modificado;
            textNroAltura.TextChanged += Control_Modificado;

            btnActivo.Click += btnActivo_Click;
            btnInactivo.Click += btnInactivo_Click;

            // Evento para seleccionar cliente de la grilla
            dgvClientes.CellDoubleClick += dgvClientes_CellDoubleClick;
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

            btnGuardar.Enabled = !string.IsNullOrWhiteSpace(txtDni.Text) &&
                                 !string.IsNullOrWhiteSpace(txtNombre.Text) &&
                                 !string.IsNullOrWhiteSpace(txtApellido.Text) &&
                                 !string.IsNullOrWhiteSpace(txtEmail.Text) &&
                                 !string.IsNullOrWhiteSpace(txtTelefono.Text);
        }

        // --- MÉTODOS DE LA GRILLA Y BOTONES DE FILTRO ---

        private void CargarGrilla(bool estadoActivo)
        {
            try
            {
                dgvClientes.DataSource = _negocio.ObtenerClientes(estadoActivo);

                // Verificamos que la grilla tenga columnas antes de intentar ocultarlas
                if (dgvClientes.Columns.Count > 0)
                {
                    // Ocultamos las columnas que NO queremos que se vean
                    dgvClientes.Columns["fechaNaciminetoCliente"].Visible = false;
                    dgvClientes.Columns["telefonoCliente"].Visible = false;
                    dgvClientes.Columns["paisCliente"].Visible = false;
                    dgvClientes.Columns["provinciaCliente"].Visible = false;

                    // Cambiamos el texto del encabezado de las columnas que SÍ se ven para que quede prolijo
                    dgvClientes.Columns["dniCliente"].HeaderText = "DNI";
                    dgvClientes.Columns["nombreCliente"].HeaderText = "Nombre";
                    dgvClientes.Columns["apellidoCliente"].HeaderText = "Apellido";
                    dgvClientes.Columns["correoCliente"].HeaderText = "Correo";
                    dgvClientes.Columns["localidadCliente"].HeaderText = "Localidad";
                    dgvClientes.Columns["calleCliente"].HeaderText = "Calle";
                    dgvClientes.Columns["altura"].HeaderText = "Nro";
                }

                // Habilitamos o deshabilitamos el botón de Modificar dependiendo de si estamos viendo los Activos o Inactivos
                buttonMod.Enabled = estadoActivo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActivo_Click(object sender, EventArgs e)
        {
            CargarGrilla(true);
        }

        private void btnInactivo_Click(object sender, EventArgs e)
        {
            CargarGrilla(false);
            LimpiarCampos(); // Limpiamos por si había alguien seleccionado al cambiar de pestaña
        }

        private void dgvClientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificamos que se haya hecho clic en una fila válida (y que el botón de modificar esté activo)
            if (e.RowIndex >= 0 && buttonMod.Enabled)
            {
                DataGridViewRow fila = dgvClientes.Rows[e.RowIndex];

                // Aunque las columnas estén ocultas visualmente, los datos siguen ahí y podemos pasarlos a los TextBox
                _dniSeleccionado = fila.Cells["dniCliente"].Value.ToString();
                txtDni.Text = _dniSeleccionado;
                txtNombre.Text = fila.Cells["nombreCliente"].Value.ToString();
                txtApellido.Text = fila.Cells["apellidoCliente"].Value.ToString();

                if (DateTime.TryParse(fila.Cells["fechaNaciminetoCliente"].Value.ToString(), out DateTime fechaNac))
                    dtpFechaNacimiento.Value = fechaNac;

                txtEmail.Text = fila.Cells["correoCliente"].Value.ToString();
                txtTelefono.Text = fila.Cells["telefonoCliente"].Value.ToString();
                textPais.Text = fila.Cells["paisCliente"].Value.ToString();
                textLocalidad.Text = fila.Cells["localidadCliente"].Value.ToString();
                txtProvincia.Text = fila.Cells["provinciaCliente"].Value.ToString();
                txtCalle.Text = fila.Cells["calleCliente"].Value.ToString();
                textNroAltura.Text = fila.Cells["altura"].Value.ToString();
            }
            else if (!buttonMod.Enabled)
            {
                MessageBox.Show("No se pueden editar clientes inactivos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- GUARDAR, MODIFICAR Y LIMPIAR ---

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(_dniSeleccionado))
                {
                    MessageBox.Show("Tiene un cliente seleccionado. Si desea guardar uno nuevo, presione 'Limpiar' primero o use el botón 'Modificar'.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _negocio.GuardarCliente(txtDni.Text.Trim(), txtNombre.Text.Trim(), txtApellido.Text.Trim(), dtpFechaNacimiento.Value, txtEmail.Text.Trim(), txtTelefono.Text.Trim(), textPais.Text.Trim(), textLocalidad.Text.Trim(), txtProvincia.Text.Trim(), txtCalle.Text.Trim(), textNroAltura.Text.Trim());

                MessageBox.Show("Cliente guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
                CargarGrilla(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonMod_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_dniSeleccionado))
            {
                MessageBox.Show("Seleccione un cliente de la lista haciendo doble clic para modificarlo.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Resumen para confirmar cambios antes de impactar en base de datos
            string mensajeConfirmacion = $"Se aplicarán los siguientes cambios:\n\n" +
                                         $"- DNI: {txtDni.Text}\n" +
                                         $"- Nombre: {txtNombre.Text} {txtApellido.Text}\n" +
                                         $"- Email: {txtEmail.Text}\n" +
                                         $"- Teléfono: {txtTelefono.Text}\n\n" +
                                         $"¿Desea aceptar y guardar los cambios?";

            if (MessageBox.Show(mensajeConfirmacion, "Confirmar Modificación", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    _negocio.ModificarCliente(_dniSeleccionado, txtDni.Text.Trim(), txtNombre.Text.Trim(), txtApellido.Text.Trim(), dtpFechaNacimiento.Value, txtEmail.Text.Trim(), txtTelefono.Text.Trim(), textPais.Text.Trim(), textLocalidad.Text.Trim(), txtProvincia.Text.Trim(), txtCalle.Text.Trim(), textNroAltura.Text.Trim());

                    MessageBox.Show("Cliente modificado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                    CargarGrilla(true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnLimpiar_Click_1(object sender, EventArgs e) => LimpiarCampos();

        private void LimpiarCampos()
        {
            _dniSeleccionado = ""; // Reseteamos la selección
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
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

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
    }
}